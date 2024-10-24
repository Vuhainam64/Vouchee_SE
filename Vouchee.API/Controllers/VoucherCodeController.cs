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
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    // Gọi sau voucher controller sau khi đã gọi xong trả id về đây để tạo ra voucher code 
    [ApiController]
    [Route("api/v1/voucherCode")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class VoucherCodeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IVoucherCodeService _voucherCodeService;
        private readonly IVoucherService _voucherService;

        public VoucherCodeController(IUserService userService,
                                        IRoleService roleService,
                                        IVoucherCodeService voucherCodeService,
                                        IVoucherService voucherService)
        {
            _userService = userService;
            _roleService = roleService;
            _voucherCodeService = voucherCodeService;
            _voucherService = voucherService;
        }

        // CREATE
        [HttpPost("create_voucher_code")]
        [Authorize]
        public async Task<IActionResult> CreateVoucherCode(Guid modalId, [FromBody] IList<CreateVoucherCodeDTO> createVoucherCodeDTOs)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            List<Guid> voucherCodeIds = [];
            //SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                foreach (var voucherCode in createVoucherCodeDTOs)
                {
                    var result = await _voucherCodeService.CreateVoucherCodeAsync(modalId, voucherCode, currentUser);
                    if (result != Guid.Empty)
                    {
                        voucherCodeIds.Add((Guid)result);
                    }
                }
                return Ok(voucherCodeIds);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_voucher_code")]
        public async Task<IActionResult> GetVoucherCodes()
        {
            var result = await _voucherCodeService.GetVoucherCodesAsync();
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_voucher_code/{id}")]
        public async Task<IActionResult> GetVoucherCodeById(Guid id)
        {
            var voucherCode = await _voucherCodeService.GetVoucherCodeByIdAsync(id);
            return Ok(voucherCode);
        }

        // UPDATE
        [HttpPut("update_voucher_code/{id}")]
        public async Task<IActionResult> UpdateVoucherCode(Guid id, [FromBody] UpdateVoucherCodeDTO updateVoucherCodeDTO)
        {
            var result = await _voucherCodeService.UpdateVoucherCodeAsync(id, updateVoucherCodeDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_voucher_code/{id}")]
        public async Task<IActionResult> DeleteVoucherCode(Guid id)
        {
            var result = await _voucherCodeService.DeleteVoucherCodeAsync(id);
            return Ok(result);
        }
    }
}
