using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/file")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class FileController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IUserService _userService;

        public FileController(IFileUploadService fileUploadService, 
                                IUserService userService)
        {
            _fileUploadService = fileUploadService;
            _userService = userService;
        }

        //[HttpPost("upload-image")]
        //[Authorize]
        //public async Task<IActionResult> UploadImageToFirebase(IFormFile file)
        //{
        //    ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var result = await _fileUploadService.UploadImageToFirebase(file, thisUserObj.userId.ToString(), StoragePathEnum.OTHER);
        //    return Ok(result);
        //}

        //[HttpPost("upload-video")]
        //[Authorize]
        //public async Task<IActionResult> UploadVideoToFirebase(IFormFile file)
        //{
        //    ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var result = await _fileUploadService.UploadVideoToFirebase(file, thisUserObj.userId.ToString(), StoragePathEnum.OTHER);
        //    return Ok(result);
        //}

        //[HttpPost("upload-file")]
        //[Authorize]
        //public async Task<IActionResult> UploadFileToFirebase(IFormFile file)
        //{
        //    ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

        //    var result = await _fileUploadService.UploadFileToFirebase(file, thisUserObj.userId.ToString(), StoragePathEnum.OTHER);
        //    return Ok(result);
        //}
    }
}
