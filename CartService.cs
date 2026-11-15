using System.Text.Json;

namespace DeliveryExpress.Services;

public sealed record CartResult(bool Ok, string Message);

public class CartService
{
    private const string SessionKey = "delivery.cart";
    private readonly IHttpContextAccessor _http;
    private readonly AppDbContext _db;

    public CartService(IHttpContextAccessor http, AppDbContext db)
    {
        _http = http;
        _db = db;
    }

    public int Count => Get().Sum(line => line.Quantity);

    public CartResult Add(int productId, int quantity)
    {
        quantity = Math.Clamp(quantity, 1, 20);
        var product = _db.Products.AsNoTracking().FirstOrDefault(item => item.Id == productId);
        if (product == null)
            return new CartResult(false, "Esse item não está mais no cardápio.");

        var items = Get();
        if (items.Count > 0)
        {
            var anchor = _db.Products.AsNoTracking().FirstOrDefault(item => item.Id == items[0].ProductId);
            if (anchor == null)
                items.Clear();
            else if (anchor.RestaurantId != product.RestaurantId)
                return new CartResult(false, "Seu carrinho já tem itens de outro restaurante. Finalize o pedido ou esvazie o carrinho.");
        }

        var existing = items.FirstOrDefault(item => item.ProductId == product.Id);
        if (existing == null)
            items.Add(new CartLine { ProductId = product.Id, Quantity = quantity });
        else
            existing.Quantity = Math.Min(20, existing.Quantity + quantity);

        Save(items);
        return new CartResult(true, $"{product.Name} adicionado ao carrinho.");
    }

    public void Update(int productId, int quantity)
    {
        var items = Get();
        var existing = items.FirstOrDefault(item => item.ProductId == productId);
        if (existing == null)
            return;

        if (quantity < 1)
            items.Remove(existing);
        else
            existing.Quantity = Math.Min(20, quantity);

        Save(items);
    }

    public void Clear() => Save(new List<CartLine>());

    public IReadOnlyList<CartLineView> Describe()
    {
        var cart = Get();
        if (cart.Count == 0)
            return Array.Empty<CartLineView>();

        var ids = cart.Select(line => line.ProductId).ToList();
        var products = _db.Products
            .AsNoTracking()
            .Include(product => product.Restaurant)
            .Where(product => ids.Contains(product.Id))
            .ToList();

        return cart.Select(line =>
        {
            var product = products.FirstOrDefault(item => item.Id == line.ProductId);
            if (product == null)
            {
                return new CartLineView
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    ProductName = "Item indisponível",
                    Missing = true
                };
            }

            return new CartLineView
            {
                ProductId = product.Id,
                RestaurantId = product.RestaurantId,
                ProductName = product.Name,
                RestaurantName = product.Restaurant?.Name ?? "",
                DeliveryFee = product.Restaurant?.DeliveryFee ?? 0,
                Quantity = line.Quantity,
                UnitPrice = product.Price
            };
        }).ToList();
    }

    private List<CartLine> Get()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(json))
            return new List<CartLine>();

        try
        {
            return JsonSerializer.Deserialize<List<CartLine>>(json) ?? new List<CartLine>();
        }
        catch (JsonException)
        {
            return new List<CartLine>();
        }
    }

    private void Save(List<CartLine> items) =>
        Session.SetString(SessionKey, JsonSerializer.Serialize(items));

    private ISession Session =>
        _http.HttpContext?.Session ?? throw new InvalidOperationException("Sessão indisponível.");
}
