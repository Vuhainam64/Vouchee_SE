using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/weatherForecast")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IWalletService _walletService;
        private readonly IWalletTransactionService _walletTransactionService;

        public WeatherForecastController(IDeviceTokenService deviceTokenService,
                                         IUserService userService,
                                         INotificationService notificationService,
                                         IWalletService walletService,
                                         IWalletTransactionService walletTransactionService)
        {
            _deviceTokenService = deviceTokenService;
            _userService = userService;
            _notificationService = notificationService;
            _walletService = walletService;
            _walletTransactionService = walletTransactionService;
        }



        // CREATE
        [HttpPost("create_device_token")]
        public async Task<IActionResult> CreateDeviceToken([FromQuery] DevicePlatformEnum devicePlatformEnum, CreateDeviceTokenDTO createDeviceTokenDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _deviceTokenService.CreateDeviceToken(currentUser.userId, createDeviceTokenDTO, devicePlatformEnum);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("create_notification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO createNotificationDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _notificationService.CreateNotificationAsync(currentUser.userId, createNotificationDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("create_wallet")]
        public async Task<IActionResult> CreateWallet()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _walletService.CreateWalletAsync(currentUser);
            return Ok(result);
        }


        // READ

        [Authorize]
        [HttpGet("get_all_device_token")]
        public async Task<IActionResult> GetAllDeviceToken([FromQuery] PagingRequest pagingRequest, [FromQuery] DeviceTokenFilter deviceTokenFilter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService);

            var result = await _deviceTokenService.GetDeviceTokenAsync(pagingRequest, deviceTokenFilter, currentUser.userId);
            return Ok(result);
        }
    }
}
