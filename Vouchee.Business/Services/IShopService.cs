using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;

namespace Vouchee.Business.Services
{
    public interface IShopService
    {
        // CREATE
        public Task<Guid?> CreateShopAsync(CreateShopDTO createShopDTO);

        // READ
        public Task<GetShopDTO> GetShopByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetShopDTO>> GetShopsAsync(PagingRequest pagingRequest,
                                                                        ShopFilter shopFilter,
                                                                        SortShopEnum sortShopEnum);

        // UPDATE
        public Task<bool> UpdateShopAsync(Guid id, UpdateShopDTO updateShopDTO);

        // DELETE
        public Task<bool> DeleteShopAsync(Guid id);
    }
}
