using System.Security.Claims;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.API.Helpers
{
    internal static class GetCurrentUserInfo
    {
        internal static async Task<ThisUserObj> GetThisUserInfo(HttpContext httpContext, 
                                                                    IUserService _userService)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = Guid.Empty;
                currentUser.email = "";
                currentUser.role = "";
                currentUser.fullName = "";
            }
            else
            {
                currentUser.userId = Guid.Parse(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value);
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.role = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
                currentUser.fullName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor).Value;
            }

            GetUserDTO? user = await _userService.GetUserByEmailAsync(currentUser.email);

            return currentUser;
        }
    }
}