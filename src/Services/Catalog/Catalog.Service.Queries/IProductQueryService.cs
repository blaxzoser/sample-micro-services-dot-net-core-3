using Catalog.Service.Queries.DTOs;
using Services.Common.Collection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Service.Queries
{
    public interface IProductQueryService
    {
        Task<DataCollection<ProductDto>> GetAllAsyc(int page, int take, IEnumerable<int> products = null);

        Task<ProductDto> GetAsyc(int id);
    }
}
