using Feedback.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feedback.Api.Tests;

/// <summary>
/// Hosts Feedback.Api in memory and swaps SQL Server for the EF Core in-memory provider.
/// Each factory instance gets its own empty database, so ids start at 1 in every test.
/// </summary>
public class FeedbackApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"FeedbackTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Not "Development", so Program.cs skips the EnsureDeleted/EnsureCreated block.
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the SQL Server registration (EF Core 9 keeps the provider setup in
            // IDbContextOptionsConfiguration<T>, so both must go).
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }

    /// <summary>Runs a query against the same database the API is using.</summary>
    public T WithDb<T>(Func<AppDbContext, T> query)
    {
        using var scope = Services.CreateScope();
        return query(scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }
}
