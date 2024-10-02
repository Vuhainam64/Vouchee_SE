using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(IPromotionRepository promotionRepository, IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public Task<Guid?> CreatePromotionAsync(CreatePromotionDTO createPromotionDTO, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<GetPromotionDTO> GetPromotionByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DynamicResponseModel<GetPromotionDTO>> GetPromotionsAsync(PagingRequest pagingRequest, PromotionFilter promotionFilter, SortPromotionEnum sortPromotionEnum)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePromotionAsync(Guid id, UpdatePromotionDTO updatePromotionDTO, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }
    }
}
