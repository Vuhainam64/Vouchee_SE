using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/accountTransaction")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AccountTransactionController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IAccountTransactionService _accountTransactionService;

        public AccountTransactionController(IRoleService roleService,
                                            IUserService userService,
                                            IAccountTransactionService accountTransactionService)
        {
            _roleService = roleService;
            _userService = userService;
            _accountTransactionService = accountTransactionService;
        }

        [Authorize]
        [HttpPost("request_top_up")]
        public async Task<IActionResult> RequestTopUp(int amount)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _accountTransactionService.CreateTopUpRequestAsync(currentUser, amount);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_account_transaction_by_id/{id}")]
        public async Task<IActionResult> GetAccountTransaction(Guid id)
        {
            var result = await _accountTransactionService.GetAccountTransactionById(id);
            return Ok(result);
        }
    }
}
