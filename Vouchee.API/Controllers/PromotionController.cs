using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Models.DTOs;
using Vouchee.Business.Services.Impls;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/promotion")]
    [EnableCors]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public PromotionController(IPromotionService promotionService,
                                    IUserService userService,
                                    IRoleService roleService)
        {
            _promotionService = promotionService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePromotion([FromForm] CreatePromotionDTO createPromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                var result = await _promotionService.CreatePromotionAsync(createPromotionDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotions([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] PromotionFilter orderFilter,
                                                            [FromQuery] SortPromotionEnum sortPromotionEnum)
        {
            var result = await _promotionService.GetPromotionsAsync(pagingRequest, orderFilter, sortPromotionEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var order = await _promotionService.GetPromotionByIdAsync(id);
            return Ok(order);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePromotion(Guid id, [FromBody] UpdatePromotionDTO updatePromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                var result = await _promotionService.UpdatePromotionAsync(id, updatePromotionDTO, currentUser);
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

            //SELLER
            if (currentUser.roleId.Equals(currentUser.sellerRoleId))
            {
                var result = await _promotionService.DeletePromotionAsync(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                Code = HttpStatusCode.Forbidden,
                Message = "Chỉ có nhà bán hàng mới có thể thực hiện chức năng này"
            });
        }
    }
}
