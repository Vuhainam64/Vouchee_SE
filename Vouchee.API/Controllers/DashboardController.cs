using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("get_order_dashboard")]
        public async Task<IActionResult> GetOrderDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetOrderDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_active_user_dashboard")]
        public async Task<IActionResult> GetActiveUserDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetActiveUserDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }
    }
}
