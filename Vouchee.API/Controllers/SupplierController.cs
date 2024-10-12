using Microsoft.AspNetCore.Authorization;
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
    [Route("api/v1/supplier")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // CREATE
        [HttpPost("create_supplier")]
        [Authorize]
        public async Task<IActionResult> CreateSupplier([FromForm] CreateSupplierDTO createSupplierDTO)
        {
            var result = await _supplierService.CreateSupplierAsync(createSupplierDTO);
            return Ok(result);
        }

        // READ
        [HttpGet("get_all_supplier")]
        public async Task<IActionResult> GetSuppliers()
        {
            var result = await _supplierService.GetSuppliersAsync();
            return Ok(result);
        }

        [HttpGet("get_best_suppliers")]
        public async Task<IActionResult> GetBestSuppiers()
        {
            var result = await _supplierService.GetBestSuppliers();
            return Ok(result);
        }

        [HttpGet("get_supplier/{id}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            return Ok(supplier);
        }

        // UPDATE
        [HttpPut("update_supplier/{id}")]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDTO updateSupplierDTO)
        {
            var result = await _supplierService.UpdateSupplierAsync(id, updateSupplierDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("delete_supplier/{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            return Ok(result);
        }
    }
}
