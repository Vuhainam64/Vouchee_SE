﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.Constants.String;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Entities;
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

        public async Task<Guid?> CreateUserAsync(CreateUserDTO createUserDTO)
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

        public async Task<DynamicResponseModel<GetUserDTO>> GetUsersAsync(PagingRequest pagingRequest,
                                                                            UserFilter userFilter,
                                                                            SortUserEnum sortUserEnum)
        {
            (int, IQueryable<GetUserDTO>) result;
            try
            {
                result = _userRepository.GetTable()
                            .ProjectTo<GetUserDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetUserDTO>(userFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LimitPaging, PageConstant.DefaultPaging);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return new DynamicResponseModel<GetUserDTO>()
            {
                Metadata = new PagingMetadata()
                {
                    Page = pagingRequest.page,
                    Size = pagingRequest.pageSize,
                    Total = result.Item1
                },
                Results = result.Item2.ToList()
            };
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDTO)
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
