using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;
using UnauthorizedAccessException = Vouchee.Business.Exceptions.UnauthorizedAccessException;

namespace Vouchee.Business.Services.Impls
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public AuthService(IOptions<AppSettings> appSettings, 
                            IBaseRepository<User> userRepository,
                            IBaseRepository<Wallet> walletRepository,
                            IDistributedCache cache,
                            IMapper mapper)
        {
            _walletRepository = walletRepository;
            _cache = cache;
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<AuthResponse> GetToken(string firebaseToken)
        {
            AuthResponse response = new();

            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            var imageUrl = userRecord.PhotoUrl?.ToString() ?? null;
            var phoneNumber = userRecord.PhoneNumber?.ToString() ?? null;
            string lastName = userRecord.DisplayName;

            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);

            if (userDTO == null)
            {
                User newUser = new()
                {
                    Email = email,
                    Image = imageUrl,
                    Role = RoleEnum.USER.ToString(),
                    Name = lastName,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    CreateDate = DateTime.Now,
                    BuyerWallet = new() 
                    { 
                        CreateDate = DateTime.Now,
                        Status = ObjectStatusEnum.ACTIVE.ToString(),
                    },
                    SellerWallet = new()
                    {
                        CreateDate = DateTime.Now,
                        Status = ObjectStatusEnum.ACTIVE.ToString(),
                    }
                };

                Guid? newUserId = await _userRepository.AddAsync(newUser);
                if (newUserId == null)
                {
                    throw new RegisterException("Đăng ký tài khoản mới không thành công");
                }

                //response.buyerId = newBuyerId.ToString();
                response.email = email;
                response.id = newUserId.ToString();
                response.uid = uid;
                response.image = imageUrl;
                response.phoneNumber = phoneNumber;
                response.name = userRecord.DisplayName;
                response.role = newUser.Role;
                response = await GenerateTokenAsync(response, RoleEnum.USER.ToString());
            }
            else
            {
                response.email = email;
                response.id = userDTO.id.ToString();
                response.uid = uid;
                response.image = userDTO.image ?? imageUrl;
                response.name = userDTO.name;
                response.phoneNumber = phoneNumber;
                response.role = userDTO.role;
                response = await GenerateTokenAsync(response, userDTO.role);
            }

            return response;
        }

        //public async Task<AuthResponse> LoginWithEmail(LoginByEmailDTO loginByEmailDTO)
        //{
        //    // Initialize response
        //    AuthResponse response = new();

        //    // Get the user based on the email login DTO
        //    User? user = await _userRepository(loginByEmailDTO);

        //    // Check if the user exists
        //    if (user != null)
        //    {
        //        // Populate user details into response
        //        response.id = user.Id.ToString();
        //        response.email = user.Email;
        //        response.name = user.Name;
        //        response.image = user.Image;
        //        response.phoneNumber = user.PhoneNumber;

        //        // Handle role assignment using RoleId
        //        if (user.RoleId.Equals(Guid.Parse("FF54ACC6-C4E9-4B73-A158-FD640B4B6940")))
        //        {
        //            response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString());
        //            response.roleName = RoleEnum.ADMIN.ToString();
        //        }
        //        else
        //        {
        //            response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.USER.ToString());
        //            response.roleName = RoleEnum.USER.ToString();
        //        }

        //        // Generate tokens and update response with token details and expiration times
        //        response = await GenerateTokenAsync(response, response.roleName);

        //        // Adding token expiration times
        //        //response.accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(30);  // e.g., Access token valid for 30 minutes
        //        //response.refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);     // e.g., Refresh token valid for 7 days

        //        //if (_userRepository.StoreRefreshToken(user, response.refreshToken, response.refreshTokenExpiresAt).Result == false)
        //        //{
        //        //    throw new Exception("Lỗi khi lưu trữ refresh token");
        //        //}
        //    }

        //    return response;
        //}


        //public async Task<AuthResponse> LoginWithPhoneNumber(LoginByPhoneNumberDTO loginByPhoneNumberDTO)
        //{
        //    AuthResponse response = new();

        //    User? user = await _userRepository.LoginWithPhone(loginByPhoneNumberDTO);

        //    if (user != null)
        //    {
        //        response.id = user.Id.ToString();
        //        response.email = user.Email.ToString();
        //        response.name = user.Name.ToString();
        //        response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.USER.ToString());
        //        response.roleName = RoleEnum.USER.ToString();
        //        response = await GenerateTokenAsync(response, RoleEnum.USER.ToString());
        //    }

        //    return response;
        //}

        //public async Task<AuthResponse> Register(RegisterDTO registerDTO)
        //{
        //    AuthResponse response = new();
        //    User newUser = _mapper.Map<User>(registerDTO);
        //    Guid? id = await _userRepository.AddAsync(newUser);
        //    if (id != null)
        //    {
        //        response.id = newUser.Id.ToString();
        //        response.email = newUser.Email;
        //        response.name = newUser.Name;
        //        response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.USER.ToString());
        //        response.roleName = RoleEnum.USER.ToString();
        //        response = await GenerateTokenAsync(response, RoleEnum.USER.ToString());
        //    }

        //    return response;
        //}

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

            // Access token expiration: Admins can have different expiration (example)
            //int accessTokenExpirationHours = roleCheck.Equals(RoleEnum.ADMIN.ToString()) ? 2 : 1; // Admin: 2 hours, others: 1 hour
            //int refreshTokenExpirationDays = 30; // Refresh token valid for 30 days

            // Define role claim
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
                Expires = DateTime.UtcNow.AddHours(5000), // Set access token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // Create and write the access token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.accessToken = tokenHandler.WriteToken(token);

            // Generate refresh token (using a custom method)
            //response.refreshToken = GenerateRefreshToken(response.accessToken);

            //// Add expiration times to the response (for both tokens)
            //response.accessTokenExpiresAt = DateTime.UtcNow.AddHours(accessTokenExpirationHours); // Access token expiry
            //response.refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays); // Refresh token expiry

            return response;
        }


        //private string GenerateRefreshToken(string accessToken)
        //{
        //    // Generate a random token
        //    var randomNumber = new byte[32]; // 256-bit length
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //    }

        //    // Convert the byte array to a base64 string
        //    string refreshToken = Convert.ToBase64String(randomNumber);

        //    // Optionally, you can store the refresh token along with the user identifier in the database

        //    return refreshToken;
        //}

        //public async Task<AuthResponse> Refresh(RefreshTokenRequest refreshTokenRequest)
        //{
        //    var user = await _userRepository.FindAsync(refreshTokenRequest.userId);

        //    if (user == null)
        //    {
        //        throw new NotFoundException("Không tìm thấy user");
        //    }

        //    //if (!refreshTokenRequest.refreshToken.Equals(user.RefreshToken))
        //    //{
        //    //    throw new Exception("Refresh token không trùng với cơ sở dữ liệu");
        //    //}

        //    //if (user.RefreshTokenExpirationDate < DateTime.UtcNow)
        //    //{
        //    //    throw new Exception("Refresh token hết hạn.");
        //    //}

        //    AuthResponse response = new();

        //    response.id = user.Id.ToString();
        //    response.email = user.Email;
        //    response.name = user.Name;
        //    // Handle role assignment using RoleId
        //    if (user.RoleId.Equals(Guid.Parse("FF54ACC6-C4E9-4B73-A158-FD640B4B6940")))
        //    {
        //        response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString());
        //        response.roleName = RoleEnum.ADMIN.ToString();
        //    }
        //    else if (user.RoleId.Equals(Guid.Parse("2D80393A-3A3D-495D-8DD7-F9261F85CC8F")))
        //    {
        //        response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString());
        //        response.roleName = RoleEnum.SELLER.ToString();
        //    }
        //    else
        //    {
        //        response.roleId = RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString());
        //        response.roleName = RoleEnum.BUYER.ToString();
        //    }
        //    response = await GenerateTokenAsync(response, RoleEnum.BUYER.ToString());

        //    //if (_userRepository.StoreRefreshToken(user, response.refreshToken, response.refreshTokenExpiresAt).Result == false)
        //    //{
        //    //    throw new Exception("Có lỗi khi cập nhật refresh token");
        //    //}

        //    return response;
        //}
    }
}
