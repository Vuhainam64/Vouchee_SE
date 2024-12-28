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
        [HttpGet("get_all_rating")]
        public async Task<IActionResult> GetAllRating([FromQuery] PagingRequest pagingRequest, [FromQuery] RatingFilter ratingFilter)
        {
            var result = await _ratingService.GetRatingAsync(pagingRequest, ratingFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_all_rating_by_seller")]
        public async Task<IActionResult> GetRatingsBySeller([FromQuery] PagingRequest pagingRequest, [FromQuery] RatingFilter ratingFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.GetSellerRatingAsync(pagingRequest, ratingFilter, currentUser);
            return Ok(result);
        } 
        
        [HttpGet("get_rating_by_id/{id}")]
        public async Task<IActionResult> GetRatingById(Guid id)
        {
            var result = await _ratingService.GetRatingByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_rating_dashboard")]
        public async Task<IActionResult> GetRatingDashboard()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.GetRatingDashboard(currentUser);
            return Ok(result);
        }

        // UPDATE
        [Authorize]
        [HttpPut("update_rating/{id}")]
        public async Task<IActionResult> UpdateRating(Guid id, [FromBody] UpdateRatingDTO updateRatingDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.UpdateRatingAsync(id, updateRatingDTO, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("reply_rating/{id}")]
        public async Task<IActionResult> ReplyRating(Guid id, string rep)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.ReplyRatingAsync(id, rep, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("report_rating/{id}")]
        public async Task<IActionResult> ReportRating(Guid id, string reason)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.ReportRatingAsync(id, reason, currentUser);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete_rating/{id}")]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _ratingService.DeleteRatingAsync(id, currentUser);
            return Ok(result);
        }
    }
}
