using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserDTO createUserDTO)
        {
            var result = await _userService.CreateUserAsync(createUserDTO);
            return CreatedAtAction(nameof(GetUserById), new { result }, result);
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
            var result = await _userService.UpdateUserAsync(id, updateUserDTO);
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

