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
    [Route("api/v1/shopPromotion")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class ShopPromotionController : ControllerBase
    {
        private readonly IShopPromotionService _shopPromotionService;
        private readonly IUserService _userService;

        public ShopPromotionController(IShopPromotionService shopPromotionService,
                                    IUserService userService)
        {
            _shopPromotionService = shopPromotionService;
            _userService = userService;
        }

        // CREATE
        [HttpPost("create_shop_promotion")]
        [Authorize]
        public async Task<IActionResult> CreateShopPromotion([FromBody] CreateShopPromotionDTO createPromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _shopPromotionService.CreateShopPromotionAsync(createPromotionDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_shop_promotions")]
        public async Task<IActionResult> GetShopPromotions([FromQuery] PagingRequest pagingRequest, 
                                                                [FromQuery] ShopPromotionFilter promotionFilter)
        {
            var result = await _shopPromotionService.GetPromotionsAsync(pagingRequest, promotionFilter);
            return Ok(result);
        }

        [HttpGet("get_shop_promotion_by_id/{id}")]
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var promotion = await _shopPromotionService.GetPromotionByIdAsync(id);
            return Ok(promotion);
        }

        [HttpGet("get_promotions_by_shop_id")]
        public async Task<IActionResult> GetShopPromotions(Guid shopId)
        {
            var promotion = await _shopPromotionService.GetShopPromotionByShopId(shopId);
            return Ok(promotion);
        }

        //[Authorize]
        //[HttpGet("get_active_shop_promotion")]
        //public async Task<IActionResult> GetActivePromotion()
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

        //    var promotion = await _shopPromotionService.GetActiveShopPromotion(currentUser);
        //    return Ok(promotion);
        //}

        //[Authorize]
        //[HttpGet("get_shop_promotion")]
        //public async Task<IActionResult> GetShopPromotion()
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var promotion = await _shopPromotionService.GetShopPromotionBySeller(currentUser);
        //    return Ok(promotion);
        //}

        // UPDATE
        //[HttpPut("update_shop_promotion/{id}")]
        //[Authorize]
        //public async Task<IActionResult> UpdatePromotion(Guid id, [FromBody] UpdateShopPromotionDTO updatePromotionDTO)
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var result = await _shopPromotionService.UpdatePromotionAsync(id, updatePromotionDTO, currentUser);
        //    return Ok(result);
        //}

        //// DELETE
        //[HttpDelete("delete_promotion/{id}")]
        //[Authorize]
        //public async Task<IActionResult> DeletePromotion(Guid id)
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var result = await _shopPromotionService.DeletePromotionAsync(id, currentUser);
        //    return Ok(result);
        //}
    }
}
