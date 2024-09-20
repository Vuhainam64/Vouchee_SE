using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IAuthService
    {
        public Task<AuthResponse> GetTokenBuyer(string firebaseToken);
        public Task<AuthResponse> GetTokenSeller(string firebaseToken);
        public Task<AuthResponse> GetTokenAdmin(string firebaseToken);
    }
}
