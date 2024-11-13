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
    public interface IPromotionService
    {
        // CREATE
        public Task<Guid?> CreatePromotionAsync(CreatePromotionDTO createPromotionDTO, ThisUserObj thisUserObj);

        // READ
        public Task<IList<GetPromotionDTO>> GetPromotionBySeller(ThisUserObj thisUserObj);
        public Task<GetDetailPromotionDTO> GetPromotionByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetPromotionDTO>> GetPromotionsAsync(PagingRequest pagingRequest, PromotionFilter promotionFilter);
        public Task<DynamicResponseModel<GetPromotionDTO>> GetActivePromotion(PagingRequest pagingRequest, PromotionFilter promotionFilter);
        //public Task<IList<GetPromotionDTO>> GetPromotionByBuyerId(Guid buyerId);

        // UPDATE
        public Task<bool> UpdatePromotionAsync(Guid id, UpdatePromotionDTO updatePromotionDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj);
    }
}
