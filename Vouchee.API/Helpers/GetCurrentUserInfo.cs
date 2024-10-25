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
                                                                    IUserService _userService,
                                                                    IRoleService _roleService)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = Guid.Empty;
                currentUser.email = "";
                currentUser.roleName = "";
                currentUser.fullName = "";
            }
            else
            {
                currentUser.userId = Guid.Parse(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value);
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.roleName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
                currentUser.fullName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor).Value;
            }

            var roleList = await _roleService.GetRolesAsync();
            GetUserDTO? user = await _userService.GetUserByEmailAsync(currentUser.email);

            if (user == null)
            {
                currentUser.roleId = Guid.Empty;
            }
            else
            {
                if (user.roleId != null)
                {
                    currentUser.roleId = user.roleId ?? Guid.Empty;
                }
            }

            if (roleList != null)
            {
                foreach (var role in roleList)
                {
                    if (role.name != null)
                    {
                        if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                        {
                            currentUser.adminRoleId = role.id ?? Guid.Empty;
                        }
                        if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                        {
                            currentUser.userRoleId = role.id ?? Guid.Empty;
                        }
                        if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                        {
                            currentUser.staffRoleId = role.id ?? Guid.Empty;
                        }
                    }
                }
            }
            return currentUser;
        }
    }
}