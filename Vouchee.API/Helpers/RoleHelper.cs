using Vouchee.Business.Models;

namespace Vouchee.API.Helpers
{
    public static class RoleHelper
    {
        // Helper method to check user roles
        public static bool IsBuyer(ThisUserObj currentUser) =>
            currentUser.roleId.Equals(currentUser.buyerRoleId);

        public static bool IsSeller(ThisUserObj currentUser) =>
            currentUser.roleId.Equals(currentUser.sellerRoleId);
    }
}
