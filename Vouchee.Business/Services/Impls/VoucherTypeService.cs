﻿using AutoMapper;
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

        public async Task<ResponseMessage<Guid>> CreateVoucherTypeAsync(CreateVoucherTypeDTO createVoucherTypeDTO, ThisUserObj thisUserObj)
        {
            var voucherType = _mapper.Map<VoucherType>(createVoucherTypeDTO);
            voucherType.CreateBy = thisUserObj.userId;

            var voucherTypeId = await _voucherTypeRepository.AddAsync(voucherType);

            //if (voucherTypeId != null && createVoucherTypeDTO.image != null)
            //{
            //    voucherType.Image = await _fileUploadService.UploadImageToFirebase(createVoucherTypeDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.VOUCHER_TYPE);

            //    if (!await _voucherTypeRepository.UpdateAsync(voucherType))
            //    {
            //        throw new UpdateObjectException("Không thể update voucher type");
            //    }
            //}

            return new ResponseMessage<Guid>()
            {
                value = (Guid) voucherTypeId,
                result = true,
                message = "Tạo voucher type thành công"
            };
        }

        public async Task<ResponseMessage<bool>> DeleteVoucherTypeAsync(Guid id, ThisUserObj thisUserObj)
        {
            var voucherType = await _voucherTypeRepository.GetByIdAsync(id, isTracking: true);
            if (voucherType == null)
            {
                throw new NotFoundException("Không thấy voucher type này");
            }
            if (voucherType.Categories.Count() != 0)
            {
                voucherType.UpdateDate = DateTime.Now;
                voucherType.UpdateBy = thisUserObj.userId;
                voucherType.IsActive = false;

                await _voucherTypeRepository.SaveChanges();

                return new ResponseMessage<bool>()
                {
                    message = "Cập nhật voucher type thành công",
                    result = true,
                    value = true
                };
            }

            await _voucherTypeRepository.DeleteAsync(voucherType);

            return new ResponseMessage<bool>()
            {
                message = "Xóa voucher type thành công",
                result = true,
                value = true
            };
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
                throw new LoadException(ex.Message);
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
                throw new LoadException(ex.Message);
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

        public async Task<ResponseMessage<bool>> UpdateVoucherTypeAsync(Guid id, UpdateVoucherTypeDTO updateVoucherTypeDTO)
        {
            var existedVoucherType = await _voucherTypeRepository.GetByIdAsync(id, isTracking: true);
            if (existedVoucherType == null)
            {
                throw new NotFoundException("Không tìm thấy voucher type này");
            }

            existedVoucherType = _mapper.Map(updateVoucherTypeDTO, existedVoucherType);

            await _voucherTypeRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = true
            };
        }
    }
}

