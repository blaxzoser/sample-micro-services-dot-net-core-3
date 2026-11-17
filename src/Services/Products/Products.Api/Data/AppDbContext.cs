namespace Products.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Products");

        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.Sku).HasMaxLength(20).IsRequired();
            e.Property(p => p.Category).HasMaxLength(50).IsRequired();
            e.Property(p => p.Price).HasPrecision(18, 2);

            // POST-3: the database rejects a second row with the same SKU. SaveChanges then
            // throws DbUpdateException (SqlException number 2601). SQL Server's default
            // collation is case-insensitive, so "lap-001" collides with "LAP-001".
            e.HasIndex(p => p.Sku).IsUnique();

            // Left for you to decide while solving the exercises:
            //   PUT-5 / PATCH-5: e.Property(p => p.Version).IsConcurrencyToken();
            //   DELETE-4:        e.HasQueryFilter(p => !p.IsDeleted);
        });

        Seed(modelBuilder);
    }

    // Same seed data as the starter kit's Db class, so the exercise test cases still apply.
    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Sku = "LAP-001", Category = "Electronics", Price = 1200m, Stock = 10 },
            new Product { Id = 2, Name = "Mouse",  Sku = "MOU-001", Category = "Electronics", Price = 25m,   Stock = 100 },
            new Product { Id = 3, Name = "Desk",   Sku = "DSK-001", Category = "Furniture",   Price = 300m,  Stock = 5 });
    }
}
