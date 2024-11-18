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
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class ShopPromotionService : IShopPromotionService
    {
        private readonly IBaseRepository<Promotion> _shopPromotionRepository;
        private readonly IMapper _mapper;

        public ShopPromotionService(IBaseRepository<Promotion> shopPromotionRepository, 
                                    IMapper mapper)
        {
            _shopPromotionRepository = shopPromotionRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateShopPromotionAsync(CreateShopPromotionDTO createShopPromotionDTO, ThisUserObj thisUserObj)
        {
            var currentTime = DateTime.Now;
            var promotion = _mapper.Map<Promotion>(createShopPromotionDTO);
            promotion.CreateBy = thisUserObj.userId;
            promotion.SellerId = thisUserObj.userId;

            var promotionId = await _shopPromotionRepository.AddAsync(promotion);

            return new ResponseMessage<Guid>()
            {
                message = "Tạo promotion thành công",
                result = true,
                value = promotionId.Value
            };
        }

        public async Task<GetShopPromotionDTO> GetActiveShopPromotion(ThisUserObj thisUserObj)
        {
            DateTime currentTime = DateTime.Now;
            var activePromotion = _shopPromotionRepository.GetTable().FirstOrDefault(x => x.SellerId == thisUserObj.userId);
            if (activePromotion == null)
            {
                throw new NotFoundException("Hiện tại bạn không có shop promotion này đang hoạt động");
            }

            return _mapper.Map<GetShopPromotionDTO>(activePromotion);
        }

        //public async Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj)
        //{

        //    var result = false;
        //    var promotion = await _promotionRepository.GetByIdAsync(id);
        //    if (promotion != null)
        //    {
        //        result = await _promotionRepository.DeleteAsync(promotion);
        //    }
        //    else
        //    {
        //        throw new NotFoundException($"Không tìm thấy promotion với id {id}");
        //    }
        //    return result;

        //}

        //public async Task<DynamicResponseModel<GetShopPromotionDTO>> GetActivePromotion(PagingRequest pagingRequest, ShopPromotionFilter promotionFilter)
        //{
        //    (int, IQueryable<GetShopPromotionDTO>) result;
        //    try
        //    {
        //        DateTime dateTime = DateTime.Now;
        //        result = _promotionRepository.GetTable().Where(x => x.StartDate <= dateTime && dateTime <= x.EndDate)
        //                    .ProjectTo<GetShopPromotionDTO>(_mapper.ConfigurationProvider)
        //                    .DynamicFilter(_mapper.Map<GetShopPromotionDTO>(promotionFilter))
        //                    .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAGE);
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerService.Logger(ex.Message);
        //        throw new LoadException(ex.Message);
        //    }
        //    return new DynamicResponseModel<GetShopPromotionDTO>()
        //    {
        //        metaData = new MetaData()
        //        {
        //            page = pagingRequest.page,
        //            size = pagingRequest.pageSize,
        //            total = result.Item1 // Total vouchers count for metadata
        //        },
        //        results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
        //    };
        //}

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

        public async Task<GetShopPromotionDTO> GetPromotionByIdAsync(Guid id)
        {
            var promotion = await _shopPromotionRepository.GetByIdAsync(id);

            if (promotion != null)
            {
                var promotioDTO = _mapper.Map<GetShopPromotionDTO>(promotion);

                return promotioDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy shop promotion với id {id}");
            }
        }

        //public async Task<IList<GetShopPromotionDTO>> GetShopPromotionBySeller(ThisUserObj thisUserObj)
        //{
        //    try
        //    {
        //        DateTime dateTime = DateTime.Now;
        //        var promotions = await _promotionRepository.GetTable()
        //                                   .Where(promotion => promotion.StartDate <= dateTime && dateTime <= promotion.EndDate)
        //                                   .Include(x => x.Modals)
        //                                   .ProjectTo<GetShopPromotionDTO>(_mapper.ConfigurationProvider)
        //                                   .ToListAsync();
        //        return promotions;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<DynamicResponseModel<GetShopPromotionDTO>> GetPromotionsAsync(PagingRequest pagingRequest, ShopPromotionFilter promotionFilter)
        {
            (int, IQueryable<GetShopPromotionDTO>) result;
            try
            {
                DateTime dateTime = DateTime.Now;
                result = _shopPromotionRepository.GetTable()
                            .ProjectTo<GetShopPromotionDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetShopPromotionDTO>(promotionFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAGE);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetShopPromotionDTO>()
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

        //public async Task<bool> UpdatePromotionAsync(Guid id, UpdateShopPromotionDTO updatePromotionDTO, ThisUserObj thisUserObj)
        //{
        //    var existedPromotion = await _promotionRepository.GetByIdAsync(id);
        //    if (existedPromotion != null)
        //    {
        //        var entity = _mapper.Map<ModalPromotion>(updatePromotionDTO);
        //        return await _promotionRepository.UpdateAsync(entity);
        //    }
        //    else
        //    {
        //        throw new NotFoundException("Không tìm thấy order");
        //    }
        //}
    }
}
