using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IAuthService
    {
        public Task<AuthResponse> GetToken(string firebaseToken , string? deviceToken);
    }
}