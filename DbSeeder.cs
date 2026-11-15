namespace DeliveryExpress.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Restaurants.Any())
            return;

        db.Restaurants.AddRange(
            Place("Burger da Esquina", "Smash burgers e batata crocante, feitos na hora.", "Lanches", 30, 6.90m, 4.8m,
                Item("Burgers", "Smash burger", "Blend 160g, queijo e pão brioche.", 32.90m),
                Item("Burgers", "Duplo cheddar", "Dois blends, cheddar e picles.", 39.90m),
                Item("Acompanhamentos", "Batata rústica", "Porção com sal e alecrim.", 16.00m),
                Item("Bebidas", "Milk-shake de chocolate", "Copo de 400ml.", 18.00m)),
            Place("Pizzaria Forno a Lenha", "Massa de longa fermentação e forno a lenha.", "Pizza", 45, 8.00m, 4.7m,
                Item("Pizzas", "Mussarela", "Molho de tomate, mussarela e orégano.", 54.00m),
                Item("Pizzas", "Calabresa", "Calabresa, cebola e mussarela.", 58.00m),
                Item("Pizzas", "Portuguesa", "Presunto, ovo, ervilha, cebola e mussarela.", 62.00m),
                Item("Bebidas", "Refrigerante 2L", "Garrafa gelada.", 12.00m)),
            Place("Sushi do Kaique", "Peixe fresco e combinados para dividir.", "Japonesa", 40, 9.90m, 4.9m,
                Item("Combinados", "Combo 16 peças", "Niguiri, uramaki e hot roll.", 49.90m),
                Item("Peças", "Hot roll", "8 peças empanadas com tarê.", 36.00m),
                Item("Peças", "Temaki de salmão", "Cone de alga com salmão e cream cheese.", 28.00m),
                Item("Entradas", "Sunomono", "Salada de pepino agridoce.", 14.00m)),
            Place("Açaí da Praça", "Açaí batido na hora, com granola crocante.", "Açaí", 20, 5.00m, 4.6m,
                Item("Tigelas", "Açaí 300ml", "Açaí, granola e banana.", 16.00m),
                Item("Tigelas", "Açaí 500ml", "Açaí, granola, banana e leite condensado.", 22.00m),
                Item("Tigelas", "Açaí 700ml", "Tigela grande com três acompanhamentos.", 28.00m),
                Item("Bebidas", "Vitamina de banana", "Copo de 400ml.", 12.00m)),
            Place("Cantina da Nonna", "Massas e sobremesas no estilo de casa.", "Italiana", 35, 7.50m, 4.8m,
                Item("Massas", "Lasanha à bolonhesa", "Camadas de massa, molho e queijo.", 42.00m),
                Item("Massas", "Espaguete à bolonhesa", "Molho de carne de panela.", 36.00m),
                Item("Massas", "Risoto de funghi", "Arroz arbóreo e cogumelos.", 39.00m),
                Item("Sobremesas", "Tiramisu", "Fatia gelada com café e cacau.", 18.00m)),
            Place("Grill do Mercado", "Pratos feitos, feijoada e grelhados.", "Brasileira", 35, 6.00m, 4.5m,
                Item("Pratos", "PF de picanha", "Arroz, feijão, farofa e salada.", 44.90m),
                Item("Pratos", "Feijoada individual", "Acompanha arroz, couve e laranja.", 38.00m),
                Item("Pratos", "Frango grelhado", "Peito grelhado, arroz e legumes.", 32.00m),
                Item("Bebidas", "Suco natural", "Laranja ou limão, copo 400ml.", 10.00m)));

        db.SaveChanges();
    }

    private static Restaurant Place(
        string name,
        string description,
        string category,
        int minutes,
        decimal fee,
        decimal rating,
        params Product[] products)
    {
        return new Restaurant
        {
            Name = name,
            Description = description,
            Category = category,
            DeliveryMinutes = minutes,
            DeliveryFee = fee,
            Rating = rating,
            Products = products.ToList()
        };
    }

    private static Product Item(string category, string name, string description, decimal price)
    {
        return new Product
        {
            Category = category,
            Name = name,
            Description = description,
            Price = price
        };
    }
}
