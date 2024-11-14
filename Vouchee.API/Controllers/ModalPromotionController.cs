using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/modalPromotion")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class ModalPromotionController : ControllerBase
    {
        private readonly IModalPromotionService _modalPromotionService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public ModalPromotionController(IModalPromotionService modalPromotionService, 
                                            IUserService userService, 
                                            IRoleService roleService)
        {
            _modalPromotionService = modalPromotionService;
            _userService = userService;
            _roleService = roleService;
        }

        [Authorize]
        [HttpPost("create_modal_promotion")]
        public async Task<IActionResult> CreateModalPromotion([FromBody] CreateModalPromotionDTO createModalPromotionDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var modalPromotion = await _modalPromotionService.CreateModalPromotionAsync(createModalPromotionDTO, currentUser);
            return Ok(modalPromotion);
        }

        [HttpGet("get_modal_promotion_by_id/{id}")]
        public async Task<IActionResult> GetModalPromotionBySeller(Guid id)
        {
            var modalPromotion = await _modalPromotionService.GetModalPromotionById(id);
            return Ok(modalPromotion);
        }

        [Authorize]
        [HttpGet("get_modal_promotion_by_seller")]
        public async Task<IActionResult> GetModalPromotionBySeller()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var modalPromotion = await _modalPromotionService.GetModalPromotionBySeller(currentUser);
            return Ok(modalPromotion);
        }
    }
}
