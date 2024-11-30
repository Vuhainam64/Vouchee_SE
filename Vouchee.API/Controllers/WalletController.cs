using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/wallet")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class WalletController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;

        public WalletController(IUserService userService,
                                IWalletService walletService)
        {
            _userService = userService;
            _walletService = walletService;
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
    }
}
