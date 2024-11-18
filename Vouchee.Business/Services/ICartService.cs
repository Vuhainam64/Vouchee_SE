using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface ICartService
    {
        // CREATE
        public Task<CartDTO> AddItemAsync(Guid modalId, ThisUserObj thisUserOb, int quantity);

        // READ
        public Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj, bool isTracking = false);
        public Task<DetailCartDTO> GetCheckoutCartsAsync(ThisUserObj thisUserObj, CheckOutViewModel checkOutViewModel);

        // UPDATE
        public Task<CartDTO> IncreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj);
        public Task<CartDTO> DecreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj);
        public Task<CartDTO> UpdateQuantityAsync(Guid modalId, int quantity, ThisUserObj thisUserObj);

        // DELETE
        public Task<CartDTO> RemoveItemAsync(Guid modalId, ThisUserObj thisUserObj);
    }
}
