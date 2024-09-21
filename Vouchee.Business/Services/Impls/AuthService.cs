using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;
using UnauthorizedAccessException = Vouchee.Business.Exceptions.UnauthorizedAccessException;

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

            if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())))
            {
                throw new UnauthorizedAccessException("You Are Not Using An Admin Account!!");
            }

            AuthResponse response = new()
            {
                email = email,
                uid = uid,
                id = userObject.Id,
                image = userObject.Image ?? imageUrl,
                fullName = (userObject.FirstName + "" + userObject.LastName) ?? userRecord.DisplayName
            };

            response = await GenerateTokenAsync(response, RoleEnum.ADMIN.ToString());

            return response;
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

        private async Task<AuthResponse> GenerateTokenAsync(AuthResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            Claim roleClaim, buyerId;

            if (roleCheck.Equals(RoleEnum.ADMIN.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.ADMIN.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, "");

            }
            else if (roleCheck.Equals(RoleEnum.BUYER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.BUYER.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, response.buyerId.ToString());
            }
            else if (roleCheck.Equals(RoleEnum.SELLER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.SELLER.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, "");

            }
            else if (roleCheck.Equals(RoleEnum.STAFF.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.STAFF.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, "");
            }
            else
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.BUYER.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, response.buyerId.ToString());
            }

            int hours;

            if (roleCheck.Equals(RoleEnum.ADMIN.ToString()))
            {
                hours = 8760;
            }
            else
            {
                hours = 8760;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
                   new Claim(ClaimTypes.Email, response.email),
                   new Claim(ClaimTypes.Actor, response.fullName),
                   buyerId,
                   roleClaim,
                }),

                Expires = DateTime.UtcNow.AddHours(hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            response.token = tokenHandler.WriteToken(token);
            return response;
        }
    }
}
