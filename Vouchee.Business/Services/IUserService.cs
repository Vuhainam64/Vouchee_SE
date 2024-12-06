﻿using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IUserService
    {
        // CREATE
        public Task<Guid?> CreateUserAsync(CreateUserDTO createUserDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetUserDTO> GetUserByEmailAsync(string email);
        public Task<GetUserDTO> GetUserByIdAsync(Guid id);
        public Task<IList<GetUserDTO>> GetUsersAsync();

        // UPDATE
        public Task<ResponseMessage<GetUserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<GetUserDTO>> UpdateUserBankAsync(UpdateUserBankDTO updateUserBankDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<GetUserDTO>> UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDTO);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteUserAsync(Guid id, ThisUserObj thisUserObj);

    }
}
