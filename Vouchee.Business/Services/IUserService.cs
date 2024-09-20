using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services
{
    public interface IUserService
    {
        // CREATE
        public Task<Guid?> CreateUserAsync(CreateUserDTO createUserDTO);

        // READ
        public Task<GetUserDTO> GetUserByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetUserDTO>> GetUsersAsync(PagingRequest pagingRequest,
                                                                                UserFilter shopFilter,
                                                                                SortUserEnum sortUserEnum);

        // UPDATE
        public Task<bool> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDTO);

        // DELETE
        public Task<bool> DeleteUserAsync(Guid id);
    }
}
