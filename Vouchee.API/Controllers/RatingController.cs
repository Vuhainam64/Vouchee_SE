using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/rating")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly IUserService _userService;

        public RatingController(IRatingService ratingService,
                                IUserService userService)
        {
            _ratingService = ratingService;
            _userService = userService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_rating")]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingDTO createRatingDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.CreateRatingAsync(createRatingDTO, currentUser);
            return Ok(result);
        }

        // READ
        [Authorize]
        [HttpGet("get_all_rating_by_buyer")]
        public async Task<IActionResult> GetAllRating([FromQuery] PagingRequest pagingRequest, [FromQuery] RatingFilter ratingFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.GetRatingAsync(pagingRequest, ratingFilter, currentUser);
            return Ok(result);
        }

        [HttpGet("get_rating_by_id/{id}")]
        public async Task<IActionResult> GetAllRating(Guid id)
        {
            var result = await _ratingService.GetRatingByIdAsync(id);
            return Ok(result);
        }
    }
}
