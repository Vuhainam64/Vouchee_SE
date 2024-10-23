using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface ICartService
    {
        // CREATE
        public Task<bool> AddItemAsync(Guid modalId, ThisUserObj thisUserObj);

        // READ
        public Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj);

        // UPDATE
        public Task<bool> IncreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj);
        public Task<bool> DecreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj);
        public Task<bool> UpdateQuantityAsync(Guid modalId, int quantity, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> RemoveItemAsync(Guid modalId, ThisUserObj thisUserObj);
    }
}
