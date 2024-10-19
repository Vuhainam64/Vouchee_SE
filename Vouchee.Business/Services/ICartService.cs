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
        public Task<bool> AddItemAsync(Guid voucherId, ThisUserObj thisUserObj);

        // READ
        public Task<DynamicResponseModel<CartDTO>> GetCartsAsync(PagingRequest pagingRequest, CartFilter cartFilter, ThisUserObj thisUserObj);

        // UPDATE
        public Task<bool> IncreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj);
        public Task<bool> DecreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj);
        public Task<bool> UpdateQuantityAsync(Guid voucherId, int quantity, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> RemoveItemAsync(Guid voucherId, ThisUserObj thisUserObj);
    }
}
