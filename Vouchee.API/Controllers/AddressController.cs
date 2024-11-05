using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Route("api/address")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AddressController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IAddressService _addressRepository;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AddressController(IBrandService brandService,
                                    IAddressService addressService, 
                                    IUserService userService, 
                                    IRoleService roleService)
        {
            _brandService = brandService;
            _addressRepository = addressService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_new_address")]
        public async Task<IActionResult> CreateAddress([FromForm] CreateAddressDTO createAddressDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _addressRepository.CreateAddressAsync(createAddressDTO, currentUser);
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
        public async Task<IActionResult> GetAddresses([FromQuery] PagingRequest pagingRequest,
                                                        [FromQuery] AddressFilter addressFilter)
        {
            var result = await _addressRepository.GetAddressesAsync(pagingRequest, addressFilter);
            return Ok(result);
        }

        [HttpGet("get_address/{id}")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            var address = await _addressRepository.GetAddressByIdAsync(id);
            return Ok(address);
        }

        // UPDATE
        [HttpPut("update_address/{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressDTO updateAddressDTO)
        {
            var result = await _addressRepository.UpdateAddressAsync(id, updateAddressDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_address/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var result = await _addressRepository.DeleteAddressAsync(id);
            return Ok(result);
        }
    }
}

