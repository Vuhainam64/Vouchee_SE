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
        public Task<ResponseMessage<Guid>> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> CreateSupplierWalletAsync(Guid supplierId);

        // READ
        public Task<GetSupplierDTO> GetSupplierByIdAsync(Guid id);
        public Task<IList<GetSupplierDTO>> GetSuppliersAsync();
        public Task<IList<BestSuppleriDTO>> GetBestSuppliers();
        public Task<ResponseMessage<dynamic>> GetSupplierWalletTransactionAsync(ThisUserObj currentUser, PagingRequest pagingRequest, SupplierWalletTransactionFilter supplierWalletTransactionFilter);
        public Task<dynamic> GetSupplierOrderAsync(ThisUserObj currentUser, PagingRequest pagingRequest, OrderFilter orderFilter);
        public Task<ResponseMessage<dynamic>> GetSupplierDashboard(ThisUserObj currentUser);
        public Task<ResponseMessage<dynamic>> GetAdminDashboard();
        public Task<ResponseMessage<dynamic>> GetSupplierDashboardbyday(ThisUserObj currentUser);

        // UPDATE
        public Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDTO updateSupplierDTO);
        public Task<ResponseMessage<bool>> UpdateSupplierBankAsync(UpdateBankSupplierDTO updateBankSupplierDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeleteSupplierAsync(Guid id);
    }
}