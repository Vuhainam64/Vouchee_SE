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
    [EnableCors]
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

        [HttpPost("login_with_phone_number")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithPhoneNumber([FromBody] LoginByPhoneNumberDTO loginByPhoneNumberDTO)
        {
            var result = await _authService.LoginWithPhoneNumber(loginByPhoneNumberDTO);
            return Ok(result);
        }

        [HttpPost("login_with_email")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithEmail([FromBody] LoginByEmailDTO loginByEmailDTO)
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

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshTokenRequest refreshTokenRequest)
        {
            var result = await _authService.Refresh(refreshTokenRequest);
            return Ok(result);
        }
    }
}