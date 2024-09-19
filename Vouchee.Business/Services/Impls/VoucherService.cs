using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;

        public VoucherService(IVoucherRepository voucherRepository,
                                IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO)
        {
            try
            {
                Voucher voucher = _mapper.Map<Voucher>(createVoucherDTO);
                var voucherId = await _voucherRepository.AddAsync(voucher);
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
                Voucher voucher = await _voucherRepository.GetByIdAsync(id);
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

        public async Task<GetVoucherDTO> GetVoucherByIdAsync(Guid id)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(id);
                if (voucher != null)
                {
                    GetVoucherDTO voucherDTO = _mapper.Map<GetVoucherDTO>(voucher);
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

        public async Task<DynamicResponseModel<GetVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                                    VoucherFiler voucherFiler,
                                                                                    VoucherOrderEnum voucherOrderEnum)
        {
            int total;
            (int, IQueryable<GetVoucherDTO>) result;
            try
            {
                result = _voucherRepository.GetTable()
                            .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetVoucherDTO>(voucherFiler))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LimitPaging, PageConstant.DefaultPaging);

                total = result.Item2.Count();

                if (total == 0)
                {
                    throw new NotFoundException("Không tìm thấy voucher");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return new DynamicResponseModel<GetVoucherDTO>()
            {
                Metadata = new PagingMetadata()
                {
                    Page = pagingRequest.page,
                    Size = pagingRequest.pageSize,
                    Total = total
                },
                Results = result.Item2.ToList()
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
