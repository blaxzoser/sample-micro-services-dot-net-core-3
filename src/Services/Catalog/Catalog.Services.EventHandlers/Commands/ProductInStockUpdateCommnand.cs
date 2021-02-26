using MediatR;
using System.Collections.Generic;
using static Catalog.Common.Enums;

namespace Catalog.Services.EventHandlers.Commands
{
    public class ProductInStockUpdateCommnand : INotification
    {
        public IEnumerable<ProductInStockUpdateItem> Items { get; set; } = new List<ProductInStockUpdateItem>();

    }

    public class ProductInStockUpdateItem 
    {
        public int ProductId { get; set; }

        public int Stock { get; set; }

        public ProducInStockAction Action { get; set; }

    }

}
