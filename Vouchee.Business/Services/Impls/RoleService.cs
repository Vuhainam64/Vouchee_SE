﻿using AutoMapper;
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
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IBaseRepository<Role> roleRepository,
                            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateRoleAsync(CreateRoleDTO createRoleDTO)
        {
            var role = _mapper.Map<Role>(createRoleDTO);

            role.Status = ObjectStatusEnum.ACTIVE.ToString();

            var roleId = await _roleRepository.AddAsync(role);

            return roleId;
        }

        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            var result = false;
            var role = await _roleRepository.GetByIdAsync(id);
            if (role != null)
            {
                result = await _roleRepository.DeleteAsync(role);
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy role với id {id}");
            }
            return result;
        }

        public async Task<GetRoleDTO> GetRoleByIdAsync(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role != null)
                {
                    var roleDTO = _mapper.Map<GetRoleDTO>(role);
                    return roleDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy role với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<List<GetRoleDTO>> GetRolesAsync()
        {
            try
            {
                var result = await _roleRepository.GetTable().ToListAsync();
                return _mapper.Map<List<GetRoleDTO>>(result);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<bool> UpdateRoleAsync(Guid id, UpdateRoleDTO updateRoleDTO)
        {
            var existedRole = await _roleRepository.GetByIdAsync(id);
            if (existedRole != null)
            {
                var entity = _mapper.Map<Role>(updateRoleDTO);
                return await _roleRepository.UpdateAsync(entity);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy role");
            }
        }
    }
}