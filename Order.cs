using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryExpress.Models;

[Table("Orders")]
public class Order
{
    public int Id { get; set; }

    [MaxLength(80)]
    public string CustomerName { get; set; } = "";

    [MaxLength(20)]
    public string Phone { get; set; } = "";

    [MaxLength(200)]
    public string Address { get; set; } = "";

    [MaxLength(80)]
    public string Complement { get; set; } = "";

    [MaxLength(40)]
    public string PaymentMethod { get; set; } = "";

    [MaxLength(240)]
    public string Notes { get; set; } = "";

    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    [Precision(10, 2)]
    public decimal DeliveryFee { get; set; }

    [Precision(10, 2)]
    public decimal Total { get; set; }

    [MaxLength(40)]
    public string Status { get; set; } = OrderStatuses.Received;

    public DateTime CreatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
