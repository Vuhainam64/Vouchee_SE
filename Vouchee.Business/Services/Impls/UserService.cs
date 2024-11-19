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
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IBaseRepository<User> userRepository,
                            IMapper mapper)
        {
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

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            bool result = false;
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                result = await _userRepository.DeleteAsync(user);
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy user với id {id}");
            }
            return result;
        }

        public async Task<GetUserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
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

        public async Task<ResponseMessage<GetUserDTO>> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDTO, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(id, isTracking: true);
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
    }
}
