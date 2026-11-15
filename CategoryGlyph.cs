namespace DeliveryExpress.Services;

public static class CategoryGlyph
{
    public static string For(string category) => category switch
    {
        "Lanches" => "🍔",
        "Pizza" => "🍕",
        "Japonesa" => "🍣",
        "Açaí" => "🍇",
        "Italiana" => "🍝",
        "Brasileira" => "🍖",
        _ => "🍽️"
    };
}
