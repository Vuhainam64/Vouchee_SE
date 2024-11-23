using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/notification")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public NotificationController(INotificationService notificationService, 
                                        IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        [Authorize]
        [HttpPost("create_notification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _notificationService.CreateNotificationAsync(currentUser.userId, createNotificationDTO);
            return Ok(result);
        }

        [HttpGet("get_receiver_notifications")]
        [Authorize]
        public async Task<IActionResult> GetReceiverNotifications([FromQuery] PagingRequest pagingRequest,
                                                                        [FromQuery] NotifcationFilter notifcationFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _notificationService.GetNotificationsByToUserIdAsync(pagingRequest, notifcationFilter, currentUser.userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("mark_seen_notification/{notificationId}")]
        public async Task<IActionResult> MarkSeenNotification(Guid notificationId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _notificationService.MarkSeenAsync(notificationId, currentUser);
            return Ok(result);
        }
    }
}
