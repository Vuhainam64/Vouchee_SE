using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Filters;
using Microsoft.AspNetCore.Cors;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/report")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;

        public ReportController(IReportService reportService, IUserService userService)
        {
            _reportService = reportService;
            _userService = userService;
        }

        // CREATE
        [Authorize]
        [HttpPost("create_report/{userId}")]
        public async Task<IActionResult> CreateReport(Guid userId, [FromBody] CreateReportDTO createReportDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _reportService.CreateReportAsync(userId, createReportDTO, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_report")]
        public async Task<IActionResult> GetAllReport([FromQuery] PagingRequest pagingRequest,
                                                        [FromQuery] ReportFilter reportFilter)
        {
            var result = await _reportService.GetReportsAsync(pagingRequest, reportFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_user_report")]
        public async Task<IActionResult> GetUserReport([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] ReportFilter reportFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _reportService.GetUserReportsAsync(pagingRequest, reportFilter, currentUser);
            return Ok(result);
        }

        [HttpGet("get_report/{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var address = await _reportService.GetReportByIdAsync(id);
            return Ok(address);
        }

        // UPDATE
        [Authorize]
        [HttpPut("update_report/{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] UpdateReportDTO updateReportDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _reportService.UpdateReportAsync(id, updateReportDTO, currentUser);
            return Ok(result);
        }

        // DELETE
        [Authorize]
        [HttpDelete("delete_report/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _reportService.DeleteReportAsync(id, currentUser);
            return Ok(result);
        }
    }
}
