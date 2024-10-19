using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class SubVoucherService : ISubVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ISubVoucherRepository _subVoucherRepository;
        private readonly IMapper _mapper;
        public SubVoucherService(IVoucherRepository voucherRepository,
                                    IFileUploadService fileUploadService,
                                    ISubVoucherRepository subVoucherRepository, 
                                    IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _fileUploadService = fileUploadService;
            _subVoucherRepository = subVoucherRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateSubVoucherAsync(Guid voucherId, CreateSubVoucherDTO createSubVoucherDTO, ThisUserObj thisUserObj)
        {
            SubVoucher subVoucher = _mapper.Map<SubVoucher>(createSubVoucherDTO);

            var existVoucher = await _voucherRepository.FindAsync(voucherId);
            
            if (existVoucher == null)
            {
                throw new NotFoundException("Không tìm thấy voucher với id");
            }

            subVoucher.VoucherId = voucherId;
            subVoucher.CreateBy = Guid.Parse(thisUserObj.userId);

            foreach (var image in createSubVoucherDTO.productImagesUrl)
            {
                Image newImage = new();

                newImage.CreateBy = Guid.Parse(thisUserObj.userId);
                newImage.CreateDate = DateTime.Now;
                newImage.MediaType = "PRODUCT";
                newImage.Status = ObjectStatusEnum.ACTIVE.ToString();
                // newImage.ImageUrl = await _fileUploadService.UploadImageToFirebase(image, thisUserObj.userId, StoragePathEnum.VOUCHER);
                newImage.MediaUrl = image;

                if (newImage != null)
                {
                    subVoucher.Images.Add(newImage);
                }
            }

            return await _subVoucherRepository.AddAsync(subVoucher);
        }

        public async Task<bool> DeleteSubVoucherAsync(Guid id)
        {
            SubVoucher subVoucher = await _subVoucherRepository.FindAsync(id);
            if (subVoucher != null)
            {
                return await _subVoucherRepository.DeleteAsync(subVoucher);
            }
            return false;
        }

        public async Task<GetSubVoucherDTO> GetSubVoucherByIdAsync(Guid id)
        {
            SubVoucher subVoucher = await _subVoucherRepository.GetByIdAsync(id,
                includeProperties: query => query.Include(x => x.Images));
            if (subVoucher != null)
            {
                GetSubVoucherDTO getSubVoucherDTO = _mapper.Map<GetSubVoucherDTO>(subVoucher);
                getSubVoucherDTO.image = getSubVoucherDTO.images.Count != 0 ? getSubVoucherDTO.images.FirstOrDefault().imageUrl : null;
                return getSubVoucherDTO;
            }
            throw new NotFoundException("Khong tim thay sub voucher");
        }

        public async Task<DynamicResponseModel<GetSubVoucherDTO>> GetSubVouchersAsync(PagingRequest pagingRequest, SubVoucherFilter subVoucherFilter)
        {

            (int, IQueryable<GetSubVoucherDTO>) result;
            try
            {
                result = _subVoucherRepository.GetTable()
                            .ProjectTo<GetSubVoucherDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetSubVoucherDTO>(subVoucherFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải sub voucher");
            }
            return new DynamicResponseModel<GetSubVoucherDTO>()
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

        public Task<bool> UpdateSubVoucherAsync(Guid id, UpdateSubVoucherDTO updateSubVoucherDTO)
        {
            throw new NotImplementedException();
        }
    }
}
