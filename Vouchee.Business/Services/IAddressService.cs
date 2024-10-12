using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IAddressService
    {
        // CREATE
        public Task<Guid?> CreateAddressAsync(CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetAddressDTO> GetAddressByIdAsync(Guid id);
        public Task<IList<GetAddressDTO>> GetAddressesAsync();

        // UPDATE
        public Task<bool> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO);

        // DELETE
        public Task<bool> DeleteAddressAsync(Guid id);
    }
}
