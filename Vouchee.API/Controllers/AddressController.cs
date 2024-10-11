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
    [Route("api/v1/address")]
    [EnableCors]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AddressController(IAddressService addressService, 
                                    IUserService userService, 
                                    IRoleService roleService)
        {
            _addressService = addressService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost("create_new_address")]
        [Authorize]
        public async Task<IActionResult> CreateAddress([FromQuery] Guid shopId, [FromForm] CreateAddressDTO createAddressDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _addressService.CreateAddressAsync(shopId, createAddressDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_address")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _addressService.GetAddresssAsync();
            return Ok(result);
        }

        [HttpGet("get_address/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
            {
                return NotFound(new { message = "Addess not found." });
            }
            return Ok(address);
        }

        // UPDATE
        [HttpPut("update_address/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressDTO updateAddressDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _addressService.UpdateAddressAsync(id, updateAddressDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete("delete_address/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDelete(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _addressService.DeleteAddressAsync(id);
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
