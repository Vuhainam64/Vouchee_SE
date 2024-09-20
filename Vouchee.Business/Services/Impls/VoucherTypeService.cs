using AutoMapper;
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
    public class VoucherTypeService : IVoucherTypeService
    {
        private readonly IVoucherTypeRepository _voucherTypeRepository;
        private readonly IMapper _mapper;

        public VoucherTypeService(IVoucherTypeRepository voucherTypeService, 
                                    IMapper mapper)
        {
            _voucherTypeRepository = voucherTypeService;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateVoucherTypeAsync(CreateVoucherTypeDTO createVoucherTypeDTO)
        {
            try
            {
                var voucherType = _mapper.Map<VoucherType>(createVoucherTypeDTO);
                var voucherTypeId = await _voucherTypeRepository.AddAsync(voucherType);
                return voucherTypeId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo voucher type");
            }
        }

        public async Task<bool> DeleteVoucherTypeAsync(Guid id)
        {
            try
            {
                bool result = false;
                var voucherType = await _voucherTypeRepository.GetByIdAsync(id);
                if (voucherType != null)
                {
                    result = await _voucherTypeRepository.DeleteAsync(voucherType);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy voucher type với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa voucher type");
            }
        }

        public async Task<GetVoucherTypeDTO> GetVoucherTypeByIdAsync(Guid id)
        {
            try
            {
                var voucherType = await _voucherTypeRepository.GetByIdAsync(id);
                if (voucherType != null)
                {
                    GetVoucherTypeDTO voucherTypeDTO = _mapper.Map<GetVoucherTypeDTO>(voucherType);
                    return voucherTypeDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy voucher type với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher type");
            }
        }

        public async Task<DynamicResponseModel<GetVoucherTypeDTO>> GetVoucherTypesAsync(PagingRequest pagingRequest,
                                                                                            VoucherTypeFilter voucherTypeFilter,
                                                                                            SortVoucherTypeEnum sortVoucherTypeEnum)
        {
            (int, IQueryable<GetVoucherTypeDTO>) result;
            try
            {
                result = _voucherTypeRepository.GetTable()
                            .ProjectTo<GetVoucherTypeDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetVoucherTypeDTO>(voucherTypeFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LimitPaging, PageConstant.DefaultPaging);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher type");
            }
            return new DynamicResponseModel<GetVoucherTypeDTO>()
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

        public async Task<bool> UpdateVoucherTypeAsync(Guid id, UpdateVoucherTypeDTO updateVoucherTypeDTO)
        {
            try
            {
                var existedVoucherType = await _voucherTypeRepository.GetByIdAsync(id);
                if (existedVoucherType != null)
                {
                    var entity = _mapper.Map<VoucherType>(updateVoucherTypeDTO);
                    return await _voucherTypeRepository.UpdateAsync(entity);
                }
                else
                {
                    throw new NotFoundException("Không tìm thấy voucher type");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật voucher type");
            }
        }
    }
}

