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
        public Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDTO updateOrderDTO);

        // DELETE
        public Task<bool> DeleteOrderAsync(Guid id);
    }
}
