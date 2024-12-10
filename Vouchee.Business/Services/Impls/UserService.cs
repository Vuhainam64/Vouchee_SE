using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IBaseRepository<Promotion> _promotionRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ISendEmailService _sendEmailService;

        public UserService(IBaseRepository<MoneyRequest> moneyRequestRepository,
                           IBaseRepository<Promotion> promotionRepository,
                           IBaseRepository<Voucher> voucherRepository,
                           IBaseRepository<Wallet> walletRepository,
                           IBaseRepository<User> userRepository,
                           IMapper mapper,
                           ISendEmailService sendEmailService)
        {
            _moneyRequestRepository = moneyRequestRepository;
            _promotionRepository = promotionRepository;
            _voucherRepository = voucherRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _sendEmailService = sendEmailService;
        }

        public async Task<ResponseMessage<bool>> BanUserAsync(Guid userId, ThisUserObj thisUserObj, bool isBan, string reason)
        {
            var existedUser = await _userRepository.GetByIdAsync(userId, isTracking: true);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tim thấy user này");
            }

            existedUser.UpdateDate = DateTime.Now;
            existedUser.UpdateBy = thisUserObj.userId;

            if (isBan)
            {
                existedUser.IsActive = false;
                existedUser.Status = UserStatusEnum.BANNED.ToString();
                existedUser.Description = reason;

                foreach (var voucher in existedUser.Vouchers)
                {
                    voucher.Status = VoucherStatusEnum.INACTIVE_BY_BANNED_USER.ToString();
                    voucher.IsActive = false;
                    voucher.UpdateDate = DateTime.Now;
                    voucher.UpdateBy = thisUserObj.userId;

                    foreach (var modal in voucher.Modals)
                    {
                        modal.Status = ModalStatusEnum.INACTIVE_BY_BANNED_USER.ToString();
                        voucher.IsActive = false;
                        voucher.UpdateDate = DateTime.Now;
                        voucher.UpdateBy = thisUserObj.userId;
                    }
                }
            }
            else
            {
                existedUser.IsActive = true;
                existedUser.Status = UserStatusEnum.NONE.ToString();
                existedUser.Description = null;

                foreach (var voucher in existedUser.Vouchers)
                {
                    voucher.Status = VoucherStatusEnum.NONE.ToString();
                    voucher.IsActive = true;
                    voucher.UpdateDate = DateTime.Now;
                    voucher.UpdateBy = thisUserObj.userId;

                    foreach (var modal in voucher.Modals)
                    {
                        modal.Status = ModalStatusEnum.INACTIVE_BY_BANNED_USER.ToString();
                        voucher.IsActive = true;
                        voucher.UpdateDate = DateTime.Now;
                        voucher.UpdateBy = thisUserObj.userId;
                    }
                }
            }

            await _userRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Đã cập nhật member thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> ChangePasswordAsync(string email, string hashPassword)
        {
            // Retrieve the user from the local database
            var existedUser = await _userRepository.GetFirstOrDefaultAsync(
                x => x.Email.ToLower().Equals(email.ToLower()),
                isTracking: true);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            try
            {
                // Update password in Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = userRecord.Uid,
                    Password = hashPassword // Set the new password
                });

                // Optionally update the hashPassword in your local database
                existedUser.HashPassword = hashPassword;
                existedUser.UpdateDate = DateTime.Now;

                await _userRepository.UpdateAsync(existedUser);

                return new ResponseMessage<bool>
                {
                    message = "Cập nhật mật khẩu thành công",
                    result = true,
                    value = true
                };
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Firebase error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<Guid>> CreateUserAsync(CreateUserDTO createUserDTO, string? deviceToken)
        {
            var existedUser = await _userRepository.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(createUserDTO.email.ToLower()));

            if (existedUser != null)
            {
                throw new ConflictException("Email này đã được dùng");
            }
            
            // Map the DTO to the user entity
            var user = _mapper.Map<User>(createUserDTO);

            // Default values for user
            user.Status = UserStatusEnum.ACTIVE.ToString();
            user.BuyerWallet = new()
            {
                CreateDate = DateTime.Now,
                Status = ObjectStatusEnum.ACTIVE.ToString(),
            };
            user.SellerWallet = new()
            {
                CreateDate = DateTime.Now,
                Status = ObjectStatusEnum.ACTIVE.ToString(),
            };

            try
            {
                // Create the user in Firebase Authentication
                var firebaseUser = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Email = createUserDTO.email,
                    Password = createUserDTO.hashPassword,
                    EmailVerified = false,
                    Disabled = false
                });

                // Optionally store device token in your system
                if (!string.IsNullOrEmpty(deviceToken))
                {
                    user.DeviceTokens.Add(new()
                    {
                        CreateDate = DateTime.UtcNow,
                        Token = deviceToken,
                    });
                }

                // Save the user in your database
                var result = await _userRepository.AddAsync(user);
                var emailSubject = "Welcome to Our Service";
                var emailBody = $"Hello {createUserDTO.email},\n\nYour account has been successfully created!";
                await _sendEmailService.SendEmailAsync(createUserDTO.email, emailSubject, emailBody);
                return new ResponseMessage<Guid>
                {
                    message = "User created successfully",
                    result = true,
                    value = (Guid) result
                };
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase Authentication errors
                return new ResponseMessage<Guid>
                {
                    message = $"Firebase error: {ex.Message}",
                    result = false,
                    value = Guid.Empty
                };
            }
            catch (Exception ex)
            {
                // Handle general errors
                return new ResponseMessage<Guid>
                {
                    message = $"Error: {ex.Message}",
                    result = false,
                    value = Guid.Empty
                };
            }
        }

        // giữ lại các transaction như order, wallet
        public async Task<ResponseMessage<bool>> DeleteUserAsync(Guid id, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.FindAsync(id);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            bool canCompletelyDelete = !(existedUser.BuyerWallet?.BuyerWalletTransactions?.Any() ?? false) &&
                                           !(existedUser.SellerWallet?.SellerWalletTransactions?.Any() ?? false) &&
                                           !existedUser.Orders.Any() &&
                                           !existedUser.Orders.Any(order => order.VoucherCodes.Any()) &&
                                           !existedUser.MoneyRequests.Any(r => r.TopUpWalletTransaction != null || r.WithdrawWalletTransaction != null) &&
                                           !existedUser.Vouchers.SelectMany(v => v.Modals.SelectMany(m => m.OrderDetails)).Any();

            if (existedUser.Vouchers != null)
            {
                foreach (var voucher in existedUser.Vouchers)
                {
                    voucher.IsActive = false;
                    voucher.Status = VoucherStatusEnum.DELETED.ToString();
                    voucher.UpdateDate = DateTime.Now;
                    voucher.UpdateBy = thisUserObj.userId;

                    foreach (var modal in voucher.Modals)
                    {
                        modal.IsActive = false;
                        modal.Status = VoucherStatusEnum.DELETED.ToString();
                        modal.UpdateBy = thisUserObj.userId;
                        modal.UpdateDate = DateTime.Now;
                    }
                }
            }

            if (canCompletelyDelete)
            {
                await _userRepository.DeleteAsync(existedUser);
            }
            else
            {
                existedUser.IsActive = false;
                existedUser.Status = UserStatusEnum.INACTIVE.ToString();
                existedUser.UpdateDate = DateTime.Now;
                existedUser.UpdateBy = thisUserObj.userId;

                await _userRepository.SaveChanges();
            }

            return new ResponseMessage<bool>
            {
                message = "Đã xóa user thành công",
                result = true,
                value = true
            };
        }

        public async Task<GetUserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()), 
                                                                            includeProperties: x => x.Include(x => x.Carts)
                                                                                                .Include(x => x.BuyerWallet)
                                                                                                .Include(x => x.SellerWallet));
            if (user != null)
            {
                GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);
                return userDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy user với email {email}");
            }
        }

        public async Task<GetDetailUserDTO> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.FindAsync(id);
                if (user != null)
                {
                    GetDetailUserDTO userDTO = _mapper.Map<GetDetailUserDTO>(user);
                    return userDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy user với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<DynamicResponseModel<GetUserDTO>> GetUsersAsync(PagingRequest pagingRequest, UserFilter userFilter)
        {
            (int, IQueryable<GetUserDTO>) result;

            result = _userRepository.GetTable()
                        .ProjectTo<GetUserDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetUserDTO>(userFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetUserDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<ResponseMessage<GetUserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, isTracking: true);
            if (existedUser != null)
            {
                var entity = _mapper.Map(updateUserDTO, existedUser);
                entity.UpdateBy = thisUserObj.userId;
                await _userRepository.UpdateAsync(entity);

                return new ResponseMessage<GetUserDTO>()
                {
                    message = "Cập nhật thành công",
                    result = true,
                    value = _mapper.Map<GetUserDTO>(entity)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy user");
            }
        }

        public async Task<ResponseMessage<GetUserDTO>> UpdateUserBankAsync(UpdateUserBankDTO updateUserBankDTO, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, isTracking: true);
            if (existedUser != null)
            {
                var entity = _mapper.Map(updateUserBankDTO, existedUser);
                entity.UpdateBy = thisUserObj.userId;
                await _userRepository.UpdateAsync(entity);

                return new ResponseMessage<GetUserDTO>()
                {
                    message = "Cập nhật thành công",
                    result = true,
                    value = _mapper.Map<GetUserDTO>(entity)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy user");
            }
        }

        public async Task<ResponseMessage<GetUserDTO>> UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDTO)
        {
            var existedUser = await _userRepository.GetByIdAsync(updateUserRoleDTO.userId, isTracking: true);
            if (existedUser != null)
            {
                existedUser.Role = updateUserRoleDTO.role.ToString();
                existedUser.UpdateBy = updateUserRoleDTO.userId;
                existedUser.UpdateDate = updateUserRoleDTO.updateDate;
                await _userRepository.UpdateAsync(existedUser);

                return new ResponseMessage<GetUserDTO>()
                {
                    message = "Cập nhật thành công",
                    result = true,
                    value = _mapper.Map<GetUserDTO>(existedUser)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy user");
            }
        }
    }
}
