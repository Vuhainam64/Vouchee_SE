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
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class ModalService : IModalService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IModalRepository _modalRepository;
        private readonly IMapper _mapper;
        public ModalService(IVoucherRepository voucherRepository,
                                    IFileUploadService fileUploadService,
                                    IModalRepository modalRepository, 
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

            var existVoucher = await _voucherRepository.FindAsync(voucherId);
            
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
            Modal modal = await _modalRepository.FindAsync(id);
            if (modal != null)
            {
                return await _modalRepository.DeleteAsync(modal);
            }
            return false;
        }

        public async Task<GetModalDTO> GetModalByIdAsync(Guid id)
        {
            Modal modal = await _modalRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.VoucherCodes)
                                                                                                .ThenInclude(x => x.OrderDetail));

            if (modal == null)
               throw new NotFoundException("Khong tim thay sub voucher");

            return _mapper.Map<GetModalDTO>(modal);
        }

        public async Task<DynamicResponseModel<GetModalDTO>> GetModalsAsync(PagingRequest pagingRequest, ModalFilter modalFilter)
        {
            List<GetModalDTO> list;
            (int, IQueryable<GetModalDTO>) result;
            try
            {
                result = _modalRepository.GetTable(includeProperties: x => x.Include(x => x.VoucherCodes).ThenInclude(x => x.OrderDetail))
                            .ProjectTo<GetModalDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetModalDTO>(modalFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

                list = result.Item2.ToList();

                foreach (var modal in list)
                {
                    modal.quantity = modal.voucherCodes.Where(x => x.status == ObjectStatusEnum.ACTIVE.ToString()).Count();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải sub voucher");
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
    }
}
