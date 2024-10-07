using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Services.Extensions.RedisCache;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
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
        private readonly IDistributedCache _cache;

        public AuthService(IOptions<AppSettings> appSettings, 
                            IUserRepository userRepository,
                            IDistributedCache cache,
                            IMapper mapper)
        {
            _cache = cache;
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

            if (userRecord == null)
                throw new NotFoundException("User not found");

            if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())))
            {
                throw new UnauthorizedAccessException("Đây không phải tài khoản admin!!");
            }

            AuthResponse response = new()
            {
                roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString()),
                roleName = RoleEnum.ADMIN.ToString(),
                email = email,
                uid = uid,
                id = userObject.Id.ToString(),
                image = userObject.Image ?? imageUrl,
                fullName = (userObject.FirstName + "" + userObject.LastName) ?? userRecord.DisplayName
            };

            response = await GenerateTokenAsync(response, RoleEnum.ADMIN.ToString());

            return response;
        }

        public async Task<AuthResponse> GetTokenBuyer(string firebaseToken, string deviceToken)
        {
            deviceToken ??= "";
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string lastName = userRecord.DisplayName;
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);

            AuthResponse response = new();

            if (userObject == null)
            {
                Guid roleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString()));

                User newBuyer = new()
                {
                    Email = email,
                    Image = ImageUrl,
                    RoleId = roleId,
                    LastName = lastName,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    CreateDate = DateTime.Now
                };

                Guid? newBuyerId = await _userRepository.AddAsync(newBuyer);
                if (newBuyerId == null)
                {
                    throw new RegisterException("Đăng ký người mua mới không thành công");
                }

                if (!deviceToken.Equals(""))
                {
                    List<string> newtokens = new()
                    {
                        deviceToken
                    };

                    newtokens = await DeviceTokenCache.ValidateDeviceToken(newtokens);
                    if (newtokens.Count > 0)
                        await DeviceTokenCache.UpdateDeviceToken(_cache, newBuyerId.ToString(), newtokens.Last());
                }

                response.buyerId = newBuyerId.ToString();
                response.email = email;
                response.id = newBuyerId.ToString();
                response.uid = uid;
                response.image = ImageUrl;
                response.fullName = userRecord.DisplayName;
                response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
            }
            else
            {
                if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString())))
                {
                    throw new RegisterException("Đây không phải gmail của người mua");
                }

                if (!deviceToken.Equals(""))
                {
                    List<string> newtokens = new()
                    {
                        deviceToken
                    };

                    newtokens = await DeviceTokenCache.ValidateDeviceToken(newtokens);
                    if (newtokens.Count > 0)
                        await DeviceTokenCache.UpdateDeviceToken(_cache, userObject.Id.ToString(), newtokens.Last());
                }

                response.roleId = userObject.RoleId.ToString();
                response.roleName = RoleEnum.BUYER.ToString();
                response.buyerId = userObject.Id.ToString();
                response.email = email;
                response.id = userObject.Id.ToString();
                response.uid = uid;
                response.image = userObject.Image;
                response.fullName = (userObject.FirstName + " " + userObject.LastName) ?? userRecord.DisplayName;
                response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
            }
            return response;
        }

        public async Task<AuthResponse> GetTokenSeller(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);

            AuthResponse response = new();

            if (userObject == null)
            {
                throw new NotFoundException("User Not Found!!");
            }

            if (userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString())))
            {
                response.email = email;
                response.id = userObject.Id.ToString();
                response.uid = uid;
                response.image = userObject.Image ?? ImageUrl;
                response.roleId = userObject.RoleId.ToString();
                response.roleName = RoleEnum.SELLER.ToString();
                response.fullName = userObject.FirstName + " " + userObject.LastName;
                response = await GenerateTokenAsync(response, RoleEnum.SELLER.ToString());
            }
            else
            {
                throw new UnauthorizedAccessException("Đây không phải tài khoản của người bán!");
            }
            return response;
        }

        public Task Logout(string userId, string deviceToken)
        {
            throw new NotImplementedException();
        }

        private async Task<AuthResponse> GenerateTokenAsync(AuthResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            int hours = roleCheck.Equals(RoleEnum.ADMIN.ToString()) ? 8760 : 8760;

            Claim roleClaim, buyerId;

            if (roleCheck.Equals(RoleEnum.ADMIN.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.ADMIN.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, "");
            }
            else if (roleCheck.Equals(RoleEnum.BUYER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.BUYER.ToString());
                buyerId = new Claim(ClaimTypes.GroupSid, response.buyerId);
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
                buyerId = new Claim(ClaimTypes.GroupSid, response.buyerId);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
                    new Claim(ClaimTypes.Email, response.email),
                    new Claim(ClaimTypes.Actor, response.fullName),
                    roleClaim,
                    buyerId
                }),

                Expires = DateTime.UtcNow.AddHours(hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // Create and write the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.token = tokenHandler.WriteToken(token);

            return response;

        }
    }
}
