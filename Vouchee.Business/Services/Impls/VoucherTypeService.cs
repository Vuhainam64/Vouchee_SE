using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class VoucherTypeService : IVoucherTypeService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<VoucherType> _voucherTypeRepository;
        private readonly IMapper _mapper;

        public VoucherTypeService(IFileUploadService fileUploadService,
                                      IBaseRepository<VoucherType> voucherTypeRepository,
                                      IMapper mapper)
        {
            _fileUploadService = fileUploadService;
            _voucherTypeRepository = voucherTypeRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateVoucherTypeAsync(CreateVoucherTypeDTO createVoucherTypeDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var voucherType = _mapper.Map<VoucherType>(createVoucherTypeDTO);
                voucherType.CreateBy = thisUserObj.userId;

                var voucherTypeId = await _voucherTypeRepository.AddAsync(voucherType);
                
                if (voucherTypeId != null && createVoucherTypeDTO.image != null)
                {
                    voucherType.Image = await _fileUploadService.UploadImageToFirebase(createVoucherTypeDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.VOUCHER_TYPE);

                    if (!await _voucherTypeRepository.UpdateAsync(voucherType))
                    {
                        throw new UpdateObjectException("Không thể update voucher type");
                    }
                }
                
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
                var voucherType = await _voucherTypeRepository.GetByIdAsync(id,
                                    query => query.Include(e => e.Categories));
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

        public async Task<DynamicResponseModel<GetVoucherTypeDTO>> GetVoucherTypesAsync(PagingRequest pagingRequest, VoucherTypeFilter voucherTypeFilter)
        {
            (int, IQueryable<GetVoucherTypeDTO>) result;
            try
            {
                result = _voucherTypeRepository.GetTable()
                            .ProjectTo<GetVoucherTypeDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetVoucherTypeDTO>(voucherTypeFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher type");
            }
            return new DynamicResponseModel<GetVoucherTypeDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
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

