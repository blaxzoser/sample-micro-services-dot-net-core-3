using Catalog.Domain;
using Catalog.Persistence.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Persistence.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options) : base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Catalog");

            ModelConfig(modelBuilder);
        }

        protected void ModelConfig(ModelBuilder modelBuilder)
        {
            new ProductConfiguration(modelBuilder.Entity<Product>());
            new ProductInStockConfiguration(modelBuilder.Entity<ProductInStock>());
        }


        public DbSet<Product> Products { get; set; }

        public DbSet<ProductInStock> Stocks { get; set; }
    }
}
