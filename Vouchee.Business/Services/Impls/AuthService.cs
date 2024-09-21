using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthService(IOptions<AppSettings> appSettings, 
                            IUserRepository userRepository, 
                            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<AuthResponse> GetTokenAdmin(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string imageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);
            AuthResponse authResponse = new();

            if (userRecord == null)
                throw new NotFoundException("User not found");

            return null;
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
