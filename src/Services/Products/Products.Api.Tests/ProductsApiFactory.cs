using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Api.Data;
using Products.Api.Models;

// Every test class shares one SQL Server database, so they must not run at the same time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Products.Api.Tests;

/// <summary>
/// Hosts Products.Api in memory against a real SQL Server LocalDB database
/// (ProductsPracticeTests, separate from the one "dotnet run" uses). A real database is
/// needed because POST-3 and PATCH-5 test the unique index and concurrent updates.
/// </summary>
public class ProductsApiFactory : WebApplicationFactory<Program>
{
    private const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=ProductsPracticeTests;Trusted_Connection=True;TrustServerCertificate=True;";

    private static readonly object CreateLock = new();
    private static bool _databaseCreated;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Not "Development", so Program.cs skips its EnsureDeleted/EnsureCreated block.
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
    }

    /// <summary>
    /// Puts the database back to the three seed products, with the next new id being 4.
    /// Runs before every test. The database itself is re-created once per test run, so
    /// changes you make to the model (new columns, concurrency token) are picked up.
    /// If you add a required column to Product, add it to the INSERT below.
    /// </summary>
    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        lock (CreateLock)
        {
            if (!_databaseCreated)
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                _databaseCreated = true;
            }
        }

        db.Database.ExecuteSqlRaw(@"
            TRUNCATE TABLE [Products].[Products];
            SET IDENTITY_INSERT [Products].[Products] ON;
            INSERT INTO [Products].[Products] (Id, Name, Sku, Category, Price, Stock, IsDeleted, Version) VALUES
                (1, 'Laptop', 'LAP-001', 'Electronics', 1200, 10,  0, 1),
                (2, 'Mouse',  'MOU-001', 'Electronics', 25,   100, 0, 1),
                (3, 'Desk',   'DSK-001', 'Furniture',   300,  5,   0, 1);
            SET IDENTITY_INSERT [Products].[Products] OFF;");
    }

    /// <summary>Reads a row straight from the database (soft-deleted rows included). Null if it is gone.</summary>
    public Product? FindProduct(int id)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return db.Products.IgnoreQueryFilters().AsNoTracking().SingleOrDefault(p => p.Id == id);
    }

    /// <summary>Marks a product as soft-deleted directly in the database.</summary>
    public void MarkDeleted(int id)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.ExecuteSqlRaw("UPDATE [Products].[Products] SET IsDeleted = 1 WHERE Id = {0}", id);
    }
}
