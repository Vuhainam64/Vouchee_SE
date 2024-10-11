using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/shop")]
    [EnableCors]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public ShopController(IShopService shopService, 
                                IUserService userService, 
                                IRoleService roleService)
        {
            _shopService = shopService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_new_shop")]
        public async Task<IActionResult> CreateShop([FromForm] CreateShopDTO createShopDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _shopService.CreateShopAsync(createShopDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_shop")]
        public async Task<IActionResult> GetShops()
        {
            var result = await _shopService.GetShopsAsync();
            return Ok(result);
        }

        [HttpGet("get_shop/{id}")]
        public async Task<IActionResult> GetShopById(Guid id)
        {
            var shop = await _shopService.GetShopByIdAsync(id);
            return Ok(shop);
        }

        // UPDATE
        [HttpPut("update_address/{id}")]
        public async Task<IActionResult> UpdateShop(Guid id, [FromBody] UpdateShopDTO updateShopDTO)
        {
            var result = await _shopService.UpdateShopAsync(id, updateShopDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_shop/{id}")]
        public async Task<IActionResult> DeleteShop(Guid id)
        {
            var result = await _shopService.DeleteShopAsync(id);
            return Ok(result);
        }
    }
}

