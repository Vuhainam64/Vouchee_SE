using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;

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

        public CartController(ICartService cartService, IUserService userService, IVoucherService voucherService, IRoleService roleService)
        {
            _cartService = cartService;
            _userService = userService;
            _voucherService = voucherService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost("create_new_brand")]
        [Authorize]
        public async Task<IActionResult> CreateBrand([FromForm] CreateCartDTO createCartDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _cartService.CreateCartAsync(createCartDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_brand")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _cartService.GetCartsAsync();
            return Ok(result);
        }

        // UPDATE
        [HttpPut("update_brand/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromBody] UpdateCartDTO updateBrandDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _cartService.UpdatCartAsync(id, updateBrandDTO);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete("delete_brand/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _cartService.DeleteCartAsync(id);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }
    }
}
