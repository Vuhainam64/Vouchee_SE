using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;

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
    }
}
