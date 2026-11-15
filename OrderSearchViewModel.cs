namespace DeliveryExpress.Models;

public class OrderSearchViewModel
{
    public string Phone { get; set; } = "";

    public bool Searched { get; set; }

    public List<Order> Orders { get; set; } = new();
}
