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
        public Task<Guid?> CreatePromotionAsync(CreateShopPromotionDTO createPromotionDTO, ThisUserObj thisUserObj);

        // READ
        public Task<IList<GetShopPromotionDTO>> GetPromotionBySeller(ThisUserObj thisUserObj);
        public Task<GetDetailShopPromotionDTO> GetPromotionByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetShopPromotionDTO>> GetPromotionsAsync(PagingRequest pagingRequest, PromotionFilter promotionFilter);
        public Task<DynamicResponseModel<GetShopPromotionDTO>> GetActivePromotion(PagingRequest pagingRequest, PromotionFilter promotionFilter);
        //public Task<IList<GetPromotionDTO>> GetPromotionByBuyerId(Guid buyerId);

        // UPDATE
        public Task<bool> UpdatePromotionAsync(Guid id, UpdateShopPromotionDTO updatePromotionDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj);
    }
}
