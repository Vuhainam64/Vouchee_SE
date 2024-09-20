using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;

namespace Vouchee.Business.Services
{
    public interface ISupplierService
    {
        // CREATE
        public Task<Guid?> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO);

        // READ
        public Task<GetSupplierDTO> GetSupplierByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetSupplierDTO>> GetSuppliersAsync(PagingRequest pagingRequest,
                                                                                SupplierFilter shopFilter,
                                                                                SortSupplierEnum sortSupplierEnum);

        // UPDATE
        public Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDTO updateSupplierDTO);

        // DELETE
        public Task<bool> DeleteSupplierAsync(Guid id);
    }
}