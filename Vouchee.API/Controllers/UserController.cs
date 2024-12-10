using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISendEmailService _sendEmailService;

        public UserController(IUserService userService,ISendEmailService sendEmailService)
        {
            _userService = userService;
            _sendEmailService = sendEmailService;
        }

        [HttpPost("create_user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO, [FromQuery] string? deviceToken)
        {
            var result = await _userService.CreateUserAsync(createUserDTO, deviceToken);
            return Ok(result);
        }
        [HttpGet("email")]
        public async Task<IActionResult> email()
        {
            var emailSubject = "Welcome to Our Service";
            var emailBody = "Hello, your account has been successfully created!";

            // Just await the method
            await _sendEmailService.SendEmailAsync("caothang7a7@gmail.com", emailSubject, emailBody);

            // Return a success response
            return Ok(new { message = "Email sent successfully" });
        }

        [HttpGet("get_all_user")]
        public async Task<IActionResult> GetUsers([FromQuery] PagingRequest pagingRequest, 
                                                    [FromQuery] UserFilter userFilter)
        {
            var result = await _userService.GetUsersAsync(pagingRequest, userFilter);
            return Ok(result);
        }

        [HttpGet("get_user_by_id/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("get_current_user")]
        public async Task<IActionResult> GetUser()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var user = await _userService.GetUserByIdAsync(currentUser.userId);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("update_user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUserDTO)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService); 

            var result = await _userService.UpdateUserAsync(updateUserDTO, thisUserObj);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update_user_role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDTO updateUserRoleDTO)
        {
            var result = await _userService.UpdateUserRoleAsync(updateUserRoleDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update_user_bank")]
        public async Task<IActionResult> UpdateUserBank([FromBody] UpdateUserBankDTO updateUserBankDTO)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _userService.UpdateUserBankAsync(updateUserBankDTO, thisUserObj);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("ban_user")]
        public async Task<IActionResult> BanUser(Guid userId, bool isBan, string reason)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _userService.BanUserAsync(userId, thisUserObj, isBan, reason);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _userService.DeleteUserAsync(id, thisUserObj);
            return Ok(result);
        }
    }
}
