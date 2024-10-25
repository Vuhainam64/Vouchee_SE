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
    [Route("api/modal")]
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
        public async Task<IActionResult> GetModals([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] ModalFilter modalFilter)
        {
            var result = await _modalService.GetModalsAsync(pagingRequest, modalFilter);
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("get_modal/{id}")]
        public async Task<IActionResult> GetVoucherCodeById(Guid id)
        {
            var modal = await _modalService.GetModalByIdAsync(id);
            return Ok(modal);
        }

        // UPDATE
        [HttpPut("update_modal/{id}")]
        public async Task<IActionResult> UpdateVoucherCode(Guid id, [FromBody] UpdateModalDTO updateModalDTO)
        {
            var result = await _modalService.UpdateModalAsync(id, updateModalDTO);
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
