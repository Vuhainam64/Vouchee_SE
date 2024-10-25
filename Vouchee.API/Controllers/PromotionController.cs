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
    [Route("api/promotion/v1")]
    [EnableCors("MyAllowSpecificOrigins")]
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
        [HttpPost("create_promotion")]
        [Authorize]
        public async Task<IActionResult> CreatePromotion([FromForm] CreatePromotionDTO createPromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _promotionService.CreatePromotionAsync(createPromotionDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_promotion")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotions([FromQuery] PagingRequest pagingRequest, [FromQuery] PromotionFilter promotionFilter)
        {
            var result = await _promotionService.GetPromotionsAsync(pagingRequest, promotionFilter);
            return Ok(result);
        }

        [HttpGet("get_all_active_promotion")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivePromotions([FromQuery] PagingRequest pagingRequest, [FromQuery] PromotionFilter promotionFilter)
        {
            var result = await _promotionService.GetActivePromotion(pagingRequest, promotionFilter);
            return Ok(result);
        }

        [HttpGet("get_promotion_by_id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            return Ok(promotion);
        }

        [HttpGet("get_promotion_by_buyer/{buyerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotionByBuyerId(Guid buyerId)
        {
            var promotion = await _promotionService.GetPromotionByBuyerId(buyerId);
            return Ok(promotion);
        }

        // UPDATE
        [HttpPut("update_promotion/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePromotion(Guid id, [FromBody] UpdatePromotionDTO updatePromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _promotionService.UpdatePromotionAsync(id, updatePromotionDTO, currentUser);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_promotion/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _promotionService.DeletePromotionAsync(id, currentUser);
            return Ok(result);
        }
    }
}
