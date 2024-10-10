using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IAuthService
    {
        //public Task<AuthResponse> GetTokenBuyer(string firebaseToken, string deviceToken);
        //public Task<AuthResponse> GetTokenSeller(string firebaseToken);
        //public Task<AuthResponse> GetTokenAdmin(string firebaseToken);
        public Task<AuthResponse> GetToken(string firebaseToken);

        public Task<AuthResponse> LoginWithPhoneNumber(LoginByPhoneNumberDTO loginByPhoneNumberDTO);

        public Task<AuthResponse> RegisterWithPhoneNumber(RegisterDTO registerDTO);

        public Task<AuthResponse> LoginWithEmail(LoginByEmailDTO loginByEmailDTO);
    }
}