using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;

namespace Vouchee.Business.Services.Impls
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthService(IOptions<AppSettings> appSettings, 
                            IUserService userService, 
                            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
            _mapper = mapper;
        }

        public Task<AuthResponse> GetTokenAdmin(string firebaseToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> GetTokenBuyer(string firebaseToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> GetTokenSeller(string firebaseToken)
        {
            throw new NotImplementedException();
        }

        public Task Logout(string userId, string deviceToken)
        {
            throw new NotImplementedException();
        }
    }
}
