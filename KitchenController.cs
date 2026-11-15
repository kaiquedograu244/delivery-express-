namespace DeliveryExpress.Controllers;

public class KitchenController : Controller
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public KitchenController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpGet("/cozinha")]
    public IActionResult Index()
    {
        if (!Authorized)
            return View(new KitchenViewModel());

        var orders = _db.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .Take(40)
            .ToList();

        return View(new KitchenViewModel { Authorized = true, Orders = orders });
    }

    [HttpPost("/cozinha/entrar")]
    public IActionResult Entrar(string? pin)
    {
        var expected = _config["Kitchen:Pin"] ?? "";
        if (expected.Length > 0 && string.Equals(pin, expected, StringComparison.Ordinal))
        {
            HttpContext.Session.SetString("kitchen", "ok");
            return RedirectToAction(nameof(Index));
        }

        TempData["Erro"] = "PIN incorreto.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/cozinha/sair")]
    public IActionResult Sair()
    {
        HttpContext.Session.Remove("kitchen");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/cozinha/avancar/{id:int}")]
    public IActionResult Avancar(int id)
    {
        if (!Authorized)
            return RedirectToAction(nameof(Index));

        var order = _db.Orders.FirstOrDefault(item => item.Id == id);
        if (order == null)
            return RedirectToAction(nameof(Index));

        order.Status = OrderStatuses.Next(order.Status);
        _db.SaveChanges();
        TempData["Ok"] = $"Pedido #{order.Id:D4} agora está {order.Status}.";
        return RedirectToAction(nameof(Index));
    }

    private bool Authorized => HttpContext.Session.GetString("kitchen") == "ok";
}
