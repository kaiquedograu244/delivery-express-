namespace DeliveryExpress.Services;

public sealed record PlaceOrderResult(Order? Order, string? Error)
{
    public bool Ok => Order != null && string.IsNullOrEmpty(Error);
}

public class OrderService
{
    private readonly AppDbContext _db;
    private readonly CartService _cart;

    public OrderService(AppDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    public PlaceOrderResult Place(CheckoutForm form)
    {
        var lines = _cart.Describe();
        if (lines.Count == 0)
            return new PlaceOrderResult(null, "Seu carrinho está vazio.");
        if (lines.Any(line => line.Missing))
            return new PlaceOrderResult(null, "Um item do carrinho não está mais no cardápio.");

        var restaurants = lines.Select(line => line.RestaurantId).Distinct().ToList();
        if (restaurants.Count != 1)
            return new PlaceOrderResult(null, "O pedido precisa ser de um único restaurante.");

        var phone = PhoneNumber.Digits(form.Telefone);
        if (phone.Length is < 10 or > 11)
            return new PlaceOrderResult(null, "Informe um telefone com DDD.");

        var payment = form.Pagamento.Trim();
        if (!PaymentMethods.All.Contains(payment))
            return new PlaceOrderResult(null, "Forma de pagamento inválida.");

        var subtotal = lines.Sum(line => line.LineTotal);
        var fee = lines[0].DeliveryFee;
        var order = new Order
        {
            CustomerName = form.Nome.Trim(),
            Phone = phone,
            Address = form.Endereco.Trim(),
            Complement = (form.Complemento ?? "").Trim(),
            PaymentMethod = payment,
            Notes = (form.Observacao ?? "").Trim(),
            Subtotal = subtotal,
            DeliveryFee = fee,
            Total = subtotal + fee,
            Status = OrderStatuses.Received,
            CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
            Items = lines.Select(line => new OrderItem
            {
                ProductId = line.ProductId,
                ProductName = line.ProductName,
                RestaurantName = line.RestaurantName,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            }).ToList()
        };

        using var transaction = _db.Database.BeginTransaction();
        _db.Orders.Add(order);
        _db.SaveChanges();
        transaction.Commit();
        _cart.Clear();
        return new PlaceOrderResult(order, null);
    }
}
