namespace DeliveryExpress.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    [MaxLength(120)]
    public string ProductName { get; set; } = "";

    [MaxLength(120)]
    public string RestaurantName { get; set; } = "";

    public int Quantity { get; set; }

    [Precision(10, 2)]
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; }
}
