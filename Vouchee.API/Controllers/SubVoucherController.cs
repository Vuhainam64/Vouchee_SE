using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/subVoucher")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class SubVoucherController : ControllerBase
    {
        private readonly ISubVoucherService _subVoucherService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public SubVoucherController(ISubVoucherService subVoucherService, 
                                        IUserService userService, 
                                        IRoleService roleService)
        {
            _subVoucherService = subVoucherService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost("create_sub_voucher")]
        [Authorize]
        public async Task<IActionResult> CreateSubVoucher(Guid voucherId, [FromForm] CreateSubVoucherDTO createSubVoucherDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                var result = await _subVoucherService.CreateSubVoucherAsync(voucherId, createSubVoucherDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_sub_voucher")]
        public async Task<IActionResult> GetSubVouchers([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] SubVoucherFilter subVoucherFilter)
        {
            var result = await _subVoucherService.GetSubVouchersAsync(pagingRequest, subVoucherFilter);
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_sub_voucher/{id}")]
        public async Task<IActionResult> GetVoucherCodeById(Guid id)
        {
            var subVoucher = await _subVoucherService.GetSubVoucherByIdAsync(id);
            return Ok(subVoucher);
        }

        // UPDATE
        [HttpPut("update_sub_voucher/{id}")]
        public async Task<IActionResult> UpdateVoucherCode(Guid id, [FromBody] UpdateSubVoucherDTO updateSubVoucherDTO)
        {
            var result = await _subVoucherService.UpdateSubVoucherAsync(id, updateSubVoucherDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_sub_voucher/{id}")]
        public async Task<IActionResult> DeleteVoucherCode(Guid id)
        {
            var result = await _subVoucherService.DeleteSubVoucherAsync(id);
            return Ok(result);
        }
    }
}
