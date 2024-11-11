using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/walletTransaction")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class WalletTransactionController : Controller
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public WalletTransactionController(IWalletTransactionService walletTransactionService,  
                                            IUserService userService, 
                                            IRoleService roleService)
        {
            _walletTransactionService = walletTransactionService;
            _userService = userService;
            _roleService = roleService;
        }

        [Authorize]
        [HttpGet("get_wallet_transactions_by_user")]
        public async Task<IActionResult> GetWalletById([FromQuery] PagingRequest pagingRequest,
                                                        [FromQuery] WalletTransactionFilter walletTransactionFilter,
                                                        WalletTransactionTypeEnum walletTransactionTypeEnum )
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _walletTransactionService.GetWalletTransactionsAsync(pagingRequest, walletTransactionFilter, currentUser, walletTransactionTypeEnum);
            return Ok(result);
        }
    }
}
