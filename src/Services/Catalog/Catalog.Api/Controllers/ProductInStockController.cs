using Catalog.Service.Queries;
using Catalog.Service.Queries.DTOs;
using Catalog.Services.EventHandlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Common.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("stocks")]
    public class ProductInStockController : ControllerBase
    {
        private readonly ILogger<ProductInStockController> _logger;
        private readonly IMediator _mediator;
        private readonly IProductInStockQueryService _productInStockQueryService;

        public ProductInStockController(
             ILogger<ProductInStockController> logger,
             IMediator mediator,
             IProductInStockQueryService productInStockQueryService
            ) 
        {
            _logger = logger;
            _mediator = mediator;
            _productInStockQueryService = productInStockQueryService;
        }

        [HttpGet]
        public async Task<DataCollection<ProductInStockDto>> GetAll(int page = 1, int take = 10, string products = null)
        {
            IEnumerable<int> ids = null;

            if (!string.IsNullOrEmpty(products))
            {
                ids = products.Split(',').Select(x => Convert.ToInt32(x));
            }

            return await _productInStockQueryService.GetAllAsync(page, take, ids);
        }

        [HttpPut]
        public async Task<IActionResult> ActionResult(ProductInStockUpdateCommnand command)
        {
            await _mediator.Publish(command);
            return NoContent();
        }

    }
}
