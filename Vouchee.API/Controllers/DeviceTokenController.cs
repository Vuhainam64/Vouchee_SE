using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/deviceToken")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class DeviceTokenController : Controller
    {
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly IUserService _userService;

        public DeviceTokenController(IDeviceTokenService deviceTokenService,
                                     IUserService userService)
        {
            _deviceTokenService = deviceTokenService;
            _userService = userService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_device_token")]
        public async Task<IActionResult> CreateDeviceToken([FromQuery] DevicePlatformEnum devicePlatformEnum, CreateDeviceTokenDTO createDeviceTokenDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _deviceTokenService.CreateDeviceToken(currentUser.userId, createDeviceTokenDTO, devicePlatformEnum);
            return Ok(result);
        }

        // READ
        [Authorize]
        [HttpGet("get_all_device_token")]
        public async Task<IActionResult> GetAllDeviceToken([FromQuery] PagingRequest pagingRequest, [FromQuery] DeviceTokenFilter deviceTokenFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _deviceTokenService.GetDeviceTokenAsync(pagingRequest, deviceTokenFilter, currentUser.userId);
            return Ok(result);
        }
    }
}
