using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.API.Controllers
{
    // gọi sau voucher controller sau khi đã gọi xong trả id về đây để tạo ra voucher code 
    [ApiController]
    [Route("api/voucherCode")]
    [EnableCors]
    public class VoucherCodeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IVoucherCodeService _voucherCodeService;
        private readonly IVoucherService _voucherService;

        public VoucherCodeController(IUserService userService, 
                                        IRoleService roleService, 
                                        IVoucherCodeService voucherCodeService, 
                                        IVoucherService voucherService)
        {
            _userService = userService;
            _roleService = roleService;
            _voucherCodeService = voucherCodeService;
            _voucherService = voucherService;
        }

        // CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVoucherCode(Guid voucherId, [FromForm] CreateVoucherCodeDTO createVoucherCodeDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                var result = await _voucherCodeService.CreateVoucherCodeAsync(voucherId, createVoucherCodeDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }


        // READ

        // UPDATE

        // DELETE
    }
}
