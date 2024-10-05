using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/order")]
    [EnableCors]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public OrderController(IOrderService orderService,
                                IUserService userService,
                                IRoleService roleService)
        {
            _orderService = orderService;
            _userService = userService;
            _roleService = roleService;
        }

        // CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            //BUYER
            if (currentUser.roleId.Equals(currentUser.buyerRoleId))
            {
                var result = await _orderService.CreateOrderAsync(createOrderDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có người mua mới có thể thực hiện chức năng này"
            });
        }

        // READ
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] PagingRequest pagingRequest,
                                                            [FromQuery] OrderFilter orderFilter,
                                                            [FromQuery] SortOrderEnum sortOrderEnum)
        {
            var result = await _orderService.GetOrdersAsync(pagingRequest, orderFilter, sortOrderEnum);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        // UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderDTO updateOrderDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            if (currentUser.roleId.Equals(currentUser.sellerRoleId)
                    || currentUser.roleId.Equals(currentUser.buyerRoleId))
            {
                var result = await _orderService.UpdateOrderAsync(id, updateOrderDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                code = HttpStatusCode.Forbidden,
                message = "Chỉ có nhà bán hàng hoặc người mua mới có thể thực hiện chức năng này"
            });
        }

        // DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            return Ok(result);
        }
    }
}
