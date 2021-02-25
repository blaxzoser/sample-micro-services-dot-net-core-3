using Catalog.Persistence.Database;
using Catalog.Service.Queries.DTOs;
using Microsoft.EntityFrameworkCore;
using Service.Common.Paging;
using Services.Common.Collection;
using Services.Common.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Service.Queries
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ProductQueryService(ApplicationDbContext context)
        {
            _applicationDbContext = context;
        }

      public async Task<DataCollection<ProductDto>> GetAllAsyc(int page,int take, IEnumerable<int> products = null)
      {
            var collection = await _applicationDbContext.Products
                                                        .Where(source => products == null || products.Contains(source.ProductId))
                                                        .OrderByDescending(x => x.ProductId)
                                                        .GetPagedAsync(page, take);

            return collection.MapTo<DataCollection<ProductDto>>();

      }

      public async Task<ProductDto> GetAsyc(int id)
      {
            return (await _applicationDbContext.Products.SingleAsync(source => source.ProductId == id)).MapTo<ProductDto>();
       }
    }
}
