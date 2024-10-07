using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Migrations;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/category")]
    [EnableCors]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public CategoryController(ICategoryService categoryService, 
                                    IUserService userService, 
                                    IRoleService roleService)
        {
            _categoryService = categoryService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory(Guid voucherTypeId, [FromForm] CreateCategoryDTO createPromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
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
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] CategoryFilter categoryFilter,
                                                            [FromQuery] SortEnum sortEnum)
        {
            var result = await _categoryService.GetCategoriesAsync(pagingRequest, categoryFilter, sortEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var order = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(order);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDTO updatePromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _categoryService.UpdateCategoryAsync(id, updatePromotionDTO);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                Code = HttpStatusCode.Forbidden,
                Message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //ADMIN
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                Code = HttpStatusCode.Forbidden,
                Message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }
    }
}
