namespace DeliveryExpress.Models;

public class CheckoutViewModel
{
    public CheckoutForm Form { get; set; } = new();

    public List<CartLineView> Lines { get; set; } = new();

    public string RestaurantName { get; set; } = "";

    public decimal Subtotal { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal Total { get; set; }
}
