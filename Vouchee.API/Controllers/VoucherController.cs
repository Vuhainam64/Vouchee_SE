using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
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
                                                            [FromQuery] List<Guid>? categoryIDs)
        {
            var result = await _voucherService.GetVoucherAsync(pagingRequest, voucherFiler, categoryIDs);
            return Ok(result);
        }

        [HttpGet("get_newest_vouchers")]
        public async Task<IActionResult> GetNewestVouchers([FromQuery] int numberOfVoucher)
        {
            var result = await _voucherService.GetNewestVouchers(numberOfVoucher);
            return Ok(result);
        }

        [HttpGet("get_best_sold_vouchers")]
        public async Task<IActionResult> GetBestSoldVouchers([FromQuery] int numberOfVoucher)
        {
            var result = await _voucherService.GetTopSaleVouchers(numberOfVoucher);
            return Ok(result);
        }

        [HttpGet("get_nearest_vouchers")]
        public async Task<IActionResult> GetNearestVouchers([FromQuery] PagingRequest pagingRequest,
                                                                [FromQuery] DistanceFilter distanceFilter,
                                                                [FromQuery] VoucherFilter voucherFilter,
                                                                [FromQuery] IList<Guid>? categoryIds)
        {
            var result = await _voucherService.GetNearestVouchersAsync(pagingRequest, distanceFilter, voucherFilter, categoryIds);
            return Ok(result);
        }

        [HttpGet("get_salest_vouchers")]
        public async Task<IActionResult> GetSalestVouchers([FromQuery] int numberOfVoucher)
        {
            var result = await _voucherService.GetSalestVouchers(numberOfVoucher);
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_voucher/{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id, [FromQuery] PagingRequest pagingRequest)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id, pagingRequest);
            return Ok(voucher);
        }

        [Authorize]
        [HttpGet("get_seller_vouchers")]
        public async Task<IActionResult> GetVoucherById([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherFilter voucherFilter,
                                                            [FromQuery] IList<Guid>? categoryIds)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var voucher = await _voucherService.GetVoucherBySellerId(currentUser.userId, pagingRequest, voucherFilter, categoryIds);
            return Ok(voucher);
        }

        // UPDATE
        [HttpPut("update_voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherDTO voucherDTO)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, voucherDTO);
            return Ok(result);
        }

        [HttpPut("update_voucher_status/{id}")]
        public async Task<IActionResult> UpdateVoucherStatus(Guid id)
        {
            var result = await _voucherService.UpdateVoucherStatusAsync(id);
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
