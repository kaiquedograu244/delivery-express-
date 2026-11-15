namespace DeliveryExpress.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("/")]
    public IActionResult Index(string? q, string? categoria)
    {
        var restaurants = _db.Restaurants.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(categoria))
            restaurants = restaurants.Where(restaurant => restaurant.Category == categoria);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            restaurants = restaurants.Where(restaurant =>
                restaurant.Name.Contains(term) ||
                restaurant.Category.Contains(term) ||
                restaurant.Description.Contains(term));
        }

        var model = new HomeViewModel
        {
            Restaurants = restaurants
                .OrderByDescending(restaurant => restaurant.Rating)
                .ThenBy(restaurant => restaurant.Name)
                .ToList(),
            Categories = _db.Restaurants
                .Select(restaurant => restaurant.Category)
                .Distinct()
                .OrderBy(category => category)
                .ToList(),
            Query = q?.Trim() ?? "",
            Category = categoria?.Trim() ?? ""
        };

        return View(model);
    }

    [HttpGet("/restaurante/{id:int}")]
    public IActionResult Cardapio(int id)
    {
        var restaurant = _db.Restaurants
            .AsNoTracking()
            .Include(item => item.Products)
            .FirstOrDefault(item => item.Id == id);

        if (restaurant == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("NaoEncontrado");
        }

        restaurant.Products = restaurant.Products
            .OrderBy(product => product.Category)
            .ThenBy(product => product.Name)
            .ToList();

        return View(restaurant);
    }

    [HttpGet("/erro")]
    public IActionResult Erro() => View("Error");
}
