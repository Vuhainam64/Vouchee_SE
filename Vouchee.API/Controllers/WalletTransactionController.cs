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

        public WalletTransactionController(IWalletTransactionService walletTransactionService,  
                                            IUserService userService)
        {
            _walletTransactionService = walletTransactionService;
            _userService = userService;
        }
    }
}
