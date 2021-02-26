using Catalog.Domain;
using Catalog.Persistence.Database;
using Catalog.Services.EventHandlers.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Catalog.Common.Enums;

namespace Catalog.Services.EventHandlers
{
    public class ProductInStockUpdateStockEventHandler 
        : INotificationHandler<ProductInStockUpdateCommnand>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<ProductInStockUpdateStockEventHandler> _logger;

        public ProductInStockUpdateStockEventHandler(
             ApplicationDbContext applicationDbContext,
             ILogger<ProductInStockUpdateStockEventHandler> logger
            )
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task Handle(ProductInStockUpdateCommnand notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("--- ProductInStockUpdateStockCommand started");

            var products = notification.Items.Select(x => x.ProductId);
            var stocks = await _applicationDbContext.Stocks.Where(x => products.Contains(x.ProductId)).ToListAsync();

            _logger.LogInformation("--- Retrieve products from database");

            foreach (var item in notification.Items)
            {
                var entry = stocks.SingleOrDefault(x => x.ProductId == item.ProductId);

                if (item.Action == ProducInStockAction.Substract)
                {
                    if (entry == null || item.Stock > entry.Stock)
                    {
                        _logger.LogError($"--- Product {entry.ProductId} -doens't have enough stock");
                        throw new System.Exception($"Product {entry.ProductId} - doens't have enough stock");
                    }

                    entry.Stock -= item.Stock;

                    _logger.LogInformation($"--- Product {entry.ProductId} - its stock was subtracted and its new stock is {entry.Stock}");
                }
                else
                {
                    if (entry == null)
                    {
                        entry = new ProductInStock
                        {
                            ProductId = item.ProductId
                        };

                        _logger.LogInformation($"--- New stock record was created for {entry.ProductId} because didn't exists before");

                        await _applicationDbContext.AddAsync(entry);
                    }

                    _logger.LogInformation($"--- Add stock to product {entry.ProductId}");
                    entry.Stock += item.Stock;
                }
            }

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
