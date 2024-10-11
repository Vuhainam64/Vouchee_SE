using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IAddressService
    {
        // CREATE
        public Task<Guid?> CreateAddressAsync(Guid shopId,CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetAddressDTO> GetAddressByIdAsync(Guid id);
        public Task<IList<GetAddressDTO>> GetAddresssAsync();

        // UPDATE
        public Task<bool> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeleteAddressAsync(Guid id);
    }
}
