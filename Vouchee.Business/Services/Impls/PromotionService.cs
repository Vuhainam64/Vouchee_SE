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
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class PromotionService : IPromotionService
    {
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<ModalPromotion> _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(IBaseRepository<Voucher> voucherRepository,
                                    IFileUploadService fileUploadService,
                                    IBaseRepository<ModalPromotion> promotionRepository,
                                    IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _fileUploadService = fileUploadService;
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreatePromotionAsync(CreatePromotionDTO createPromotionDTO, ThisUserObj thisUserObj)
        {

            var promotion = _mapper.Map<ModalPromotion>(createPromotionDTO);

            promotion.CreateBy = thisUserObj.userId;

            var promotionId = _promotionRepository.AddAsync(promotion).Result;

            if (createPromotionDTO.image != null && promotionId != null)
            {
                await _promotionRepository.UpdateAsync(promotion);
            }

            return promotionId;

        }

        public async Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj)
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
                throw new LoadException(ex.Message);
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

        //public async Task<IList<GetPromotionDTO>> GetPromotionByBuyerId(Guid buyerId)
        //{
        //    var promotion = await _promotionRepository.GetFirstOrDefaultAsync(x => x.);
        //    if (promotion != null)
        //    {
        //        var promotioDTO = _mapper.Map<IList<GetPromotionDTO>>(promotion);
        //        return promotioDTO;
        //    }
        //    else
        //    {
        //        throw new NotFoundException($"Không tìm thấy promotion của buyer id {buyerId}");
        //    }
        //}

        public async Task<GetDetailPromotionDTO> GetPromotionByIdAsync(Guid id)
        {
            //var promotion = await _promotionRepository.GetByIdAsync(id, includeProperties: query =>
            //    query.Include(x => x.Vouchers) // Include Vouchers
            //         .ThenInclude(v => v.Brand) // Include Brand inside Vouchers
            //         .Include(x => x.Vouchers)  // Include Vouchers again
            //         .ThenInclude(v => v.Categories) // Include Categories inside Vouchers
            //);

            //if (promotion != null)
            //{
            //    var promotioDTO = _mapper.Map<GetDetailPromotionDTO>(promotion);

            //    // Check for available promotions
            //    foreach (var voucher in promotioDTO.vouchers)
            //    {
            //        var availablePromotion = _promotionRepository.GetAvailableByVoucherId((Guid)voucher.id).Result;
            //        if (availablePromotion != null)
            //        {
            //            voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.FirstOrDefault().PercentDiscount / 100);
            //            voucher.percentDiscount = availablePromotion.FirstOrDefault().PercentDiscount;
            //        }
            //    }

            //    return promotioDTO;
            //}
            //else
            //{
            //    throw new NotFoundException($"Không tìm thấy promotion với id {id}");
            //}

            return null;
        }

        public async Task<IList<GetPromotionDTO>> GetPromotionBySeller(ThisUserObj thisUserObj)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                var promotions = await _promotionRepository.GetTable()
                                           .Where(promotion => promotion.StartDate <= dateTime && dateTime <= promotion.EndDate)
                                           .Include(x => x.Modals)
                                           .ProjectTo<GetPromotionDTO>(_mapper.ConfigurationProvider)
                                           .ToListAsync();
                return promotions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                throw new LoadException(ex.Message);
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
            var existedPromotion = await _promotionRepository.GetByIdAsync(id);
            if (existedPromotion != null)
            {
                var entity = _mapper.Map<ModalPromotion>(updatePromotionDTO);
                return await _promotionRepository.UpdateAsync(entity);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy order");
            }
        }
    }
}
