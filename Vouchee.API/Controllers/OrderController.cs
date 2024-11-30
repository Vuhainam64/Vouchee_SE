using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/order")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IVoucherCodeService _voucherCodeSerivce;

        public OrderController(IOrderService orderService,
                                IUserService userService,
                                IVoucherCodeService voucherCodeSerivce)
        {
            _orderService = orderService;
            _userService = userService;
            _voucherCodeSerivce = voucherCodeSerivce;
        }

        // CREATE
        [HttpPost("create_order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CheckOutViewModel checkOutViewModel)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _orderService.CreateOrderAsync(currentUser, checkOutViewModel);
            return Ok(result);

        }

        // READ
        [HttpGet("get_all_order")]
        public async Task<IActionResult> GetOrders([FromQuery] PagingRequest pagingRequest, [FromQuery] OrderFilter orderFilter)
        {
            var result = await _orderService.GetOrdersAsync(pagingRequest, orderFilter, null);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_buyer_order")]
        public async Task<IActionResult> GetBuyerOrders([FromQuery] PagingRequest pagingRequest, [FromQuery] OrderFilter orderFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _orderService.GetOrdersAsync(pagingRequest, orderFilter, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_seller_order")]
        public async Task<IActionResult> GetSellerOrder([FromQuery] PagingRequest pagingRequest, [FromQuery] OrderDetailFilter orderDetailFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _orderService.GetSellerOrderAsync(pagingRequest, orderDetailFilter, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_seller_order_detail")]
        public async Task<IActionResult> GetSellerOrderDetail(string orderId, Guid modalId, [FromQuery] PagingRequest pagingRequest)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _orderService.GetSellerOrderedVoucherCodeAsync(orderId, modalId, pagingRequest);
            return Ok(result);
        }

        [HttpGet("get_order/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }
    }
}
