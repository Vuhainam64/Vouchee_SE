﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Business.Services;
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
        private readonly IRoleService _roleService;
        private readonly IVoucherCodeService _voucherCodeSerivce;

        public OrderController(IOrderService orderService,
                                IUserService userService,
                                IRoleService roleService,
                                IVoucherCodeService voucherCodeSerivce)
        {
            _orderService = orderService;
            _userService = userService;
            _roleService = roleService;
            _voucherCodeSerivce = voucherCodeSerivce;
        }

        // CREATE
        [HttpPost("create_order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (RoleHelper.IsBuyer(currentUser))
            {
                var result = await _orderService.CreateOrderAsync(createOrderDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_order")]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _orderService.GetOrdersAsync();
            return Ok(result);
        }

        [HttpGet("get_order/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        // UPDATE
        [HttpPut("update_order/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderDTO updateOrderDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (RoleHelper.IsSeller(currentUser)
                    || RoleHelper.IsBuyer(currentUser))
            {
                var result = await _orderService.UpdateOrderAsync(id, updateOrderDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng hoặc người mua mới có thể thực hiện chức năng này"
            });
        }

        // Assign Voucher Code
        [HttpPut("voucher/assign_voucher_code_to_order")]
        [Authorize]
        public async Task<IActionResult> AssignCode(Guid orderDetailId, [FromBody] VoucherCodeList voucherCodeList)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (RoleHelper.IsSeller(currentUser))
            {
                var result = await _orderService.AssignCodeToOrderAsync(orderDetailId, voucherCodeList);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete("delete_order/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            return Ok(result);
        }
    }
}
