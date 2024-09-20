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
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthController(IAuthService authService, IUserService userService, IRoleService roleService)
        {
            _authService = authService;
            _userService = userService;
            _roleService = roleService;
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
        public async Task<IActionResult> GetTokenBuyer([FromQuery] string token)
        {
            var result = await _authService.GetTokenBuyer(token);
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
