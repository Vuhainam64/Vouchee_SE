using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Services;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("seller")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenSeller([FromQuery] string token)
        {
            var result = await _authService.GetTokenSeller(token);
            return Ok(result);
        }

        [HttpPost]
        [Route("buyer")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenBuyer([FromQuery] string token, string? deviceToken)
        {
            var result = await _authService.GetTokenBuyer(token, deviceToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("admin")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenAdmin([FromQuery] string token)
        {
            var result = await _authService.GetTokenAdmin(token);
            return Ok(result);
        }
    }
}