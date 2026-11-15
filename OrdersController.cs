namespace DeliveryExpress.Controllers;

public class OrdersController : Controller
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("/pedido/{id:int}")]
    public IActionResult Confirmacao(int id)
    {
        var order = _db.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .FirstOrDefault(item => item.Id == id);

        if (order == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("NaoEncontrado");
        }

        return View(order);
    }

    [HttpGet("/meus-pedidos")]
    public IActionResult Buscar(string? telefone)
    {
        var model = new OrderSearchViewModel
        {
            Phone = telefone?.Trim() ?? ""
        };

        var digits = PhoneNumber.Digits(model.Phone);
        if (digits.Length == 0)
            return View(model);

        model.Searched = true;
        model.Orders = _db.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.Phone == digits)
            .OrderByDescending(order => order.CreatedAt)
            .ToList();

        return View(model);
    }
}
