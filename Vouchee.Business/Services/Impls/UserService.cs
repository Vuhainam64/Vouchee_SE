using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IBaseRepository<Promotion> _promotionRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ISendEmailService _sendEmailService;

        public UserService(IBaseRepository<Supplier> supplierRepository,
                           IBaseRepository<MoneyRequest> moneyRequestRepository,
                           IBaseRepository<Promotion> promotionRepository,
                           IBaseRepository<Voucher> voucherRepository,
                           IBaseRepository<Wallet> walletRepository,
                           IBaseRepository<User> userRepository,
                           IMapper mapper,
                           ISendEmailService sendEmailService)
        {
            _supplierRepository = supplierRepository;
            _moneyRequestRepository = moneyRequestRepository;
            _promotionRepository = promotionRepository;
            _voucherRepository = voucherRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _sendEmailService = sendEmailService;
        }

        public async Task<ResponseMessage<bool>> BanUserAsync(Guid userId, ThisUserObj thisUserObj, bool isBan, string? reason)
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
                if (!reason.IsNullOrEmpty())
                {
                    existedUser.IsActive = false;
                    existedUser.Status = UserStatusEnum.BANNED.ToString();
                    existedUser.Description = reason;
                    existedUser.UpdateDate = DateTime.Now;
                    existedUser.UpdateBy = thisUserObj.userId;

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
                    throw new ConflictException("Phải có lý do");
                }
            }
            else
            {
                existedUser.IsActive = true;
                existedUser.Status = UserStatusEnum.NONE.ToString();
                existedUser.Description = reason;

                foreach (var voucher in existedUser.Vouchers)
                {
                    voucher.Status = VoucherStatusEnum.NONE.ToString();
                    voucher.IsActive = true;
                    voucher.UpdateDate = DateTime.Now;
                    voucher.UpdateBy = thisUserObj.userId;

                    foreach (var modal in voucher.Modals)
                    {
                        modal.Status = ModalStatusEnum.NONE.ToString();
                        voucher.IsActive = true;
                        voucher.UpdateDate = DateTime.Now;
                        voucher.UpdateBy = thisUserObj.userId;
                    }
                }

                await _sendEmailService.SendEmailAsync(existedUser.Email, "Trạng thái tài khoản", $"Tài khoản của bạn đã được gỡ ban vào lúc {existedUser.UpdateDate}");
            }

            await _userRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Đã cập nhật member thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> ChangePasswordAsync(string hashPassword, ThisUserObj thisUserObj)
        {
            // Retrieve the user from the local database
            var existedUser = await _userRepository.GetFirstOrDefaultAsync(
                x => x.Email.ToLower().Equals(thisUserObj.email.ToLower()),
                isTracking: true);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            try
            {
                // Update password in Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(thisUserObj.email);

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                {
                    Uid = userRecord.Uid,
                    Password = hashPassword // Set the new password
                });

                // Optionally update the hashPassword in your local database
                existedUser.HashPassword = hashPassword;
                existedUser.UpdateDate = DateTime.Now;
                existedUser.UpdateBy = thisUserObj.userId;

                await _sendEmailService.SendEmailAsync(existedUser.Email, "Trạng thái tài khoản", $"Tài khoản đã được thay đổi mật khẩu vào lúc {existedUser.UpdateDate}");

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

            if (createUserDTO.supplierId != null)
            {
                var existedSupplier = await _supplierRepository.GetByIdAsync(createUserDTO.supplierId);
                if (existedSupplier == null)
                {
                    throw new NotFoundException("Không tìm thấy supplỉer này");
                }
            }

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

                string roleName = user.Role.Equals(RoleEnum.ADMIN.ToString()) ? "quản trị viên" : "nhà cung cấp";
                var emailSubject = $"Tài khoản {roleName} Vouchee";

                var emailBody = $"Tài khoản email: {user.Email}\n" +
                                    $"Mật khẩu: {user.HashPassword}";
                await _sendEmailService.SendEmailAsync(createUserDTO.email, emailSubject, emailBody);

                return new ResponseMessage<Guid>
                {
                    message = "User created successfully",
                    result = true,
                    value = (Guid)result
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
            var existedUser = await _userRepository.FindAsync(id, isTracking: true);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (existedUser.Role == RoleEnum.ADMIN.ToString())
            {
                throw new ConflictException("Không thể xóa tài khoản admin");
            }

            //bool canCompletelyDelete = !(existedUser.BuyerWallet?.BuyerWalletTransactions?.Any() ?? false) &&
            //                           !(existedUser.SellerWallet?.SellerWalletTransactions?.Any() ?? false) &&
            //                           !existedUser.Orders.Any() &&
            //                           !existedUser.Orders.Any(order => order.VoucherCodes.Any()) &&
            //                           !existedUser.MoneyRequests.Any(r => r.TopUpWalletTransaction != null || r.WithdrawWalletTransaction != null) &&
            //                           !existedUser.Vouchers.SelectMany(v => v.Modals.SelectMany(m => m.OrderDetails)).Any();

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

            //try
            //{
            //    var firebaseAuth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;

            //    if (!string.IsNullOrEmpty(existedUser.Email))
            //    {
            //        var userRecord = await firebaseAuth.GetUserByEmailAsync(existedUser.Email);
            //        if (userRecord != null)
            //        {
            //            await firebaseAuth.DeleteUserAsync(userRecord.Uid);
            //        }
            //    }
            //}
            //catch (FirebaseAdmin.Auth.FirebaseAuthException ex) when (ex.Message.Contains("Failed to get user with email"))
            //{

            //}

            //if (canCompletelyDelete)
            //{
            //    await _userRepository.DeleteAsync(existedUser);
            //}
            //else
            //{
            //    existedUser.IsActive = false;
            //    existedUser.Status = UserStatusEnum.INACTIVE.ToString();
            //    existedUser.UpdateDate = DateTime.Now;
            //    existedUser.UpdateBy = thisUserObj.userId;

            //    await _userRepository.SaveChanges();
            //}

            existedUser.IsActive = false;
            existedUser.Status = UserStatusEnum.INACTIVE.ToString();
            existedUser.UpdateDate = DateTime.Now;
            existedUser.UpdateBy = thisUserObj.userId;
            existedUser.Description = "Dã xóa bởi người dùng";

            await _sendEmailService.SendEmailAsync(existedUser.Email, "Trạng thái tài khoản", $"Tài khoản của bạn đã bị xóa vào lúc {existedUser.UpdateDate} bởi người dùng");

            await _userRepository.SaveChanges();

            return new ResponseMessage<bool>
            {
                message = "Đã xóa user thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> DeleteUserFromFirebaseAsync(string email)
        {
            try
            {
                // Find the user by email
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return new ResponseMessage<bool>
                    {
                        result = false,
                        message = "User not found.",
                        value = false
                    };
                }

                // Delete the user by UID
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(user.Uid);

                return new ResponseMessage<bool>
                {
                    result = true,
                    message = "User successfully deleted.",
                    value = true
                };
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication-specific exceptions
                return new ResponseMessage<bool>
                {
                    value = false,
                    message = $"Firebase error: {ex.Message}",
                    result = false
                };
            }
            catch (Exception ex)
            {
                // Handle other general exceptions
                return new ResponseMessage<bool>
                {
                    result = false,
                    message = $"An error occurred: {ex.Message}",
                    value = false
                };
            }
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

        public async Task<string> GetUserFromFirebase(string email)
        {
            try
            {
                // Find the user by email
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return "User not found.";
                }

                // Return user details as a JSON string (or customize as needed)
                return $"User ID: {user.Uid}, Email: {user.Email}, Name: {user.DisplayName}";
            }
            catch (FirebaseAuthException ex) when (ex.Message.Contains("Failed to get user with email"))
            {
                // Handle case where email does not exist in Firebase
                return "User with the provided email does not exist in Firebase.";
            }
            catch (FirebaseAuthException ex)
            {
                // Handle other Firebase-specific exceptions
                return $"Firebase error: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return $"An error occurred: {ex.Message}";
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

        public async Task<ResponseMessage<bool>> ReactiveUserAsync(Guid userId, ThisUserObj currentUser)
        {
            var existedUser = await _userRepository.FindAsync(userId, isTracking: true);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (existedUser.IsActive == false && existedUser.Status == UserStatusEnum.INACTIVE.ToString())
            {
                throw new ConflictException("User này đã xóa trước đó");
            }

            existedUser.Description = null;
            existedUser.IsActive = true;
            existedUser.Status = UserStatusEnum.ACTIVE.ToString();
            existedUser.UpdateDate = DateTime.Now;
            existedUser.UpdateBy = currentUser.userId;

            foreach (var voucher in existedUser.Vouchers)
            {
                voucher.Status = VoucherStatusEnum.NONE.ToString();
                voucher.IsActive = true;
                voucher.UpdateBy = currentUser.userId;
                voucher.UpdateDate = DateTime.Now;

                foreach (var modal in voucher.Modals)
                {
                    modal.Status = ModalStatusEnum.NONE.ToString();
                    modal.IsActive = true;
                    modal.UpdateBy = currentUser.userId;
                    modal.UpdateDate = DateTime.Now;
                }
            }

            await _sendEmailService.SendEmailAsync(existedUser.Email, "Trạng thái tài khoản", $"Tài khoản của bạn đã được kích hoạt lại vào lúc {existedUser.UpdateDate}");

            await _userRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Kích hoạt lại user thành công",
                result = true,
                value = true
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

        public async Task<ResponseMessage<GetUserDTO>> UpdateUserBankAsync(UpdateUserBankDTO updateUserBankDTO, ThisUserObj thisUserObj, WalletTypeEnum walletTypeEnum)
        {
            var existedUser = await _userRepository.FindAsync(thisUserObj.userId, isTracking: true);

            if (existedUser != null)
            {
                // Update wallet based on wallet type
                if (walletTypeEnum == WalletTypeEnum.BUYER)
                {
                    existedUser.BuyerWallet = _mapper.Map(updateUserBankDTO, existedUser.BuyerWallet);
                }
                else if (walletTypeEnum == WalletTypeEnum.SELLER)
                {
                    existedUser.SellerWallet = _mapper.Map(updateUserBankDTO, existedUser.SellerWallet);
                }
                else
                {
                    if (existedUser.Supplier == null)
                    {
                        throw new NotFoundException("Tài khoản này không phải là tài khoản supplier");
                    }

                    existedUser.Supplier.SupplierWallet = _mapper.Map(updateUserBankDTO, existedUser.Supplier.SupplierWallet);
                }

                // Save changes to the database
                await _userRepository.SaveChanges();

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


        public async Task<ResponseMessage<GetUserDTO>> UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDTO)
        {
            if (updateUserRoleDTO.supplierId != null && updateUserRoleDTO.role.Equals(RoleEnum.SUPPLIER.ToString()))
            {
                throw new ConflictException("Role supplier cần phải có supplier");
            }

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
