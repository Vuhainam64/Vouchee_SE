using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{

    [ApiController]
    [Route("api/v1/cart")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IVoucherService _voucherService;
        private readonly IRoleService _roleService;

        public CartController(ICartService cartService, 
                                IUserService userService, 
                                IVoucherService voucherService, 
                                IRoleService roleService)
        {
            _cartService = cartService;
            _userService = userService;
            _voucherService = voucherService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost("add_item/{modalId}")]
        [Authorize]
        public async Task<IActionResult> AddItem(Guid modalId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _cartService.AddItemAsync(modalId, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_item")]
        [Authorize]
        public async Task<IActionResult> GetAllItemFromCart()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _cartService.GetCartsAsync(currentUser);
            return Ok(result);
        }

        // UPDATE
        [HttpPut("increase_quantity/{modalId}")]
        [Authorize]
        public async Task<IActionResult> IncreaseQuantity(Guid modalId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _cartService.IncreaseQuantityAsync(modalId, currentUser);
            return Ok(result);
        }

        [HttpPut("decrease_quantity/{modalId}")]
        public async Task<IActionResult> DecreaseQuantity(Guid modalId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _cartService.DecreaseQuantityAsync(modalId, currentUser);
            return Ok(result);
        }

        [HttpPut("update_quantity/{modalId}")]
        public async Task<IActionResult> UpdateQuantity(Guid modalId, int quantity)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _cartService.UpdateQuantityAsync(modalId, quantity, currentUser);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("remove_item/{modalId}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem(Guid modalId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _cartService.RemoveItemAsync(modalId, currentUser);
            return Ok(result);
        }
    }
}