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
    [Route("api/supplier")]
    [EnableCors]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromForm] CreateSupplierDTO createSupplierDTO)
        {
            var result = await _supplierService.CreateSupplierAsync(createSupplierDTO);
            return CreatedAtAction(nameof(GetSupplierById), new { result }, result);
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetSuppliers([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] SupplierFilter supplierFilter,
                                                            [FromQuery] SortSupplierEnum sortSupplierEnum)
        {
            var result = await _supplierService.GetSuppliersAsync(pagingRequest, supplierFilter, sortSupplierEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            return Ok(supplier);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDTO updateSupplierDTO)
        {
            var result = await _supplierService.UpdateSupplierAsync(id, updateSupplierDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            return Ok(result);
        }
    }
}