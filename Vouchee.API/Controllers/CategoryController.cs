using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/category")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public CategoryController(ICategoryService categoryService,
                                  IUserService userService)
        {
            _categoryService = categoryService;
            _userService = userService;
        }

        // CREATE
        [HttpPost("create_category")]
        [Authorize]
        public async Task<IActionResult> CreateCategory(Guid voucherTypeId, [FromBody] CreateCategoryDTO createPromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
            {
                var result = await _categoryService.CreateCategoryAsync(voucherTypeId, createPromotionDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet("get_all_category")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] CategoryFilter categoryFilter)
        {
            var result = await _categoryService.GetCategoriesAsync(pagingRequest, categoryFilter);
            return Ok(result);
        }

        [HttpGet("get_category/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        // UPDATE
        [HttpPut("update_category/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
            {
                var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        [HttpPut("update_category_state/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategoryState(Guid id, bool isActive)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
            {
                var result = await _categoryService.UpdateCategoryStateAsync(id, isActive, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // REMOVE
        [HttpDelete("delete_category/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
            {
                var result = await _categoryService.DeleteCategoryAsync(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }
    }
}
