using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IOrderService
    {
        // CREATE
        public Task<Guid?> CreateOrderAsync(CreateOrderDTO createOrderDTO);

        // READ
        public Task<GetOrderDTO> GetOrderByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetOrderDTO>> GetOrdersAsync(PagingRequest pagingRequest,
                                                                            OrderFilter orderFilter,
                                                                            SortOrderEnum sortOrderEnum);

        // UPDATE
        public Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDTO updateOrderDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeleteOrderAsync(Guid id);
    }
}
