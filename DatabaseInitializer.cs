using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace DeliveryExpress.Data;

public static class DatabaseInitializer
{
    public static void Prepare(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DeliveryExpress");
        var connectionString = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string Default ausente.");

        Exception? last = null;
        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                EnsureDatabase(connectionString);
                db.Database.EnsureCreated();
                DbSeeder.Seed(db);
                logger.LogInformation("Banco delivery_express pronto.");
                return;
            }
            catch (Exception ex)
            {
                last = ex;
                logger.LogWarning("Tentativa {Attempt}/30 de conectar no MySQL: {Message}", attempt, ex.Message);
                Thread.Sleep(1000);
            }
        }

        throw new InvalidOperationException(
            "Não foi possível conectar no MySQL em 127.0.0.1:3306. Abra o site com abrir-site.bat.",
            last);
    }

    private static void EnsureDatabase(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        if (string.IsNullOrWhiteSpace(database) || !Regex.IsMatch(database, "^[A-Za-z0-9_]+$"))
            throw new InvalidOperationException("Nome do banco inválido na connection string.");

        builder.Database = "";
        using var connection = new MySqlConnection(builder.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{database}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
        command.ExecuteNonQuery();
    }
}
