using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

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

        public async Task<bool> DeleteVoucherCodeAsync(Guid id)
        {
            try
            {
                var existedVoucherCode = await _voucherCodeRepository.FindAsync(id);
                if (existedVoucherCode == null)
                {
                    throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
                }

                var result = await _voucherCodeRepository.DeleteAsync(existedVoucherCode);

                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException(ex.Message);
            }
        }

        public async Task<GetVoucherCodeDTO> GetVoucherCodeByIdAsync(Guid id)
        {
            try
            {
                var existedVoucherCode = await _voucherCodeRepository.GetByIdAsync(id);
                GetVoucherCodeDTO voucherCodeDTO = _mapper.Map<GetVoucherCodeDTO>(existedVoucherCode);

                if (existedVoucherCode == null)
                {
                    throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
                }

                return voucherCodeDTO;   
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<IList<GetVoucherCodeDTO>> GetVoucherCodesAsync()
        {
            try
            {
                IQueryable<GetVoucherCodeDTO> result;
                try
                {
                    result = _voucherCodeRepository.GetTable()
                                .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider);
                }
                catch (Exception ex)
                {
                    LoggerService.Logger(ex.Message);
                    throw new LoadException("Lỗi không xác định khi tải voucher code");
                }
                return result.ToList();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<bool> UpdateVoucherCodeAsync(Guid id, UpdateVoucherCodeDTO updateVoucherCodeDTO)
        {
            try
            {
                var existedVoucherCode = await _voucherCodeRepository.FindAsync(id);

                if (existedVoucherCode == null)
                {
                    throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
                }

                _mapper.Map(updateVoucherCodeDTO, existedVoucherCode);

                var result = await _voucherCodeRepository.UpdateAsync(existedVoucherCode);

                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException(ex.Message);
            }
        }
    }
}