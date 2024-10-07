﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    // Lúc tạo ra sẽ tạo voucher và voucher code bình thường
    // Gọi voucher controller trước rồi loop cái voucher code controller
    [ApiController]
    [Route("api/voucher")]
    [EnableCors]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public VoucherController(IVoucherService voucherService, 
                                    IUserService userService,
                                    IRoleService roleService)
        {
            _voucherService = voucherService;
            _roleService = roleService;
            _userService = userService;
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVoucher([FromForm] CreateVoucherDTO voucherDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            // SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                var result = await _voucherService.CreateVoucherAsync(voucherDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllVouchers([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherFiler voucherFiler,
                                                            [FromQuery] SortVoucherEnum sortVoucherEnum)
        {
            var result = await _voucherService.GetVouchersAsync(pagingRequest, voucherFiler, sortVoucherEnum);
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id);
            return Ok(voucher);
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherDTO voucherDTO)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, voucherDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var result = await _voucherService.DeleteVoucherAsync(id);
            return Ok(result);
        }
    }
}