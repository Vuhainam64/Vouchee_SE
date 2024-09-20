using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/shop")]
    [EnableCors]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromForm] CreateShopDTO createShopDTO)
        {
            var result = await _shopService.CreateShopAsync(createShopDTO);
            return CreatedAtAction(nameof(GetShopById), new { result }, result);
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] ShopFilter shopFilter,
                                                            [FromQuery] SortShopEnum sortShopEnum)
        {
            var result = await _shopService.GetShopsAsync(pagingRequest, shopFilter, sortShopEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetShopById(Guid id)
        {
            var shop = await _shopService.GetShopByIdAsync(id);
            return Ok(shop);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateShop(Guid id, [FromBody] UpdateShopDTO updateShopDTO)
        {
            var result = await _shopService.UpdateShopAsync(id, updateShopDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteShop(Guid id)
        {
            var result = await _shopService.DeleteShopAsync(id);
            return Ok(result);
        }
    }
}

