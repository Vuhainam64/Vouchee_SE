using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
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
        private readonly IRoleService _roleService;

        public UserController(IUserService userService,
                                IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet("get_all_user")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("get_user_by_id/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("get_user")]
        public async Task<IActionResult> GetUser()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var user = await _userService.GetUserByIdAsync(currentUser.userId);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("update_user/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService); // Ensure current user is fetched here
            var result = await _userService.UpdateUserAsync(id, updateUserDTO, thisUserObj);
            return Ok(result);
        }

        [HttpDelete("delete_user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
}
