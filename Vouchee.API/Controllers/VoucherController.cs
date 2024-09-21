using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/voucher")]
    [EnableCors]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromForm] CreateVoucherDTO voucherDTO)
        {
            var result = await _voucherService.CreateVoucherAsync(voucherDTO);
            return CreatedAtAction(nameof(GetVoucherById), new { result }, result);
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllVouchers([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherFiler voucherFiler,
                                                            [FromQuery] SortVoucherEnum sortVoucherEnum)
        {
            var result = await _voucherService.GetVouchersAsync(pagingRequest, voucherFiler, sortVoucherEnum);
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id);
            return Ok(voucher);
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] UpdateVoucherDTO voucherDTO)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, voucherDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var result = await _voucherService.DeleteVoucherAsync(id);
            return Ok(result);
        }
    }
}