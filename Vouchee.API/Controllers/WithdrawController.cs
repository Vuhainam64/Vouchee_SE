using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/withdraw")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class WithdrawController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWithdrawService _withdrawService;
        private readonly IExcelExportService _excelExportService;

        public WithdrawController(IUserService userService,
                                  IWithdrawService withdrawService,
                                  IExcelExportService excelExportService)
        {
            _userService = userService;
            _withdrawService = withdrawService;
            _excelExportService = excelExportService;
        }

        [HttpPost("create_withdraw_request")]
        [Authorize]
        public async Task<IActionResult> CreateSellerWithdrawRequest([FromQuery] WalletTypeEnum walletType, [FromBody] CreateWithdrawRequestDTO createWithdrawRequestDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _withdrawService.CreateWithdrawRequestAsync(walletType, createWithdrawRequestDTO, currentUser);
            return Ok(result);
        }

        [HttpGet("get_withdraw_request_by_id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetWithdrawRequestbyId(string id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _withdrawService.GetWithdrawRequestById(id);
            return Ok(result);
        }

        [HttpGet("export_withdraw")]
        [Authorize]
        public async Task<IActionResult> ExportWithdraw()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _excelExportService.GenerateWithdrawRequestExcel();

            var fileName = $"WITHDRAW.xlsx";

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(result, contentType, fileName);
        }

        [Authorize]
        [HttpGet("get_withdraw_requests")]
        public async Task<IActionResult> GetWithdrawRequests([FromQuery] PagingRequest pagingRequest, [FromQuery] WithdrawRequestFilter withdrawRequestFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _withdrawService.GetWithdrawRequestAsync(pagingRequest, withdrawRequestFilter, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_withdraw_transactions")]
        public async Task<IActionResult> GetWithdrawTransasction([FromQuery] PagingRequest pagingRequest, [FromQuery] WalletTransactionFilter walletTransactionFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _withdrawService.GetWithdrawWalletTransactionAsync(pagingRequest, walletTransactionFilter, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("update_withdraw_request_status/{id}")]
        public async Task<IActionResult> UpdateWithdrawRequestStatus(Guid id, WithdrawRequestStatusEnum withdrawRequestStatusEnum)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _withdrawService.UpdateWithdrawRequest(id, withdrawRequestStatusEnum, currentUser);
            return Ok(result);
        }
    }
}
