﻿using AutoMapper;
using Azure.Core;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.RedisCache;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;
using UnauthorizedAccessException = Vouchee.Business.Exceptions.UnauthorizedAccessException;

namespace Vouchee.Business.Services.Impls
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public AuthService(IOptions<AppSettings> appSettings, 
                            IUserRepository userRepository,
                            IDistributedCache cache,
                            IMapper mapper,
                            IRoleRepository roleRepository)
        {
            _cache = cache;
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<AuthResponse> GetToken(string firebaseToken)
        {
            AuthResponse response = new();

            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string imageUrl = userRecord.PhotoUrl.ToString();
            string lastName = userRecord.DisplayName;

            var user = await _userRepository.GetUserByEmail(email);
            GetUserDTO useDTO = _mapper.Map<GetUserDTO>(user);

            // trường hợp người dùng mới, mặc định là buyer
            if (useDTO == null)
            {
                Guid roleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString()));

                User newBuyer = new()
                {
                    Email = email,
                    Image = imageUrl,
                    RoleId = roleId,
                    Name = lastName,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    CreateDate = DateTime.Now
                };

                Guid? newBuyerId = await _userRepository.AddAsync(newBuyer);
                if (newBuyerId == null)
                {
                    throw new RegisterException("Đăng ký người mua mới không thành công");
                }

                //response.buyerId = newBuyerId.ToString();
                response.email = email;
                response.id = newBuyerId.ToString();
                response.uid = uid;
                response.image = imageUrl;
                response.name = userRecord.DisplayName;
                response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
            }
            else
            {
                response.email = email;
                response.id = useDTO.id.ToString();
                response.uid = uid;
                response.image = useDTO.image ?? imageUrl;
                response.roleId = useDTO.roleId.ToString();
                response.roleName = useDTO.roleName;
                response.name = useDTO.name;
                response = await GenerateTokenAsync(response, useDTO.roleName);
            }

            return response;
        }

        public async Task<AuthResponse> LoginWithEmail(LoginByEmailDTO loginByEmailDTO)
        {
            AuthResponse response = new();

            User? user = await _userRepository.LoginWithEmail(loginByEmailDTO);

            if (user != null)
            {
                response.id = user.Id.ToString();
                response.email = user.Email.ToString();
                response.name = user.Name.ToString();
                if (user.RoleId.Equals(Guid.Parse("FF54ACC6-C4E9-4B73-A158-FD640B4B6940")))
                {
                    response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString());
                    response.roleName = RoleEnum.ADMIN.ToString();
                }
                else if (user.RoleId.Equals(Guid.Parse("2D80393A-3A3D-495D-8DD7-F9261F85CC8F")))
                {
                    response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString());
                    response.roleName = RoleEnum.SELLER.ToString();
                }
                else
                {
                    response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString());
                    response.roleName = RoleEnum.BUYER.ToString();
                }
                response.image = user.Image;
                response.phoneNumber = user.PhoneNumber;
                response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
            }

            return response;
        }

        public async Task<AuthResponse> LoginWithPhoneNumber(LoginByPhoneNumberDTO loginByPhoneNumberDTO)
        {
            AuthResponse response = new();

            User? user = await _userRepository.LoginWithPhone(loginByPhoneNumberDTO);

            if (user != null)
            {
                response.id = user.Id.ToString();
                response.email = user.Email.ToString();
                response.name = user.Name.ToString();
                response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString());
                response.roleName = RoleEnum.BUYER.ToString();
                response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
            }

            return response;
        }

        public async Task<AuthResponse> Register(RegisterDTO registerDTO)
        {
            AuthResponse response = new();
            User newUser = _mapper.Map<User>(registerDTO);
            Guid? id = await _userRepository.AddAsync(newUser);
            if (id != null)
            {
                response.id = newUser.Id.ToString();
                response.email = newUser.Email;
                response.name = newUser.Name;
                response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString());
                response.roleName = RoleEnum.BUYER.ToString();
                response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
            }

            return response;
        }

        //public async Task<AuthResponse> GetTokenAdmin(string firebaseToken)
        //{
        //    FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
        //    string uid = decryptedToken.Uid;

        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    string email = userRecord.Email;
        //    string imageUrl = userRecord.PhotoUrl.ToString();

        //    User userObject = await _userRepository.GetUserByEmail(email);

        //    if (userRecord == null)
        //        throw new NotFoundException("User not found");

        //    if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())))
        //    {
        //        throw new UnauthorizedAccessException("Đây không phải tài khoản admin!!");
        //    }

        //    AuthResponse response = new()
        //    {
        //        roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString()),
        //        roleName = RoleEnum.ADMIN.ToString(),
        //        email = email,
        //        uid = uid,
        //        id = userObject.Id.ToString(),
        //        image = userObject.Image ?? imageUrl,
        //        fullName = (userObject.FirstName + "" + userObject.LastName) ?? userRecord.DisplayName
        //    };

        //    response = await GenerateTokenAsync(response, RoleEnum.ADMIN.ToString());

        //    return response;
        //}

        //public async Task<AuthResponse> GetTokenBuyer(string firebaseToken, string deviceToken)
        //{
        //    deviceToken ??= "";
        //    FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
        //    string uid = decryptedToken.Uid;

        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    string email = userRecord.Email;
        //    string lastName = userRecord.DisplayName;
        //    string ImageUrl = userRecord.PhotoUrl.ToString();

        //    User userObject = await _userRepository.GetUserByEmail(email);

        //    AuthResponse response = new();

        //    if (userObject == null)
        //    {
        //        Guid roleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString()));

        //        User newBuyer = new()
        //        {
        //            Email = email,
        //            Image = ImageUrl,
        //            RoleId = roleId,
        //            LastName = lastName,
        //            Status = ObjectStatusEnum.ACTIVE.ToString(),
        //            CreateDate = DateTime.Now
        //        };

        //        Guid? newBuyerId = await _userRepository.AddAsync(newBuyer);
        //        if (newBuyerId == null)
        //        {
        //            throw new RegisterException("Đăng ký người mua mới không thành công");
        //        }

        //        if (!deviceToken.Equals(""))
        //        {
        //            List<string> newtokens = new()
        //            {
        //                deviceToken
        //            };

        //            newtokens = await DeviceTokenCache.ValidateDeviceToken(newtokens);
        //            if (newtokens.Count > 0)
        //                await DeviceTokenCache.UpdateDeviceToken(_cache, newBuyerId.ToString(), newtokens.Last());
        //        }

        //        response.buyerId = newBuyerId.ToString();
        //        response.email = email;
        //        response.id = newBuyerId.ToString();
        //        response.uid = uid;
        //        response.image = ImageUrl;
        //        response.fullName = userRecord.DisplayName;
        //        response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
        //    }
        //    else
        //    {
        //        if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString())))
        //        {
        //            throw new RegisterException("Đây không phải gmail của người mua");
        //        }

        //        if (!deviceToken.Equals(""))
        //        {
        //            List<string> newtokens = new()
        //            {
        //                deviceToken
        //            };

        //            newtokens = await DeviceTokenCache.ValidateDeviceToken(newtokens);
        //            if (newtokens.Count > 0)
        //                await DeviceTokenCache.UpdateDeviceToken(_cache, userObject.Id.ToString(), newtokens.Last());
        //        }

        //        response.roleId = userObject.RoleId.ToString();
        //        response.roleName = RoleEnum.BUYER.ToString();
        //        response.buyerId = userObject.Id.ToString();
        //        response.email = email;
        //        response.id = userObject.Id.ToString();
        //        response.uid = uid;
        //        response.image = userObject.Image;
        //        response.fullName = (userObject.FirstName + " " + userObject.LastName) ?? userRecord.DisplayName;
        //        response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());
        //    }
        //    return response;
        //}

        //public async Task<AuthResponse> GetTokenSeller(string firebaseToken)
        //{
        //    FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
        //    string uid = decryptedToken.Uid;

        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    string email = userRecord.Email;
        //    string ImageUrl = userRecord.PhotoUrl.ToString();

        //    User userObject = await _userRepository.GetUserByEmail(email);

        //    AuthResponse response = new();

        //    if (userObject == null)
        //    {
        //        throw new NotFoundException("User Not Found!!");
        //    }

        //    if (userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString())))
        //    {
        //        response.email = email;
        //        response.id = userObject.Id.ToString();
        //        response.uid = uid;
        //        response.image = userObject.Image ?? ImageUrl;
        //        response.roleId = userObject.RoleId.ToString();
        //        response.roleName = RoleEnum.SELLER.ToString();
        //        response.fullName = userObject.FirstName + " " + userObject.LastName;
        //        response = await GenerateTokenAsync(response, RoleEnum.SELLER.ToString());
        //    }
        //    else
        //    {
        //        throw new UnauthorizedAccessException("Đây không phải tài khoản của người bán!");
        //    }
        //    return response;
        //}

        //public Task Logout(string userId, string deviceToken)
        //{
        //    throw new NotImplementedException();
        //}

        private async Task<AuthResponse> GenerateTokenAsync(AuthResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            int accessTokenExpirationHours = roleCheck.Equals(RoleEnum.ADMIN.ToString()) ? 1 : 1; // Access token valid for 1 hour
            int refreshTokenExpirationDays = 30; // Refresh token valid for 30 days

            Claim roleClaim = new Claim(ClaimTypes.Role, roleCheck);

            // Access token claims
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
                    new Claim(ClaimTypes.Email, response.email),
                    new Claim(ClaimTypes.Actor, response.name),
                    roleClaim
                }),
                Expires = DateTime.UtcNow.AddHours(accessTokenExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // Create and write the access token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.accessToken = tokenHandler.WriteToken(token);
            response.refreshToken = GenerateRefreshToken(response.accessToken);

            return response;
        }

        private string GenerateRefreshToken(string accessToken)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_appSettings.Secret)))
            {
                var tokenBytes = Encoding.UTF8.GetBytes(accessToken);
                var hashBytes = hmac.ComputeHash(tokenBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
