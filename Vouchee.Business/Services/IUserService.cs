using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IUserService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateUserAsync(CreateUserDTO createUserDTO, string deviceToken);

        // READ
        public Task<GetUserDTO> GetUserByEmailAsync(string email);
        public Task<GetDetailUserDTO> GetUserByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetUserDTO>> GetUsersAsync(PagingRequest pagingRequest, UserFilter userFilter);

        // UPDATE
        public Task<ResponseMessage<GetUserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<GetUserDTO>> UpdateUserBankAsync(UpdateUserBankDTO updateUserBankDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<GetUserDTO>> UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDTO);
        public Task<ResponseMessage<bool>> BanUserAsync(Guid userId, ThisUserObj thisUserObj, bool isBan, string reason);
        public Task<ResponseMessage<bool>> ChangePasswordAsync(string email, string hashPassword);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteUserAsync(Guid id, ThisUserObj thisUserObj);

    }
}
