using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/brand")]
    [EnableCors]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public BrandController(IBrandService brandService, 
                                IUserService userService, 
                                IRoleService roleService)
        {
            _brandService = brandService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBrand([FromForm] CreateBrandDTO createBrandDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _brandService.CreateBrandAsync(createBrandDTO, currentUser);
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
        public async Task<IActionResult> GetBrands([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] BrandFilter brandFilter,
                                                            [FromQuery] SortEnum sortEnum)
        {
            var result = await _brandService.GetBrandsAsync(pagingRequest, brandFilter, sortEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            return Ok(brand);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromBody] UpdateBrandDTO updateBrandDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _brandService.UpdateBrandAsync(id, updateBrandDTO);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                Code = HttpStatusCode.Forbidden,
                Message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //ADMIN
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _brandService.DeleteBrandAsync(id);
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
