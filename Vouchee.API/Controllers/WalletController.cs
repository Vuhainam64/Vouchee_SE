﻿using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/wallet")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class WalletController : ControllerBase
    {
        private readonly IExcelExportService _excelExportService;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletController(IExcelExportService excelExportService,
                                IUserService userService,
                                IWalletService walletService,
                                IWalletTransactionService walletTransactionService)
        {
            _excelExportService = excelExportService;
            _userService = userService;
            _walletService = walletService;
            _walletTransactionService = walletTransactionService;
        }

        [Authorize]
        [HttpGet("get_seller_wallet")]
        public async Task<IActionResult> GetSellerWallet()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletService.GetSellerWalletAsync(currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_buyer_wallet")]
        public async Task<IActionResult> GetBuyerWallet()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletService.GetBuyerWalletAsync(currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_seller_transactions")]
        public async Task<IActionResult> GetSellerTransactions([FromQuery] PagingRequest pagingRequest,
                                                                [FromQuery] SellerWalletTransactionFilter sellerWalletTransactionFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletTransactionService.GetSellerWalletTransactionsAsync(pagingRequest, sellerWalletTransactionFilter, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_buyer_transactions")]
        public async Task<IActionResult> GetBuyerTransactions([FromQuery] PagingRequest pagingRequest,
                                                                [FromQuery] BuyerWalletTransactionFilter buyerWalletTransactionFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletTransactionService.GetBuyerWalletTransactionsAsync(pagingRequest, buyerWalletTransactionFilter, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("export_seller_transactions")]
        public async Task<IActionResult> ExportSellerTransaction([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _excelExportService.GenerateStatementExcel(currentUser, startDate, endDate);

            var fileName = $"Seller_Wallet_Statement_{currentUser.userId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(result, contentType, fileName);
        }

        [Authorize]
        [HttpGet("get_dashboard_transaction")]
        public async Task<IActionResult> GetDashboardTransactions()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletTransactionService.GetWalletTransactionsAsync(currentUser);
            return Ok(result);
        }
    }
}
