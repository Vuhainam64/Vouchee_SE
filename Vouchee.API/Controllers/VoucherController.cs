using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
    [Route("api/v1/voucher")]
    [EnableCors("MyAllowSpecificOrigins")]
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

        // CREATE
        [HttpPost("create_voucher")]
        [Authorize]
        public async Task<dynamic> CreateVoucher([FromBody] CreateVoucherDTO voucherDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _voucherService.CreateVoucherAsync(voucherDTO, currentUser);
            return result;
        }

        // GET ALL
        [HttpGet("get_all_voucher")]
        public async Task<IActionResult> GetAllVouchers([FromQuery] PagingRequest pagingRequest, 
                                                            [FromQuery] VoucherFilter voucherFiler, 
                                                            [Required] decimal lon, 
                                                            [Required] decimal lat, 
                                                            [FromQuery] List<Guid>? categoryIDs)
        {
            var result = await _voucherService.GetVouchersAsync(pagingRequest, voucherFiler, lon, lat, categoryIDs);
            return Ok(result);
        }

        [HttpGet("get_newest_vouchers")]
        public async Task<IActionResult> GetNewestVouchers()
        {
            var result = await _voucherService.GetNewestVouchers();
            return Ok(result);
        }

        [HttpGet("get_best_sold_vouchers")]
        public async Task<IActionResult> GetBestSoldVouchers()
        {
            var result = await _voucherService.GetTopSaleVouchers();
            return Ok(result);
        }

        [HttpGet("get_nearest_vouchers")]
        public async Task<IActionResult> GetNearestVouchers([FromQuery] decimal lon,
                                                                [FromQuery] decimal lat)
        {
            var result = await _voucherService.GetNearestVouchers(lon, lat);
            return Ok(result);
        }

        [HttpGet("get_salest_vouchers")]
        public async Task<IActionResult> GetSalestVouchers()
        {
            var result = await _voucherService.GetSalestVouchers();
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_voucher/{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id);
            return Ok(voucher);
        }

        [HttpGet("get_voucher_by_seller_id/{sellerId}")]
        public async Task<IActionResult> GetVoucherById(Guid sellerId, 
                                                            [FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherFilter voucherFilter)
        {
            var voucher = await _voucherService.GetVoucherBySellerId(sellerId, pagingRequest, voucherFilter);
            return Ok(voucher);
        }

        // UPDATE
        [HttpPut("update_voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherDTO voucherDTO)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, voucherDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_voucher/{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var result = await _voucherService.DeleteVoucherAsync(id);
            return Ok(result);
        }
    }
}
