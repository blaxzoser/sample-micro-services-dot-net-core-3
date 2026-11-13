namespace Customer.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Customer");

        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.Sku).HasMaxLength(20).IsRequired();
            e.Property(p => p.Category).HasMaxLength(50).IsRequired();
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.HasIndex(p => p.Sku).IsUnique();
        });

        modelBuilder.Entity<CustomerEntity>(e =>
        {
            e.ToTable("Customers");
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.Property(c => c.Balance).HasPrecision(18, 2);
            e.Property(c => c.Birthday).HasColumnType("date");
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.Total).HasPrecision(18, 2);
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            e.HasOne<CustomerEntity>().WithMany().HasForeignKey(o => o.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);   // DELETE-3 checks for orders itself
        });

        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Name).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(30);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.Property(p => p.Amount).HasPrecision(18, 2);
            e.HasOne<CustomerEntity>().WithMany().HasForeignKey(p => p.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Setting>(e =>
        {
            e.HasKey(s => s.Key);
            e.Property(s => s.Key).HasMaxLength(50);
        });

        modelBuilder.Entity<CartItem>(e =>
        {
            e.HasKey(c => new { c.CartId, c.ProductId });
            e.HasOne<Product>().WithMany().HasForeignKey(c => c.ProductId);
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

        modelBuilder.Entity<CustomerEntity>().HasData(
            new CustomerEntity { Id = 1, Name = "Ana",  Balance = 500m, Birthday = new DateTime(1987, 1, 13) },  // 01/13/1987
            new CustomerEntity { Id = 2, Name = "Luis", Balance = 20m,  Birthday = new DateTime(2000, 6, 15) },  // 30 or under, until 06/15/2031
            new CustomerEntity { Id = 3, Name = "Eva",  Balance = 0m,   Birthday = new DateTime(1980, 3, 2) });  // has no orders and no payments

        // Payments for GET-6. Ana has 3, Luis has 1, Eva has none.
        // Note: POST-5's test cases expect the first new payment to get id 1. Here it gets id 5.
        var paidAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Payment>().HasData(
            new Payment { Id = 1, CustomerId = 1, Amount = 100m, CreatedAt = paidAt },
            new Payment { Id = 2, CustomerId = 1, Amount = 50m,  CreatedAt = paidAt },
            new Payment { Id = 3, CustomerId = 1, Amount = 70m,  CreatedAt = paidAt },
            new Payment { Id = 4, CustomerId = 2, Amount = 100m, CreatedAt = paidAt });

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, CustomerId = 1, Total = 100m, Status = OrderStatus.Pending },
            new Order { Id = 2, CustomerId = 2, Total = 100m, Status = OrderStatus.Pending },
            new Order { Id = 3, CustomerId = 1, Total = 50m,  Status = OrderStatus.Paid },
            new Order { Id = 4, CustomerId = 1, Total = 70m,  Status = OrderStatus.Delivered });

        // HasData needs fixed values, so CreatedAt can't use DateTime.UtcNow here.
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Ana Torres", Email = "ana@mail.com", CreatedAt = seededAt },
            new User { Id = 2, Name = "Luis Perez", Email = "luis@mail.com", Phone = "+51 999 111 222", CreatedAt = seededAt });

        modelBuilder.Entity<Setting>().HasData(new Setting { Key = "theme", Value = "dark" });

        modelBuilder.Entity<CartItem>().HasData(
            new CartItem { CartId = 1, ProductId = 1 },
            new CartItem { CartId = 1, ProductId = 2 });
    }
}
