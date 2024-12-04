using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/voucherCode")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class PosController : Controller
    {
        private readonly IUserService _userService;
        private readonly IVoucherCodeService _voucherCodeService;
        private readonly IVoucherService _voucherService;

        public PosController(IUserService userService, IVoucherCodeService voucherCodeService, IVoucherService voucherService)
        {
            _userService = userService;
            _voucherCodeService = voucherCodeService;
            _voucherService = voucherService;
        }

        [HttpPut("Scan_QR")]
        [Authorize]
        public async Task<IActionResult> ScanQR(string code)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);
            var result = await _voucherCodeService.UpdatePosVoucherCodeAsync(code, currentUser);
            return Ok(result);
        }
    }
}
