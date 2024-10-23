using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Business.Services;

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
        public async Task<IActionResult> LoginWithGoogle([FromQuery] string token)
        {
            var result = await _authService.GetToken(token);
            return Ok(result);
        }
    }
}