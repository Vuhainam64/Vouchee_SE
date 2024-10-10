using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface ISupplierService
    {
        // CREATE
        public Task<Guid?> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO);

        // READ
        public Task<GetSupplierDTO> GetSupplierByIdAsync(Guid id);
        public Task<IList<GetSupplierDTO>> GetSuppliersAsync();

        // UPDATE
        public Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDTO updateSupplierDTO);

        // DELETE
        public Task<bool> DeleteSupplierAsync(Guid id);
    }
}