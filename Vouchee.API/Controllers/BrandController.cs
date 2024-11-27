using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/brand")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IUserService _userService;

        public BrandController(IBrandService brandService,
                               IUserService userService)
        {
            _brandService = brandService;
            _userService = userService;
        }

        // CREATE
        [HttpPost("create_brand")]
        [Authorize]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDTO createBrandDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _brandService.CreateBrandAsync(createBrandDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_brand")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands([FromQuery] PagingRequest pagingRequest,
                                                        [FromQuery] BrandFilter brandFilter)
        {
            var result = await _brandService.GetBrandsAsync(pagingRequest, brandFilter);
            return Ok(result);
        }

        [HttpGet("get_brand_by_name")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands(string name)
        {
            var result = await _brandService.GetBrandsByName(name);
            return Ok(result);
        }

        [HttpGet("get_brand/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            return Ok(result);
        }

        // UPDATE
        [HttpPut("update_brand/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromBody] UpdateBrandDTO updateBrandDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _brandService.UpdateBrandAsync(id, updateBrandDTO, currentUser);
            return Ok(result);
        }

        [HttpPut("update_brand_state/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBrandState(Guid id, [FromQuery] bool isActive)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _brandService.UpdateBrandStateAsync(id, isActive, currentUser);
            return Ok(result);
        }

        [HttpPut("update_brand_status/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBrandStatus(Guid id, [FromQuery] ObjectStatusEnum status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _brandService.UpdateBrandStatusAsync(id, status, currentUser);
            return Ok(result);
        }

        [HttpPut("verify_brand/{id}")]
        [Authorize]
        public async Task<IActionResult> VerifyBrand(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
            {
                var result = await _brandService.VerifyBrand(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có quản trị viên mới có thể thực hiện chức năng này"
            });
        }

        [HttpPut("remove_address_from_brand")]
        [Authorize]
        public async Task<IActionResult> RemoveAddressFromBrand(Guid addressId, Guid brandId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _brandService.RemoveAddressFromBrandAsync(addressId, brandId);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_brand/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            if (currentUser.role.Equals(RoleEnum.ADMIN.ToString()))
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
