using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IShopService
    {
        // CREATE
        public Task<Guid?> CreateShopAsync(CreateShopDTO createShopDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetShopDTO> GetShopByIdAsync(Guid id);
        public Task<IList<GetShopDTO>> GetShopsAsync();

        // UPDATE
        public Task<bool> UpdateShopAsync(Guid id, UpdateShopDTO updateShopDTO);

        // DELETE
        public Task<bool> DeleteShopAsync(Guid id);
    }
}
