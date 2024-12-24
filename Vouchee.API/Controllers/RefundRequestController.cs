using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/refundRequest")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class RefundRequestController : ControllerBase
    {
        private readonly IRefundRequestService _refundRequestService;
        private readonly IUserService _userService;

        public RefundRequestController(IRefundRequestService refundRequestService,
                                       IUserService userService)
        {
            _refundRequestService = refundRequestService;
            _userService = userService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_refund_request")]
        public async Task<IActionResult> CreateRefundRequest([FromBody] CreateRefundRequestDTO refundRequestDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _refundRequestService.CreateRefundRequestAsync(refundRequestDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_refund_request")]
        public async Task<IActionResult> GetAllRefundRequest([FromQuery] PagingRequest pagingRequest, [FromQuery] RefundRequestFilter refundRequestFilter)
        {
            var result = await _refundRequestService.GetAllRefundRequestAsync(pagingRequest, refundRequestFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_supplier_refund_request")]
        public async Task<IActionResult> GetSupplierRefundRequest([FromQuery] PagingRequest pagingRequest, [FromQuery] RefundRequestFilter refundRequestFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _refundRequestService.GetSupplierRefundRequestAsync(currentUser, pagingRequest, refundRequestFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_buyer_refund_request")]
        public async Task<IActionResult> GetBuyerRefundRequest([FromQuery] PagingRequest pagingRequest, [FromQuery] RefundRequestFilter refundRequestFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _refundRequestService.GetBuyerRefundRequestAsync(currentUser, pagingRequest, refundRequestFilter);
            return Ok(result);
        }

        [HttpGet("get_refund_request/{id}")]
        public async Task<IActionResult> GetSupplierRefundRequest(Guid id)
        {
            var result = await _refundRequestService.GetRefundRequestByIdAsync(id);
            return Ok(result);
        }

        // UPDATE
        [HttpPut("update_refund_request/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRefundRequest(Guid id, [FromBody] UpdateRefundRequestDTO updateRefundRequestDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _refundRequestService.UpdateRefundRequestAsync(id, updateRefundRequestDTO, currentUser);
            return Ok(result);
        }

        [HttpPut("update_refund_request_status/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRefundRequestStatus(Guid id, [FromQuery] string? reason, [FromQuery] RefundRequestStatusEnum status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _refundRequestService.UpdateRefundRequestStatusAsync(id, status, reason, currentUser);
            return Ok(result);
        }

        // DELETE
        [Authorize]
        [HttpDelete("delete_refund_request/{id}")]
        public async Task<IActionResult> DeleteRefundRequest(Guid id)
        {
            var result = await _refundRequestService.DeleteRefundRequestAsync(id);
            return Ok(result);
        }
    }
}
