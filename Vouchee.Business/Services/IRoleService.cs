using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IRoleService
    {
        // CREATE
        public Task<Guid?> CreateRoleAsync(CreateRoleDTO createRoleDTo);

        // READ
        public Task<GetRoleDTO> GetRoleByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetRoleDTO>> GetRolesAsync(PagingRequest pagingRequest,
                                                                            RoleFilter roleFilter,
                                                                            SortRoleEnum sortRoleEnum);
        public Task<List<GetRoleDTO>> GetRolesAsync();

        // UPDATE
        public Task<bool> UpdateRoleAsync(Guid id, UpdateRoleDTO updateRoleDTO);

        // DELETE
        public Task<bool> DeleteRoleAsync(Guid id);
    }
}
