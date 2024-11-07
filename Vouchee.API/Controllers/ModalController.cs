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
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/modal")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class ModalController : ControllerBase
    {
        private readonly IModalService _modalService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public ModalController(IModalService modalService, 
                                        IUserService userService, 
                                        IRoleService roleService)
        {
            _modalService = modalService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost("create_modal")]
        [Authorize]
        public async Task<IActionResult> CreateModal(Guid voucherId, [FromBody] CreateModalDTO createModalDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //SELLER
            var result = await _modalService.CreateModalAsync(voucherId, createModalDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_modal")]
        [Authorize]
        public async Task<IActionResult> GetModals([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] ModalFilter modalFilter)
        {
            var result = await _modalService.GetModalsAsync(pagingRequest, modalFilter);
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_modal/{id}")]
        [Authorize]
        public async Task<IActionResult> GetVoucherCodeById(Guid id, [FromQuery] PagingRequest pagingRequest)
        {
            var modal = await _modalService.GetModalByIdAsync(id, pagingRequest);
            return Ok(modal);
        }

        // UPDATE
        [HttpPut("update_modal/{id}")]
        public async Task<IActionResult> UpdateVoucherCode(Guid id, [FromBody] UpdateModalDTO updateModalDTO)
        {
            var result = await _modalService.UpdateModalAsync(id, updateModalDTO);
            return Ok(result);
        }
        [HttpPut("update_modal_status/{id}")]
        public async Task<IActionResult> UpdateModalStatus(Guid id, VoucherStatusEnum modalStatus)
        {
            var result = await _modalService.UpdateModalStatusAsync(id, modalStatus);
            return Ok(result);
        }
        [HttpPut("update_modal_isActive/{id}")]
        public async Task<IActionResult> UpdateModalisActive(Guid id, bool isActive)
        {
            var result = await _modalService.UpdateModalisActiveAsync(id, isActive);
            return Ok(result);
        }
        // DELETE
        [HttpDelete("delete_modal/{id}")]
        public async Task<IActionResult> DeleteVoucherCode(Guid id)
        {
            var result = await _modalService.DeleteModalAsync(id);
            return Ok(result);
        }
    }
}
