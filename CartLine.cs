namespace DeliveryExpress.Models;

public class CartLine
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}

public class CartLineView
{
    public int ProductId { get; set; }

    public int RestaurantId { get; set; }

    public string ProductName { get; set; } = "";

    public string RestaurantName { get; set; } = "";

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DeliveryFee { get; set; }

    public bool Missing { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
