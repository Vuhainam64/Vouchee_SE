using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;

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

        // UPDATE
        public Task<bool> UpdateRoleAsync(Guid id, UpdateRoleDTO updateRoleDTO);

        // DELETE
        public Task<bool> DeleteRoleAsync(Guid id);
    }
}
