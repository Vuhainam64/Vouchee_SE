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
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using UnauthorizedAccessException = Vouchee.Business.Exceptions.UnauthorizedAccessException;

namespace Vouchee.Business.Services.Impls
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IBaseRepository<DeviceToken> _deviceTokenRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IMapper _mapper;

        public AuthService(IOptions<AppSettings> appSettings, 
                            IBaseRepository<User> userRepository,
                            IBaseRepository<Wallet> walletRepository,
                            IBaseRepository<DeviceToken> deviceTokenRepository,
                            IMapper mapper)
        {
            _walletRepository = walletRepository;
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _mapper = mapper;
            _deviceTokenRepository = deviceTokenRepository;
        }

        public async Task<AuthResponse> GetToken(string firebaseToken, string? deviceToken)
        {
            AuthResponse response = new();

            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            var imageUrl = userRecord.PhotoUrl?.ToString() ?? null;
            var phoneNumber = userRecord.PhoneNumber?.ToString() ?? null;
            string lastName = userRecord.DisplayName;

            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()), includeProperties: x => x.Include(x => x.DeviceTokens), isTracking: true);

            if (user == null)
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
                    },
                };

                if (deviceToken != null)
                {
                    newUser.DeviceTokens.Add(new()
                    {
                        CreateDate = DateTime.Now,
                        Token = deviceToken.Trim(),
                    });
                }

                Guid? newUserId = await _userRepository.AddAsync(newUser);

                response.email = email;
                response.id = newUserId.ToString();
                response.uid = uid;
                response.image = imageUrl;
                response.phoneNumber = phoneNumber;
                response.name = userRecord.DisplayName;
                response.role = newUser.Role;
                response.deviceTokens = _mapper.Map<IList<GetDeviceTokenDTO>>(newUser.DeviceTokens);
                response = await GenerateTokenAsync(response, RoleEnum.USER.ToString());
                newUser.LastAccessTime = DateTime.Now;

                await _userRepository.SaveChanges();
            }
            else
            {
                if (user.Status == UserStatusEnum.BANNED.ToString())
                {
                    throw new ConflictException($"Bạn đã bị ban vào ngày {user.UpdateDate}\n" +
                                                    $"Lý do: {user.Description}" +
                                                    $"Nếu có thắc mắc xin vui lòng liên hệ với admin qua địa chỉ adminvouchee@gmail.com để được xem xét và kích hoạt lại");
                }
                else if (user.Status == UserStatusEnum.INACTIVE.ToString())
                {
                    throw new ConflictException($"Tài khoản của bạn đã dừng hoạt động vào ngày {user.UpdateDate}." +
                                                    $"Lý do: {user.Description}" +
                                                    $"Bạn vui lòng kích hoạt lại để có thể sử dụng tiếp");
                }

                if (deviceToken != null)
                {
                    // check user nay co trung device token chua, chua thi add, co r thi k
                    var existedDeviceToken = user.DeviceTokens.FirstOrDefault(x => x.Token.Equals(deviceToken));

                    if (existedDeviceToken == null)
                    {
                        // kiem tra xem db co token nay chua, neu co r thi add cai co san vao trong user, neu chua co thi tao moi
                        existedDeviceToken = await _deviceTokenRepository.GetFirstOrDefaultAsync(x => x.Token.Equals(deviceToken));

                        if (existedDeviceToken == null)
                        {
                            user.DeviceTokens.Add(new()
                            {
                                CreateDate = DateTime.Now,
                                Token = deviceToken,
                            });
                        }
                        else
                        {
                            user.DeviceTokens.Add(existedDeviceToken);
                        }
                    }
                }

                response.email = email;
                response.id = user.Id.ToString();
                response.uid = uid;
                response.image = user.Image ?? imageUrl;
                response.name = user.Name;
                response.phoneNumber = user.PhoneNumber;
                response.role = user.Role;
                response.deviceTokens = _mapper.Map<IList<GetDeviceTokenDTO>>(user.DeviceTokens);
                response = await GenerateTokenAsync(response, user.Role);
                user.LastAccessTime = DateTime.Now;

                await _userRepository.SaveChanges();
            }

            return response;
        }

        private async Task<AuthResponse> GenerateTokenAsync(AuthResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

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

            return response;
        }
    }
}
