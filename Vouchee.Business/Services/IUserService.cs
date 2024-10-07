using Vouchee.Business.Models;
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
        public Task<DynamicResponseModel<GetUserDTO>> GetUsersAsync(PagingRequest pagingRequest,
                                                                                UserFilter shopFilter,
                                                                                SortUserEnum sortUserEnum);

        // UPDATE
        public Task<bool> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeleteUserAsync(Guid id);
    }
}
