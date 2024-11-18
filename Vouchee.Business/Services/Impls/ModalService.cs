using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class ModalService : IModalService
    {
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IMapper _mapper;
        public ModalService(IBaseRepository<Voucher> voucherRepository,
                                    IFileUploadService fileUploadService,
                                    IBaseRepository<Modal> modalRepository,
                                    IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _fileUploadService = fileUploadService;
            _modalRepository = modalRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateModalAsync(Guid voucherId, CreateModalDTO createModalDTO, ThisUserObj thisUserObj)
        {
            Modal modal = _mapper.Map<Modal>(createModalDTO);

            var existVoucher = await _voucherRepository.FindAsync(voucherId, false);

            if (existVoucher == null)
            {
                throw new NotFoundException("Không tìm thấy voucher với id");
            }

            modal.VoucherId = voucherId;
            modal.CreateBy = thisUserObj.userId;

            return await _modalRepository.AddAsync(modal);
        }

        public async Task<bool> DeleteModalAsync(Guid id)
        {
            Modal modal = await _modalRepository.FindAsync(id, false);
            if (modal != null)
            {
                return await _modalRepository.DeleteAsync(modal);
            }
            return false;
        }

        public async Task<dynamic> GetModalByIdAsync(Guid id, PagingRequest pagingRequest)
        {
            Modal existedModal = await _modalRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.VoucherCodes));

            if (existedModal != null)
            {
                var modal = _mapper.Map<GetDetailModalDTO>(existedModal);
                var total = modal.voucherCodes.Count();

                var pagedVoucherCode = modal.voucherCodes
                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                    .Take(pagingRequest.pageSize).ToList();

                modal.voucherCodes = pagedVoucherCode;

                return new
                {
                    metaData = new
                    {
                        page = pagingRequest.page,
                        size = pagingRequest.pageSize,
                        total = total
                    },
                    results = modal
                };
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }
        }

        public async Task<DynamicResponseModel<GetModalDTO>> GetModalsAsync(PagingRequest pagingRequest, ModalFilter modalFilter)
        {
            List<GetModalDTO> list;
            (int, IQueryable<GetModalDTO>) result;
            try
            {
                result = _modalRepository.GetTable()
                            .Include(x => x.VoucherCodes)
                            .ProjectTo<GetModalDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetModalDTO>(modalFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

                list = result.Item2.ToList();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetModalDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = list// Return the paged voucher list with nearest address and distance
            };
        }

        public Task<bool> UpdateModalAsync(Guid id, UpdateModalDTO updateModalDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseMessage<GetModalDTO>> UpdateModalisActiveAsync(Guid id, bool isActive)
        {
            var existedModal = await _modalRepository.GetByIdAsync(id, isTracking: true);
            if (existedModal != null)
            {
                existedModal.IsActive = isActive;
                await _modalRepository.UpdateAsync(existedModal);
                return new ResponseMessage<GetModalDTO>()
                {
                    message = "Đổi isActive thành công",
                    result = true,
                    value = _mapper.Map<GetModalDTO>(existedModal)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy modal");

            }
        }

        public async Task<ResponseMessage<GetModalDTO>> UpdateModalStatusAsync(Guid id, VoucherStatusEnum modalStatus)
        {
            var existedModal = await _modalRepository.GetByIdAsync(id, isTracking: true);
            if (existedModal != null)
            {
                existedModal.Status = modalStatus.ToString();
                await _modalRepository.UpdateAsync(existedModal);
                return new ResponseMessage<GetModalDTO>()
                {
                    message = "Đổi status thành công",
                    result = true,
                    value = _mapper.Map<GetModalDTO>(existedModal)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy modal");
            }
        }
    }
}
