namespace DeliveryExpress.Models;

public class Product
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = "";

    [MaxLength(400)]
    public string Description { get; set; } = "";

    [MaxLength(40)]
    public string Category { get; set; } = "";

    [Precision(10, 2)]
    public decimal Price { get; set; }

    public Restaurant? Restaurant { get; set; }
}
