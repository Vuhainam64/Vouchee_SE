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

        // CREATE (Commented out for now)
        //[HttpPost("create_user")]
        //public async Task<IActionResult> CreateUser([FromForm] CreateUserDTO createUserDTO)
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);
        //
        //    if (currentUser.roleId.Equals(currentUser.adminRoleId)
        //        || currentUser.roleId.Equals(currentUser.sellerRoleId)
        //        || currentUser.roleId.Equals(currentUser.buyerRoleId)
        //        || currentUser.roleId.Equals(currentUser.staffRoleId))
        //    {
        //        var result = await _userService.CreateUserAsync(createUserDTO, currentUser);
        //        return CreatedAtAction(nameof(GetUserById), new { id = result.UserId }, result);
        //    }
        //    return Forbid("Unauthorized to create user");
        //}

        // READ: Get all users with filtering, sorting, and paging
        [HttpGet("get_all_user")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetUsersAsync();
            return Ok(result);
        }

        // READ: Get user by ID
        [HttpGet("get_user{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        // UPDATE: Update a user by ID
        [HttpPut("update_user/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService); // Ensure current user is fetched here
            var result = await _userService.UpdateUserAsync(id, updateUserDTO, thisUserObj);
            return Ok(result);
        }

        // DELETE: Delete a user by ID
        [HttpDelete("delete_user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
}
