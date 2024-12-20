﻿using Microsoft.AspNetCore.Authorization;
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
using Vouchee.Data.Models.Constants.Enum.Status;
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
        private readonly IMediaService _mediaService;

        public VoucherController(IVoucherService voucherService,
                                 IUserService userService,
                                 IMediaService mediaService)
        {
            _voucherService = voucherService;
            _userService = userService;
            _mediaService = mediaService;
        }

        // CREATE
        [HttpPost("create_voucher")]
        [Authorize]
        public async Task<dynamic> CreateVoucher([FromBody] CreateVoucherDTO voucherDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherService.CreateVoucherAsync(voucherDTO, currentUser);
            return result;
        }

        // GET ALL
        [HttpGet("get_all_voucher")]
        public async Task<IActionResult> GetAllVouchers([FromQuery] PagingRequest pagingRequest, 
                                                            [FromQuery] VoucherFilter voucherFiler,
                                                            [FromQuery] SortVoucherEnum sortVoucherEnum)
        {
            var result = await _voucherService.GetVoucherAsync(pagingRequest, voucherFiler, sortVoucherEnum);
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

        [HttpGet("get_mini_vouchers")]
        public async Task<IActionResult> GetMiniVouchers(string title)
        {
            var result = await _voucherService.GetMiniVoucherAsync(title);
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_voucher/{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id);
            return Ok(voucher);
        }

        [Authorize]
        [HttpGet("get_seller_vouchers")]
        public async Task<IActionResult> GetVoucherById([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherFilter voucherFilter,
                                                            [FromQuery] IList<Guid>? categoryIds)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var voucher = await _voucherService.GetVoucherBySellerId(currentUser.userId, pagingRequest, voucherFilter, categoryIds);
            return Ok(voucher);
        }

        // UPDATE
        [Authorize]
        [HttpPut("update_voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherDTO voucherDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherService.UpdateVoucherAsync(id, voucherDTO, currentUser);
            return Ok(result);
        }

        [HttpPut("update_voucher_status/{id}")]
        public async Task<IActionResult> UpdateVoucherStatus(Guid id, VoucherStatusEnum voucherStatus)
        {
            var result = await _voucherService.UpdateVoucherStatusAsync(id, voucherStatus);
            return Ok(result);
        }

        [HttpPut("update_voucher_isActive/{id}")]
        public async Task<IActionResult> UpdateVoucherisActive(Guid id, bool isActive)
        {
            var result = await _voucherService.UpdateVoucherisActiveAsync(id, isActive);
            return Ok(result);
        }

        //[HttpPut("remove_category_from_voucher")]
        //public async Task<IActionResult> RemoveCategoryFromVoucher(Guid categoryId, Guid voucherId)
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

        //    var result = await _voucherService.RemoveCategoryFromVoucherAsync(categoryId, voucherId, currentUser);
        //    return Ok(result);
        //}

        // DELETE
        [Authorize]
        [HttpDelete("delete_voucher/{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherService.DeleteVoucherAsync(id);
            return Ok(result);
        }
    }
}
