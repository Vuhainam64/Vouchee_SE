using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/brand")]
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
        [HttpPost("create_new_brand")]
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
        [HttpGet("get_all_brand")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands([FromQuery] PagingRequest pagingRequest,
                                                    [FromQuery] BrandFilter brandFilter,
                                                    [FromQuery] SortEnum sortEnum)
        {
            var result = await _brandService.GetBrandsAsync(pagingRequest, brandFilter, sortEnum);
            return Ok(result);
        }

        [HttpGet("get_brand_by_id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound(new { message = "Brand not found." });
            }
            return Ok(brand);
        }

        // UPDATE
        [HttpPut("update_brand/{id}")]
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
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete("delete_brand/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _brandService.DeleteBrandAsync(id);
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
