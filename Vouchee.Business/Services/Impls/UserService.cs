using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
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

        public UserService(IBaseRepository<MoneyRequest> moneyRequestRepository,
                           IBaseRepository<Promotion> promotionRepository,
                           IBaseRepository<Voucher> voucherRepository,
                           IBaseRepository<Wallet> walletRepository,
                           IBaseRepository<User> userRepository,
                           IMapper mapper)
        {
            _moneyRequestRepository = moneyRequestRepository;
            _promotionRepository = promotionRepository;
            _voucherRepository = voucherRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateUserAsync(CreateUserDTO createUserDTO, ThisUserObj thisUserObj)
        {
            var user = _mapper.Map<User>(createUserDTO);

            user.CreateBy = thisUserObj.userId;
            user.Status = UserStatusEnum.ACTIVE.ToString();

            var userId = await _userRepository.AddAsync(user);
            return userId;
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

        public async Task<GetUserDTO> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Carts)
                                                                                                .Include(x => x.BuyerWallet)
                                                                                                .Include(x => x.SellerWallet));
                if (user != null)
                {
                    GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);
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

        public async Task<IList<GetUserDTO>> GetUsersAsync()
        {
            IQueryable<GetUserDTO> result;
            try
            {
                result = _userRepository.GetTable()
                            .ProjectTo<GetUserDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return result.ToList();
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
