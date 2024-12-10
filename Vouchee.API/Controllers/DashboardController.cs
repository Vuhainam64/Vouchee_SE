using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Services;

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
        public async Task<IActionResult> GetOrderDashboard(int? year)
        {
            var result = await _dashboardService.GetOrderDashboard(year);
            return Ok(result);
        }
    }
}
