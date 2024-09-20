using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/order")]
    [EnableCors]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] CreateOrderDTO createOrderDTO)
        {
            var result = await _orderService.CreateOrderAsync(createOrderDTO);
            return CreatedAtAction(nameof(GetOrderById), new { result }, result);
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] OrderFilter orderFilter,
                                                            [FromQuery] SortOrderEnum sortOrderEnum)
        {
            var result = await _orderService.GetOrdersAsync(pagingRequest, orderFilter, sortOrderEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderDTO updateOrderDTO)
        {
            var result = await _orderService.UpdateOrderAsync(id, updateOrderDTO);
            return Ok(result);
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            return Ok(result);
        }
    }
}
