namespace DeliveryExpress.Controllers;

public class CheckoutController : Controller
{
    private readonly CartService _cart;
    private readonly OrderService _orders;

    public CheckoutController(CartService cart, OrderService orders)
    {
        _cart = cart;
        _orders = orders;
    }

    [HttpGet("/checkout")]
    public IActionResult Index()
    {
        var model = Build();
        if (model.Lines.Count == 0)
        {
            TempData["Erro"] = "Seu carrinho está vazio.";
            return RedirectToAction("Index", "Cart");
        }

        if (model.Lines.Any(line => line.Missing) || model.Lines.Select(line => line.RestaurantId).Distinct().Count() != 1)
        {
            TempData["Erro"] = "Revise o carrinho antes de finalizar.";
            return RedirectToAction("Index", "Cart");
        }

        return View(model);
    }

    [HttpPost("/checkout")]
    public IActionResult Index(CheckoutViewModel posted)
    {
        var model = Build();
        model.Form = posted.Form ?? new CheckoutForm();

        if (model.Lines.Count == 0 || model.Lines.Any(line => line.Missing))
        {
            TempData["Erro"] = "Revise o carrinho antes de finalizar.";
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
            return View(model);

        var result = _orders.Place(model.Form);
        if (!result.Ok || result.Order == null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Não foi possível fechar o pedido.");
            return View(model);
        }

        return RedirectToAction("Confirmacao", "Orders", new { id = result.Order.Id });
    }

    private CheckoutViewModel Build()
    {
        var lines = _cart.Describe().ToList();
        var subtotal = lines.Where(line => !line.Missing).Sum(line => line.LineTotal);
        var fee = lines.Count == 0 ? 0 : lines[0].DeliveryFee;
        return new CheckoutViewModel
        {
            Lines = lines,
            RestaurantName = lines.FirstOrDefault()?.RestaurantName ?? "",
            Subtotal = subtotal,
            DeliveryFee = fee,
            Total = subtotal + fee
        };
    }
}
