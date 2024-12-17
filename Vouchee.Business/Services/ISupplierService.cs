using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.DTOs.Dashboard;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface ISupplierService
    {
        // CREATE
        public Task<Guid?> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO);
        public Task<ResponseMessage<bool>> CreateSupplierWalletAsync(Guid supplierId);

        // READ
        public Task<GetSupplierDTO> GetSupplierByIdAsync(Guid id);
        public Task<IList<GetSupplierDTO>> GetSuppliersAsync();
        public Task<IList<BestSuppleriDTO>> GetBestSuppliers();
        public Task<dynamic> GetSupplierWalletTransactionAsync(ThisUserObj currentUser, PagingRequest pagingRequest, SupplierWalletTransactionFilter supplierWalletTransactionFilter);
        public Task<dynamic> GetSupplierDashboard(ThisUserObj currentUser);

        // UPDATE
        public Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDTO updateSupplierDTO);
        public Task<ResponseMessage<bool>> UpdateSupplierBankAsync(UpdateBankSupplierDTO updateBankSupplierDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeleteSupplierAsync(Guid id);
    }
}