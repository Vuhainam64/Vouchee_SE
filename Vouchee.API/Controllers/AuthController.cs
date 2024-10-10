using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("login_with_google_token")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromQuery] string token)
        {
            var result = await _authService.GetToken(token);
            return Ok(result);
        }

        [HttpGet("login_with_phone_number")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithPhoneNumber(LoginByPhoneNumberDTO loginByPhoneNumberDTO)
        {
            var result = await _authService.LoginWithPhoneNumber(loginByPhoneNumberDTO);
            return Ok(result);
        }

        [HttpGet("login_with_email")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithEmailr(LoginByEmailDTO loginByEmailDTO)
        {
            var result = await _authService.LoginWithEmail(loginByEmailDTO);
            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterDTO registerDTO)
        {
            var result = await _authService.Register(registerDTO);
            return Ok(result);
        }
    }
}