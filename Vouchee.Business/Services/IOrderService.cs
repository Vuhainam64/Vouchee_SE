﻿using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IOrderService
    {
        public Task<ResponseMessage<string>> CreateOrderAsync(ThisUserObj thisUserObj, CheckOutViewModel checkOutViewModel);

        // READ
        public Task<GetDetailOrderDTO> GetOrderByIdAsync(string id);
        public Task<DynamicResponseModel<GetOrderDTO>> GetOrdersAsync(PagingRequest pagingRequest, OrderFilter orderFilter, ThisUserObj? thisUserObj);
        public Task<DynamicResponseModel<GetOrderDTO>> GetSellerOrderAsync(PagingRequest pagingRequest, OrderFilter orderFilter, ThisUserObj thisUserObj);
        public Task<GetDetailSellerOrderDTO> GetDetailSellerOrderAsync(string orderId, ThisUserObj thisUserObj);
    }
}
