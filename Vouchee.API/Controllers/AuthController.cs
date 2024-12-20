﻿using Google.Api.Gax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IDeviceTokenService _deviceTokenService;

        public AuthController(IAuthService authService,
                              IUserService userService,
                              IDeviceTokenService deviceTokenService)
        {
            _authService = authService;
            _userService = userService;
            _deviceTokenService = deviceTokenService;
        }

        [HttpPost("login_with_google_token")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromQuery] string token,
                                                            [FromQuery] string? deviceToken)
        {
            var result = await _authService.GetToken(token, deviceToken);
            return Ok(result);
        }

        [HttpDelete("logout")]
        public async Task<IActionResult> Logout([FromQuery] string deviceToken)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _deviceTokenService.RemoveDeviceTokenAsync(currentUser.userId, deviceToken);
            return Ok(result);
        }
    }
}