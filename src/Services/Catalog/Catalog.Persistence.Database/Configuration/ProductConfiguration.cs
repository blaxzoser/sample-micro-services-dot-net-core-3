using Catalog.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Persistence.Database.Configuration
{
    public class ProductConfiguration
    {
        public ProductConfiguration(EntityTypeBuilder<Product> entityBuilder)
        {
            entityBuilder.HasKey(x => x.ProductId);
            entityBuilder.Property(x => x.ProductId).IsRequired();

            entityBuilder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entityBuilder.Property(x => x.Description).IsRequired().HasMaxLength(500);

            var random = new Random();
            var products = new List<Product>();

            for (var i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    ProductId = i,
                    Name = $"Product {i}",
                    Description = $"Description for product {i}",
                    Price = random.Next(100, 1000)
                });
            }

            entityBuilder.HasData(products);

        }

        public void  SetupProducts(EntityTypeBuilder<Product> entityBuilder)
        {
            //Setup Products by default
            var products = new List<Product>();
            var random = new Random();

            for(int i = 0; i <= 100; i++)
            {
                var newProduct = new Product
                {
                    ProductId = i,

                    Description = $"Description for product {i}",
                    Price = random.Next(100, 1000),
                };
                products.Add(newProduct);
            }

            entityBuilder.HasData(products);
        }

    }
}
