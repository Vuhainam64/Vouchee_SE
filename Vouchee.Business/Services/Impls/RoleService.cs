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
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateRoleAsync(CreateRoleDTO createRoleDTO)
        {
            try
            {
                var role = _mapper.Map<Role>(createRoleDTO);

                role.Status = ObjectStatusEnum.ACTIVE.ToString();

                var roleId = await _roleRepository.AddAsync(role);

                return roleId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo id");
            }
        }

        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa role");
            }
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
                throw new LoadException("Lỗi không xác định khi tải role");
            }
        }

        public async Task<DynamicResponseModel<GetRoleDTO>> GetRolesAsync(PagingRequest pagingRequest,
                                                                                    RoleFilter roleFilter,
                                                                                    SortRoleEnum sortRoleEnum)
        {
            (int, IQueryable<GetRoleDTO>) result;
            try
            {
                result = _roleRepository.GetTable()
                            .ProjectTo<GetRoleDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetRoleDTO>(roleFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LimitPaging, PageConstant.DefaultPaging);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải role");
            }
            return new DynamicResponseModel<GetRoleDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1
                },
                results = result.Item2.ToList()
            };
        }

        public async Task<bool> UpdateRoleAsync(Guid id, UpdateRoleDTO updateRoleDTO)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật role");
            }
        }
    }
}