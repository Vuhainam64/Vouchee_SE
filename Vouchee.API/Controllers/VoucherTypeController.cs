﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/voucherType")]
    [EnableCors]
    public class VoucherTypeController : ControllerBase
    {
        private readonly IVoucherTypeService _voucherTypeService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public VoucherTypeController(IVoucherTypeService voucherTypeService, 
                                        IUserService userService, 
                                        IRoleService roleService)
        {
            _voucherTypeService = voucherTypeService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateVoucherType([FromBody] CreateVoucherTypeDTO createVoucherTypeDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _voucherTypeService.CreateVoucherTypeAsync(createVoucherTypeDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetVoucherTypes([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherTypeFilter voucherTypeFilter,
                                                            [FromQuery] SortVoucherTypeEnum sortVoucherTypeEnum)
        {
            var result = await _voucherTypeService.GetVoucherTypesAsync(pagingRequest, voucherTypeFilter, sortVoucherTypeEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoucherTypeById(Guid id)
        {
            var voucherType = await _voucherTypeService.GetVoucherTypeByIdAsync(id);
            return Ok(voucherType);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateVoucherType(Guid id, [FromBody] UpdateVoucherTypeDTO updateVoucherTypeDTO)
        {
            var result = await _voucherTypeService.UpdateVoucherTypeAsync(id, updateVoucherTypeDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoucherType(Guid id)
        {
            var result = await _voucherTypeService.DeleteVoucherTypeAsync(id);
            return Ok(result);
        }
    }
}

