namespace DeliveryExpress.Models;

public class HomeViewModel
{
    public List<Restaurant> Restaurants { get; set; } = new();

    public List<string> Categories { get; set; } = new();

    public string Query { get; set; } = "";

    public string Category { get; set; } = "";
}
