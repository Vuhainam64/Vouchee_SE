using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/myVoucher")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class MyVoucherController : ControllerBase
    {
        private readonly IModalService _modalService;
        private readonly IUserService _userService;
        private readonly IVoucherCodeService _voucherCodeService;

        public MyVoucherController(IModalService modalService,
                                   IUserService userService,
                                   IVoucherCodeService voucherCodeService)
        {
            _modalService = modalService;
            _userService = userService;
            _voucherCodeService = voucherCodeService;
        }

        [HttpGet("get_my_vouchers")]
        [Authorize]
        public async Task<IActionResult> GetOrderedModals([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherCodeFilter voucherCodeFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _modalService.GetOrderedModals(currentUser.userId, pagingRequest, voucherCodeFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_ordered_voucher_codes")]
        public async Task<IActionResult> GetOrderedVoucherCodes(Guid modalId, [FromQuery] PagingRequest pagingRequest, [FromQuery] VoucherCodeFilter voucherCodeFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherCodeService.GetOrderedVoucherCode(modalId, currentUser, pagingRequest, voucherCodeFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_voucher_code/{id}")]
        public async Task<IActionResult> GetVoucherCodeById(Guid id)
        {
            var voucherCode = await _voucherCodeService.GetVoucherCodeByIdAsync(id);
            return Ok(voucherCode);
        }
    }
}
