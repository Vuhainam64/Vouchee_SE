using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Business.Services;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/vouchers")]
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
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDTO voucherDTO)
        {
            var result = await _voucherService.CreateVoucherAsync(voucherDTO);
            return CreatedAtAction(nameof(GetVoucherById), new { result }, result);
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllVouchers([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherFiler voucherFiler,
                                                            [FromQuery] VoucherOrderEnum voucherOrderEnum)
        {
            var result = await _voucherService.GetVouchersAsync(pagingRequest, voucherFiler, voucherOrderEnum);
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
    }
}
