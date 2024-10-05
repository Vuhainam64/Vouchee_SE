using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class VoucherCodeService : IVoucherCodeService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IVoucherCodeRepository _voucherCodeRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;

        public VoucherCodeService(IVoucherCodeRepository voucherCodeRepository, 
                                    IVoucherRepository voucherRepository, 
                                    IFileUploadService fileUploadService,
                                    IMapper mapper)
        {
            _fileUploadService = fileUploadService;
            _voucherCodeRepository = voucherCodeRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateVoucherCodeAsync(Guid voucherId, CreateVoucherCodeDTO createVoucherCodeDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var existedVoucher = await _voucherRepository.FindAsync(voucherId);
                if (existedVoucher == null)
                {
                    throw new NotFoundException($"Không tìm thấy voucher với id {voucherId}");
                }

                VoucherCode voucherCode = _mapper.Map<VoucherCode>(createVoucherCodeDTO);

                voucherCode.Status = VoucherCodeStatusEnum.ACTIVE.ToString();
                voucherCode.CreateBy = Guid.Parse(thisUserObj.userId);

                var voucherCodeId = await _voucherCodeRepository.AddAsync(voucherCode);

                if (createVoucherCodeDTO.image != null && voucherCodeId != null)
                {
                    voucherCode.Image = await _fileUploadService.UploadImageToFirebase(createVoucherCodeDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.VOUCHER_CODE);

                    await _voucherCodeRepository.UpdateAsync(voucherCode);
                }

                return voucherId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException(ex.Message);
            }
        }

        public Task<bool> DeleteVoucherAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GetVoucherDTO> GetVoucherCodeByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherCodesAsync(PagingRequest pagingRequest, VoucherCodeFilter voucherCodeFilter, SortVoucherCodeEnum sortVoucherCodeEnum)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO)
        {
            throw new NotImplementedException();
        }
    }
}
