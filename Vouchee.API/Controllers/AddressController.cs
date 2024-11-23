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
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/address")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AddressController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IAddressService _addresService;
        private readonly IUserService _userService;

        public AddressController(IBrandService brandService,
                                    IAddressService addressService, 
                                    IUserService userService)
        {
            _brandService = brandService;
            _addresService = addressService;
            _userService = userService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_address")]
        public async Task<IActionResult> CreateAddress(Guid brandId, [FromBody] CreateAddressDTO createAddressDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _addresService.CreateAddressAsync(brandId, createAddressDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_address")]
        public async Task<IActionResult> GetAddresses([FromQuery] PagingRequest pagingRequest,
                                                        [FromQuery] AddressFilter addressFilter)
        {
            var result = await _addresService.GetAddressesAsync(pagingRequest, addressFilter);
            return Ok(result);
        }

        [HttpGet("get_address/{id}")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            var address = await _addresService.GetAddressByIdAsync(id);
            return Ok(address);
        }

        // UPDATE
        [Authorize]
        [HttpPut("update_address/{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressDTO updateAddressDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _addresService.UpdateAddressAsync(id, updateAddressDTO, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update_address_state/{id}")]
        public async Task<IActionResult> UpdateAddressState(Guid id, bool isActive)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _addresService.UpdateAddressStateAsync(id, isActive, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update_address_status/{id}")]
        public async Task<IActionResult> UpdateAddressStatus(Guid id, ObjectStatusEnum status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _addresService.UpdateAddressStatusAsync(id, status, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("verify_address/{id}")]
        public async Task<IActionResult> VerifyAddress(Guid id, bool isVerify)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
            {
                var result = await _addresService.VerifyAddressAsync(id, isVerify, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [Authorize]
        [HttpDelete("delete_address/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _addresService.DeleteAddressAsync(id);
            return Ok(result);
        }
    }
}

