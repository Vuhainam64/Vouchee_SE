using AutoMapper;
using AutoMapper.QueryableExtensions;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, 
                            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateUserAsync(CreateUserDTO createUserDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var user = _mapper.Map<User>(createUserDTO);

                user.Status = UserStatusEnum.ACTIVE.ToString();

                var userId = await _userRepository.AddAsync(user);
                return userId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo user");
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa user");
            }
        }

        public async Task<GetUserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
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
                var user = await _userRepository.GetByIdAsync(id);
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
                throw new LoadException("Lỗi không xác định khi tải user");
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
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return result.ToList();
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var existedUser = await _userRepository.GetByIdAsync(id);
                if (existedUser != null)
                {
                    var entity = _mapper.Map<User>(updateUserDTO);
                    return await _userRepository.UpdateAsync(entity);
                }
                else
                {
                    throw new NotFoundException("Không tìm thấy user");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật user");
            }
        }
    }
}
