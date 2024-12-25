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

        [HttpGet("get_active_user_dashboard")]
        public async Task<IActionResult> GetActiveUserDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetActiveUserDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_voucher_dashboard")]
        public async Task<IActionResult> GetVoucherDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetVoucherDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_modal_dashboard")]
        public async Task<IActionResult> GetModalDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetModalDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_voucher_code_dashboard")]
        public async Task<IActionResult> GetVoucherCodeDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetVoucherCodeDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_order_dashboard")]
        public async Task<IActionResult> GetOrderDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetOrderDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_withdraw_request_dashboard")]
        public async Task<IActionResult> GetWithdrawRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetWithdrawRequestDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_topup_request_dashboard")]
        public async Task<IActionResult> GetTopUpRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetTopUpRequestDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_refund_request_dashboard")]
        public async Task<IActionResult> GetRefundRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetRefundRequestDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_order_transaction_dashboard")]
        public async Task<IActionResult> GetOrderTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetOrderWalletTransactionDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_topup_transaction_dashboard")]
        public async Task<IActionResult> GetTopUpTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetTopupWalletTransactionDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_withdraw_transaction_dashboard")]
        public async Task<IActionResult> GetWithdrawTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetWithdrawWalletTransactionDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_refund_transaction_dashboard")]
        public async Task<IActionResult> GetRefundTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetRefundWalletTransactionDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }

        [HttpGet("get_partner_transaction_dashboard")]
        public async Task<IActionResult> GetPartnerTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum filterType)
        {
            var result = await _dashboardService.GetPartnerTransactionDashboard(fromDate, toDate, today, filterType);
            return Ok(result);
        }
    }
}
