using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Services;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableCors]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("seller")]
        public async Task<IActionResult> GetTokenSeller([FromQuery] string token)
        {
            var result = await _authService.GetTokenSeller(token);
            return Ok(result);
        }

        [HttpPost]
        [Route("buyer")]
        public async Task<IActionResult> GetTokenBuyer([FromQuery] string token, string deviceToken)
        {
            var result = await _authService.GetTokenBuyer(token, deviceToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("admin")]
        public async Task<IActionResult> GetTokenAdmin([FromQuery] string token)
        {
            var result = await _authService.GetTokenAdmin(token);
            return Ok(result);
        }
    }
}