using MediatR;
using System;

namespace Catalog.Services.EventHandlers
{
    public class ProductCreateCommand : INotification
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

    }
}
