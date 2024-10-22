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

            foreach (var image in createModalDTO.productImagesUrl)
            {
                Media newImage = new()
                {
                    CreateBy = thisUserObj.userId,
                    CreateDate = DateTime.Now,
                    Type = MediaEnum.VOUCHER.ToString(),
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    // newImage.ImageUrl = await _fileUploadService.UploadImageToFirebase(image, thisUserObj.userId, StoragePathEnum.VOUCHER);
                    Url = image.ToString()
                };

                if (newImage != null)
                {
                    modal.Medias.Add(newImage);
                }
            }

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
            Modal modal = await _modalRepository.GetByIdAsync(id,
                includeProperties: query => query.Include(x => x.Medias));
            if (modal != null)
            {
                GetModalDTO getModalDTO = _mapper.Map<GetModalDTO>(modal);
                getModalDTO.image = getModalDTO.images.Count != 0 ? getModalDTO.images.FirstOrDefault().url : null;
                return getModalDTO;
            }
            throw new NotFoundException("Khong tim thay sub voucher");
        }

        public async Task<DynamicResponseModel<GetModalDTO>> GetModalsAsync(PagingRequest pagingRequest, ModalFilter modalFilter)
        {

            (int, IQueryable<GetModalDTO>) result;
            try
            {
                result = _modalRepository.GetTable()
                            .ProjectTo<GetModalDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetModalDTO>(modalFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
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
                results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
            };
        }

        public Task<bool> UpdateModalAsync(Guid id, UpdateModalDTO updateModalDTO)
        {
            throw new NotImplementedException();
        }
    }
}
