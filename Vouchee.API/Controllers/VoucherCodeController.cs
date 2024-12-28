using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    // Gọi sau voucher controller sau khi đã gọi xong trả id về đây để tạo ra voucher code 
    [ApiController]
    [Route("api/v1/voucherCode")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class VoucherCodeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVoucherCodeService _voucherCodeService;
        private readonly IVoucherService _voucherService;
        private readonly IExcelExportService _excelExportService;

        public VoucherCodeController(IUserService userService,
                                     IVoucherCodeService voucherCodeService,
                                     IVoucherService voucherService,
                                     IExcelExportService excelExportService)
        {
            _userService = userService;
            _voucherCodeService = voucherCodeService;
            _voucherService = voucherService;
            _excelExportService = excelExportService;
        }

        // CREATE
        [HttpPost("create_voucher_code")]
        [Authorize]
        public async Task<IActionResult> CreateVoucherCode(Guid modalId, [FromBody] IList<CreateVoucherCodeDTO> createVoucherCodeDTOs)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);
            var result = await _voucherCodeService.CreateVoucherCodeAsync(modalId, createVoucherCodeDTOs, currentUser);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_voucher_code")]
        public async Task<IActionResult> GetVoucherCodes([FromQuery] VoucherCodeFilter voucherCodeFilter)
        {
            var result = await _voucherCodeService.GetVoucherCodesAsync(voucherCodeFilter);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get_supplier_voucher_code")]
        public async Task<IActionResult> GetConvertedVoucherCodes([FromQuery] PagingRequest pagingRequest,
                                                                    [FromQuery] VoucherCodeFilter voucherCodeFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherCodeService.GetSupplierVoucherCodeAsync(currentUser, pagingRequest, voucherCodeFilter);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("get_supplier_voucher_code_converting")]
        public async Task<IActionResult> GetConvertedVoucherCodesConverting([FromQuery] PagingRequest pagingRequest)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherCodeService.GetSupplierVoucherCodeConvertingAsync(currentUser, pagingRequest);
            return Ok(result);
        }
        // GET BY ID
        [HttpGet("get_voucher_code/{id}")]
        public async Task<IActionResult> GetVoucherCodeById(Guid id)
        {
            var voucherCode = await _voucherCodeService.GetVoucherCodeByIdAsync(id);
            return Ok(voucherCode);
        }

        [Authorize]
        [HttpGet("get_voucher_code_by_Update/{id}")]
        public async Task<IActionResult> GetVoucherCodeByUpdateId(Guid id, [FromQuery] PagingRequest pagingRequest)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var voucherCode = await _voucherCodeService.GetVoucherCodeByUpdateIdAsync(currentUser, id, pagingRequest);
            return Ok(voucherCode);
        }

        [Authorize]
        [HttpGet("export_voucher_code_excel")]
        public async Task<IActionResult> ExportVoucherCodeExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var voucherCode = await _excelExportService.GenerateVoucherCodeExcel(currentUser, startDate, endDate);

            var fileName = $"Voucher_code_{currentUser.userId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(voucherCode, contentType, fileName);
        }

        // UPDATE
        [HttpPut("update_voucher_code/{id}")]
        public async Task<IActionResult> UpdateVoucherCode(Guid id, [FromBody] UpdateVoucherCodeDTO updateVoucherCodeDTO)
        {
            var result = await _voucherCodeService.UpdateVoucherCodeAsync(id, updateVoucherCodeDTO);
            return Ok(result);
        }

        [HttpPut("update_status_voucher_code/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusVoucherCode(Guid id, VoucherCodeStatusEnum status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);
            var result = await _voucherCodeService.UpdateStatusVoucherCodeAsync(id, status, currentUser);
            return Ok(result);
        }

        [HttpPut("update_code_voucher_code")]
        [Authorize]
        public async Task<IActionResult> UpdateCodeVoucherCode(IList<UpdateCodeVoucherCodeDTO> updateCodeVoucherCodeDTOs)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _voucherCodeService.UpdateCodeVoucherCodeAsync(updateCodeVoucherCodeDTOs, currentUser);
            return Ok(result);
        }

        [HttpPut("update_list_code_status_converting_voucher_code")]
        [Authorize]
        public async Task<IActionResult> UpdateListCodeStatustoConvertingVoucherCode(IList<Guid> updateCodeVoucherCodeDTOs)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);
            var result = await _voucherCodeService.UpdateVoucherCodeStatusConvertingAsync(updateCodeVoucherCodeDTOs, currentUser);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_voucher_code/{id}")]
        public async Task<IActionResult> DeleteVoucherCode(Guid id)
        {
            var result = await _voucherCodeService.DeleteVoucherCodeAsync(id);
            return Ok(result);
        }
    }
}
