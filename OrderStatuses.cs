namespace DeliveryExpress.Models;

public static class OrderStatuses
{
    public const string Received = "Recebido";
    public const string Preparing = "Preparando";
    public const string OutForDelivery = "Saiu para entrega";
    public const string Delivered = "Entregue";

    public static readonly string[] Flow = [Received, Preparing, OutForDelivery, Delivered];

    public static string Next(string current)
    {
        var index = Array.IndexOf(Flow, current);
        if (index < 0 || index >= Flow.Length - 1)
            return Delivered;
        return Flow[index + 1];
    }

    public static bool IsLast(string current) => current == Delivered;
}
