using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IShopPromotionService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateShopPromotionAsync(CreateShopPromotionDTO createPromotionDTO, ThisUserObj thisUserObj);

        // READ
        // public Task<IList<GetShopPromotionDTO>> GetShopPromotionBySeller(ThisUserObj thisUserObj);
        public Task<GetShopPromotionDTO> GetPromotionByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetShopPromotionDTO>> GetPromotionsAsync(PagingRequest pagingRequest, ShopPromotionFilter promotionFilter);
        public Task<GetShopPromotionDTO> GetActiveShopPromotion(ThisUserObj thisUserObj);
        public Task<IList<GetShopPromotionDTO>> GetShopPromotionByShopId(Guid shopId);
        // public Task<DynamicResponseModel<GetShopPromotionDTO>> GetActivePromotion(PagingRequest pagingRequest, ShopPromotionFilter promotionFilter);
        // public Task<IList<GetPromotionDTO>> GetPromotionByBuyerId(Guid buyerId);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdatePromotionAsync(Guid id, UpdateShopPromotionDTO updatePromotionDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> UpdatePromotionState(Guid id, bool isActive, ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj);
    }
}
