using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OntarioParksExplorer.Api.Data;

namespace OntarioParksExplorer.Api.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ParksDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Create and open the SQLite connection if not already open
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();
            }

            // Use in-memory SQLite for tests
            services.AddDbContext<ParksDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });

        // Configure the host to skip startup database migration/seeding
        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host first
        var host = base.CreateHost(builder);

        // Seed the database after the host is created
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ParksDbContext>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed test data
        var seedDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "seed-data", "parks.json");
        if (File.Exists(seedDataPath))
        {
            DataSeeder.SeedAsync(context, seedDataPath).Wait();
        }

        return host;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}
