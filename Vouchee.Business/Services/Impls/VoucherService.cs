﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class VoucherService : IVoucherService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;

        public VoucherService(IFileUploadService fileUploadService,
                                IVoucherRepository voucherRepository,
                                IMapper mapper)
        {
            _fileUploadService = fileUploadService;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO, ThisUserObj thisUserObj)
        {
            try
            {
                Voucher voucher = _mapper.Map<Voucher>(createVoucherDTO);

                voucher.Status = VoucherStatusEnum.ACTIVE.ToString();
                voucher.CreateBy = Guid.Parse(thisUserObj.userId);


                var voucherId = await _voucherRepository.AddAsync(voucher);

                if (createVoucherDTO.image != null && voucherId != null)
                {
                    voucher.Image = await _fileUploadService.UploadImageToFirebase(createVoucherDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.VOUCHER);

                    await _voucherRepository.UpdateAsync(voucher);
                }

                return voucherId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo voucher");
            }
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            try
            {
                bool result = false;
                var voucher = await _voucherRepository.GetByIdAsync(id);
                if (voucher != null)
                {
                    result = await _voucherRepository.DeleteAsync(voucher);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy voucher với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa voucher");
            }
        }

        public Task<IList<GetAllVoucherDTO>> GetBestSoldVouchers()
        {
            throw new NotImplementedException();
        }

        public Task<IList<GetAllVoucherDTO>> GetNearestVouchers(decimal lon, decimal lat)
        {
            throw new NotImplementedException();
        }

        public Task<IList<GetAllVoucherDTO>> GetNearestVouchers(PagingRequest pagingRequest, decimal lon, decimal lat)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<GetAllVoucherDTO>> GetNewestVouchers()
        {
            IQueryable<GetAllVoucherDTO> result;
            try
            {
                result = _voucherRepository.GetTable()
                                            .OrderByDescending(x => x.CreateDate)  
                                            .Take(8)
                                            .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return result.ToList();
        }

        public Task<IList<GetAllVoucherDTO>> GetTopSaleVouchers()
        {
            throw new NotImplementedException();
        }

        public Task<IList<GetAllVoucherDTO>> GetTopSaleVouchers(PagingRequest pagingRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAllVoucherDTO> GetVoucherByIdAsync(Guid id)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(id,
                                        query => query.Include(x => x.VoucherCodes)
                                                        .Include(x => x.Addresses));
                if (voucher != null)
                {
                    GetAllVoucherDTO voucherDTO = _mapper.Map<GetAllVoucherDTO>(voucher);
                    return voucherDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy voucher với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
        }

        public async Task<DynamicResponseModel<GetAllVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                                    VoucherFilter voucherFiler)
        {
            (int, IQueryable<GetAllVoucherDTO>) result;
            try
            {
                result = _voucherRepository.GetTable()
                            .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetAllVoucherDTO>(voucherFiler))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return new DynamicResponseModel<GetAllVoucherDTO>()
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

        public async Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO)
        {
            try
            {
                var existedVoucher = await _voucherRepository.GetByIdAsync(id);
                if (existedVoucher != null)
                {
                    var entity = _mapper.Map<Voucher>(updateVoucherDTO);
                    return await _voucherRepository.UpdateAsync(entity);
                }
                else
                {
                    throw new NotFoundException("Không tìm thấy voucher");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật voucher");
            }
        }
    }
}
