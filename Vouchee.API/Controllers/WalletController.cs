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
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletController(IUserService userService,
                                IWalletService walletService,
                                IWalletTransactionService walletTransactionService)
        {
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
                                                                [FromQuery] WalletTransactionFilter walletTransactionFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletTransactionService.GetSellerWalletTransactionsAsync(pagingRequest, walletTransactionFilter, currentUser);
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
    }
}
