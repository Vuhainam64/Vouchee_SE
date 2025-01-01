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
        private readonly ISendEmailService _sendEmailService;

        private readonly IWithdrawService _withdrawService;
        private readonly ISupplierService _supplierService;
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IWalletService _walletService;
        private readonly IWalletTransactionService _walletTransactionService;

        public WeatherForecastController(ISendEmailService sendEmailService,
                                         IWithdrawService withdrawService,
                                         ISupplierService supplierService,
                                         IDeviceTokenService deviceTokenService,
                                         IUserService userService,
                                         INotificationService notificationService,
                                         IWalletService walletService,
                                         IWalletTransactionService walletTransactionService)
        {
            _sendEmailService = sendEmailService;
            _withdrawService = withdrawService;
            _supplierService = supplierService;
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

            var result = await _notificationService.CreateNotificationAsync(createNotificationDTO);
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

        [HttpPost("create_supplier_wallet")]
        public async Task<IActionResult> CreateSupplierWallet(Guid supplierId)
        {
            var result = await _supplierService.CreateSupplierWalletAsync(supplierId);
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

        [HttpGet("email")]
        public async Task<IActionResult> email()
        {
            var emailSubject = "Welcome to Our Service";
            var emailBody = "Hello, your account has been successfully created!";

            // Just await the method
            await _sendEmailService.SendEmailAsync("caothang7a7@gmail.com", emailSubject, emailBody);

            // Return a success response
            return Ok(new { message = "Email sent successfully" });
        }

        [HttpGet("get_user_from_firebase")]
        public async Task<IActionResult> GetUserFromFirebase(string email)
        {
            var result = await _userService.GetUserFromFirebase(email);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("remove_email_from_firebase")]
        public async Task<IActionResult> RemoveEmailFromFirebase(string email)
        {
            var result = await _userService.DeleteUserFromFirebaseAsync(email);
            return Ok(result);
        }

        // Endpoint to manually trigger the withdraw job
        [HttpPost("withdraw/all-wallets")]
        public async Task<IActionResult> TriggerWithdrawJob()
        {
            var response = await _withdrawService.CreateWithdrawRequestInAllWalletAsync();
            return Ok(response);
        }
    }
}
