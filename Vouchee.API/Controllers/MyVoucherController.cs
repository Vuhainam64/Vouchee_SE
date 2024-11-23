using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
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

        public MyVoucherController(IModalService modalService,
                                   IUserService userService)
        {
            _modalService = modalService;
            _userService = userService;
        }

        [HttpGet("get_my_vouchers")]
        [Authorize]
        public async Task<IActionResult> GetOrderedModals([FromQuery] PagingRequest pagingRequest,
                                                    [FromQuery] ModalFilter modalFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _modalService.GetOrderedModals(currentUser.userId, pagingRequest, modalFilter);
            return Ok(result);
        }
    }
}
