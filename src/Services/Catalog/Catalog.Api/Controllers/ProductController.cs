using Catalog.Service.Queries;
using Catalog.Service.Queries.DTOs;
using Catalog.Services.EventHandlers;
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
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductQueryService _productQueryService;
        private readonly IMediator _mediator;
        public ProductController(ILogger<ProductController> logger,
            IProductQueryService productQueryService,
            IMediator mediator
            )
        {
            _logger = logger;
            _productQueryService = productQueryService;
            _mediator = mediator;
        }

        // products
        [HttpGet]
        public async Task<DataCollection<ProductDto>> GetAll(int page = 1, int take = 10, string ids = null)
        {
            IEnumerable<int> products = null;
            if (!string.IsNullOrEmpty(ids))
            {
                products = ids.Split(',').Select(x => Convert.ToInt32(x));
            }

            return await _productQueryService.GetAllAsyc(page, take, products);
        }

        // products/1
        [HttpGet("{id}")]
        public async Task<ProductDto> Get(int id)
        {
            return await _productQueryService.GetAsyc(id);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateCommand command)
        {
            await _mediator.Publish(command);
            return Ok();
        }
    }
}
