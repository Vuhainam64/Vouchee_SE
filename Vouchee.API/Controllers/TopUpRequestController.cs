using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/topUpRequest")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class TopUpRequestController : ControllerBase
    {
        private readonly ITopUpRequestService _topUpRequestService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public TopUpRequestController(ITopUpRequestService topUpRequestService, IUserService userService, IRoleService roleService)
        {
            _topUpRequestService = topUpRequestService;
            _userService = userService;
            _roleService = roleService;
        }

        [Authorize]
        [HttpPost("create_top_up_request")]
        public async Task<IActionResult> CreateTopUpRequest([FromBody] CreateTopUpRequestDTO createTopUpRequestDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _topUpRequestService.CreateTopUpRequest(createTopUpRequestDTO, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_top_up_request_by_id/{id}")]
        public async Task<IActionResult> GetTopUpRequestById(Guid id)
        {
            var result = await _topUpRequestService.GetTopUpRequestById(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_top_up_requests")]
        public async Task<IActionResult> GetTopUpRequests([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] TopUpRequestFilter topUpRequestFilter) 
        {
            var result = await _topUpRequestService.GetTopUpRequestsAsync(pagingRequest, topUpRequestFilter);
            return Ok(result);
        }

        //[Authorize]
        //[HttpPut("update_top_up_request/{id}")]
        //public async Task<IActionResult> UpdateTopUpRequest(Guid id, Guid partnerTransactionId)
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var result = await _topUpRequestService.UpdateTopUpRequest(id, partnerTransactionId, currentUser);
        //    return Ok(result);
        //}
    }
}
