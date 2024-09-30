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
    [Route("api/user")]
    [EnableCors]
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

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserDTO createUserDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                    || currentUser.roleId.Equals(currentUser.sellerRoleId)
                    || currentUser.roleId.Equals(currentUser.buyerRoleId)
                    || currentUser.roleId.Equals(currentUser.staffRoleId)
                    )
            {
                var result = await _userService.CreateUserAsync(createUserDTO, currentUser);
                return CreatedAtAction(nameof(GetUserById), new { result }, result);
            }
            return null;
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] UserFilter userFilter,
                                                            [FromQuery] SortUserEnum sortUserEnum)
        {
            var result = await _userService.GetUsersAsync(pagingRequest, userFilter, sortUserEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            ThisUserObj thisUserObj = null;
            var result = await _userService.UpdateUserAsync(id, updateUserDTO, thisUserObj);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
}

