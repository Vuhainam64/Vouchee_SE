﻿using System;
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
        public Task<CartDTO> AddItemAsync(Guid modalId, ThisUserObj thisUserOb, int quantity = 1);

        // READ
        public Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj, bool isTracking = false);

        // UPDATE
        public Task<CartDTO> IncreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj);
        public Task<CartDTO> DecreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj);
        public Task<CartDTO> UpdateQuantityAsync(Guid modalId, int quantity, ThisUserObj thisUserObj);

        // DELETE
        public Task<CartDTO> RemoveItemAsync(Guid modalId, ThisUserObj thisUserObj);
    }
}
