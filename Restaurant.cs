namespace DeliveryExpress.Models;

public class Restaurant
{
    public int Id { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = "";

    [MaxLength(400)]
    public string Description { get; set; } = "";

    [MaxLength(40)]
    public string Category { get; set; } = "";

    public int DeliveryMinutes { get; set; }

    [Precision(10, 2)]
    public decimal DeliveryFee { get; set; }

    [Precision(3, 1)]
    public decimal Rating { get; set; }

    public List<Product> Products { get; set; } = new();
}
