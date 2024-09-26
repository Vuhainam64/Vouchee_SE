using System.Security.Claims;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.API.Helpers
{
    internal static class GetCurrentUserInfo
    {

        internal static async Task<ThisUserObj> GetThisUserInfo(HttpContext httpContext, IUserService _userService)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.buyerId = "";
                currentUser.roleName = "";
                currentUser.fullName = "";
            }
            else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.buyerId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
                currentUser.roleName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
                currentUser.fullName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor).Value;
            }

            return currentUser;
        }
    }
}