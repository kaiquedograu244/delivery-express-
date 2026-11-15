namespace DeliveryExpress.Models;

public class KitchenViewModel
{
    public bool Authorized { get; set; }

    public List<Order> Orders { get; set; } = new();
}
