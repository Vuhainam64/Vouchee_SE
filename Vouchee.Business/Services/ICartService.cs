using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface ICartService
    {
        public Task<IList<GetCartDTO>> GetCartsAsync();
        public Task<IList<GetCartDTO>> GetCartsbyUIdAsync(Guid id);
        // CREATE
        public Task<Guid?> CreateCartAsync(CreateCartDTO createBrandDTO, ThisUserObj thisUserObj);

        // UPDATE
        public Task<bool> UpdatCartAsync(Guid id, UpdateCartDTO updateBrandDTO);

        // DELETE
        public Task<bool> DeleteCartAsync(Guid id);
    }
}
