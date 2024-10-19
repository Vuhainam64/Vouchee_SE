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
        [HttpPost("add_item/{voucherId}")]
        [Authorize]
        public async Task<IActionResult> AddItem(Guid voucherId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.buyerRoleId) )
            {
                var result = await _cartService.AddItemAsync(voucherId, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua hàng mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_item")]
        [Authorize]
        public async Task<IActionResult> GetAllItemFromCart([FromQuery] PagingRequest pagingRequest,
                                                                [FromQuery] CartFilter cartFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.buyerRoleId))
            {
                var result = await _cartService.GetCartsAsync(pagingRequest, cartFilter, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua hàng mới có thể thực hiện chức năng này"
            });
        }

        // UPDATE
        [HttpPut("increase_quantity/{voucherId}")]
        [Authorize]
        public async Task<IActionResult> IncreaseQuantity(Guid voucherId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.buyerRoleId))
            {
                var result = await _cartService.IncreaseQuantityAsync(voucherId, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua hàng mới có thể thực hiện chức năng này"
            });
        }

        [HttpPut("decrease_quantity/{voucherId}")]
        public async Task<IActionResult> DecreaseQuantity(Guid voucherId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.buyerRoleId))
            {
                var result = await _cartService.DecreaseQuantityAsync(voucherId, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua hàng mới có thể thực hiện chức năng này"
            });
        }

        [HttpPut("update_quantity/{voucherId}")]
        public async Task<IActionResult> UpdateQuantity(Guid voucherId, int quantity)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _cartService.UpdateQuantityAsync(voucherId, quantity, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua hàng mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete("remove_item/{voucherId}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem(Guid voucherId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _cartService.RemoveItemAsync(voucherId, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua hàng mới có thể thực hiện chức năng này"
            });
        }
    }
}
