namespace DeliveryExpress.Controllers;

[Route("carrinho")]
public class CartController : Controller
{
    private readonly CartService _cart;

    public CartController(CartService cart)
    {
        _cart = cart;
    }

    [HttpGet("")]
    public IActionResult Index() => View(_cart.Describe());

    [HttpPost("adicionar")]
    public IActionResult Adicionar(int productId, int quantity, int restaurantId)
    {
        var result = _cart.Add(productId, quantity);
        TempData[result.Ok ? "Ok" : "Erro"] = result.Message;
        if (restaurantId > 0)
            return RedirectToAction("Cardapio", "Home", new { id = restaurantId });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("atualizar")]
    public IActionResult Atualizar(int productId, int quantity)
    {
        _cart.Update(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("remover")]
    public IActionResult Remover(int productId)
    {
        _cart.Update(productId, 0);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("esvaziar")]
    public IActionResult Esvaziar()
    {
        _cart.Clear();
        TempData["Ok"] = "Carrinho esvaziado.";
        return RedirectToAction(nameof(Index));
    }
}
