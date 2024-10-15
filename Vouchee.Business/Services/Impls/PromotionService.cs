using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class PromotionService : IPromotionService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(IVoucherRepository voucherRepository,
                                    IFileUploadService fileUploadService,
                                    IPromotionRepository promotionRepository, 
                                    IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _fileUploadService = fileUploadService;
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreatePromotionAsync(CreatePromotionDTO createPromotionDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var promotion = _mapper.Map<Promotion>(createPromotionDTO);

                promotion.CreateBy = Guid.Parse(thisUserObj.userId);

                var promotionId = _promotionRepository.AddAsync(promotion).Result;
                
                if (createPromotionDTO.image != null && promotionId != null)
                {
                    promotion.Image = await _fileUploadService.UploadImageToFirebase(createPromotionDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.PROMOTION);

                    await _promotionRepository.UpdateAsync(promotion);
                }

                return promotionId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception("Lỗi không xác định khi tạo promotion");
            }
        }

        public async Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj)
        {
            try
            {
                var result = false;
                var promotion = await _promotionRepository.GetByIdAsync(id);
                if (promotion != null)
                {
                    result = await _promotionRepository.DeleteAsync(promotion);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy promotion với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa promotion");
            }
        }

        public async Task<DynamicResponseModel<GetPromotionDTO>> GetActivePromotion(PagingRequest pagingRequest, PromotionFilter promotionFilter)
        {
            (int, IQueryable<GetPromotionDTO>) result;
            try
            {
                DateTime dateTime = DateTime.Now;
                result = _promotionRepository.GetTable().Where(x => x.StartDate <= dateTime && dateTime <= x.EndDate)
                            .ProjectTo<GetPromotionDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetPromotionDTO>(promotionFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAGE);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải promotion");
            }
            return new DynamicResponseModel<GetPromotionDTO>()
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

        public async Task<IList<GetPromotionDTO>> GetPromotionByBuyerId(Guid buyerId)
        {
            var promotion = await _promotionRepository.GetPromotionByBuyerId(buyerId);
            if (promotion != null)
            {
                var promotioDTO = _mapper.Map<IList<GetPromotionDTO>>(promotion);
                return promotioDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy promotion của buyer id {buyerId}");
            }
        }

        public async Task<GetDetailPromotionDTO> GetPromotionByIdAsync(Guid id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id, includeProperties: query =>
                query.Include(x => x.Vouchers) // Include Vouchers
                     .ThenInclude(v => v.Brand) // Include Brand inside Vouchers
                     .Include(x => x.Vouchers)  // Include Vouchers again
                     .ThenInclude(v => v.Categories) // Include Categories inside Vouchers
            );

            if (promotion != null)
            {
                var promotioDTO = _mapper.Map<GetDetailPromotionDTO>(promotion);

                // Check for available promotions
                foreach (var voucher in promotioDTO.vouchers)
                {
                    var availablePromotion = _promotionRepository.GetAvailableByVoucherId((Guid)voucher.id).Result;
                    if (availablePromotion != null)
                    {
                        voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.FirstOrDefault().PercentDiscount / 100);
                        voucher.percentDiscount = availablePromotion.FirstOrDefault().PercentDiscount;
                    }
                }

                return promotioDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy promotion với id {id}");
            }
        }

        public async Task<DynamicResponseModel<GetPromotionDTO>> GetPromotionsAsync(PagingRequest pagingRequest, PromotionFilter promotionFilter)
        {
            (int, IQueryable<GetPromotionDTO>) result;
            try
            {
                DateTime dateTime = DateTime.Now;
                result = _promotionRepository.GetTable()
                            .ProjectTo<GetPromotionDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetPromotionDTO>(promotionFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAGE);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải promotion");
            }
            return new DynamicResponseModel<GetPromotionDTO>()
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

        public async Task<bool> UpdatePromotionAsync(Guid id, UpdatePromotionDTO updatePromotionDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var existedPromotion = await _promotionRepository.GetByIdAsync(id);
                if (existedPromotion != null)
                {
                    var entity = _mapper.Map<Promotion>(updatePromotionDTO);
                    return await _promotionRepository.UpdateAsync(entity);
                }
                else
                {
                    throw new NotFoundException("Không tìm thấy order");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật order");
            }
        }
    }
}
