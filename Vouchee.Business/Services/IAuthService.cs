using Vouchee.Business.Models;

namespace Vouchee.Business.Services
{
    public interface IAuthService
    {
        public Task<AuthResponse> GetTokenBuyer(string firebaseToken, string deviceToken);
        public Task<AuthResponse> GetTokenSeller(string firebaseToken);
        public Task<AuthResponse> GetTokenAdmin(string firebaseToken);
    }
}