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
        private readonly IRoleService _roleService;
        private readonly IWalletService _walletService;

        public WalletController(IUserService userService,
                                IRoleService roleService,
                                IWalletService walletService)
        {
            _userService = userService;
            _roleService = roleService;
            _walletService = walletService;
        }

        [Authorize]
        [HttpPost("create_wallet")]
        public async Task<IActionResult> CreateWallet()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _walletService.CreateWalletAsync(currentUser);
            return Ok(result);
        }

        [HttpGet("get_wallet_by_id/{id}")]
        public async Task<IActionResult> GetWalletById(Guid id)
        {
            var result = await _walletService.GetWalletByIdAsync(id);
            return Ok(result);
        }
    }
}
