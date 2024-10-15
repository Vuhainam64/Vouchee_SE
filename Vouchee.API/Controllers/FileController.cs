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
        private readonly IRoleService _roleService;

        public FileController(IFileUploadService fileUploadService, 
                                IUserService userService, 
                                IRoleService roleService)
        {
            _fileUploadService = fileUploadService;
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost("upload-file")]
        [Authorize]
        public async Task<IActionResult> UploadFileToFirebase(IFormFile file)
        {
            ThisUserObj thisUserObj = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _userService, _roleService);

            var result = await _fileUploadService.UploadImageToFirebase(file, thisUserObj.userId, StoragePathEnum.OTHER);
            return Ok(result);
        }
    }
}
