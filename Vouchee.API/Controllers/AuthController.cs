using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login_with_google_token")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromQuery] string token,
                                                            [FromQuery] DevicePlatformEnum? platform,
                                                            [FromQuery] string? deviceToken)
        {
            if ((platform == null && deviceToken != null) || (platform != null && deviceToken == null))
            {
                return Conflict("Cần phải chọn cả platform và device token");
            }

            var result = await _authService.GetToken(token, platform, deviceToken);
            return Ok(result);
        }
    }
}