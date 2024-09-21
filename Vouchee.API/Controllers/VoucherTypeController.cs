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
    [Route("api/voucherType")]
    [EnableCors]
    public class VoucherTypeController : ControllerBase
    {
        private readonly IVoucherTypeService _voucherTypeService;

        public VoucherTypeController(IVoucherTypeService voucherTypeService)
        {
            _voucherTypeService = voucherTypeService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateVoucherType([FromForm] CreateVoucherTypeDTO createVoucherTypeDTO)
        {
            var result = await _voucherTypeService.CreateVoucherTypeAsync(createVoucherTypeDTO);
            return CreatedAtAction(nameof(GetVoucherTypeById), new { result }, result);
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetVoucherTypes([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] VoucherTypeFilter voucherTypeFilter,
                                                            [FromQuery] SortVoucherTypeEnum sortVoucherTypeEnum)
        {
            var result = await _voucherTypeService.GetVoucherTypesAsync(pagingRequest, voucherTypeFilter, sortVoucherTypeEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoucherTypeById(Guid id)
        {
            var voucherType = await _voucherTypeService.GetVoucherTypeByIdAsync(id);
            return Ok(voucherType);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateVoucherType(Guid id, [FromBody] UpdateVoucherTypeDTO updateVoucherTypeDTO)
        {
            var result = await _voucherTypeService.UpdateVoucherTypeAsync(id, updateVoucherTypeDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoucherType(Guid id)
        {
            var result = await _voucherTypeService.DeleteVoucherTypeAsync(id);
            return Ok(result);
        }
    }
}

