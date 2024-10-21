using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IOrderService
    {
        // CREATE
        // public Task<Guid?> CreateOrderAsync(CreateOrderDTO createOrderDTO, ThisUserObj thisUserObj);
        public Task<Guid?> CreateOrderAsync(ThisUserObj thisUserObj);

        // READ
        public Task<GetOrderDTO> GetOrderByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetOrderDTO>> GetOrdersAsync(PagingRequest pagingRequest, OrderFilter orderFilter, ThisUserObj? thisUserObj);

        // UPDATE
        public Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDTO updateOrderDTO, ThisUserObj thisUserObj);
        public Task<bool> AssignCodeToOrderAsync(Guid orderDetailId, VoucherCodeList voucherCodeId);

        // DELETE
        public Task<bool> DeleteOrderAsync(Guid id);
    }
}
