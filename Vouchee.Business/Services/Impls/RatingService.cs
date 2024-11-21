using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class RatingService : IRatingService
    {
        private readonly IBaseRepository<Rating> _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IBaseRepository<Rating> ratingRepository, IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public Task<ResponseMessage<Guid>> CreateRatingAsync(CreateRatingDTO createRatingDTO, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<GetRatingDTO>> GetRatingAsync(PagingRequest pagingRequest, RatingFilter ratingFilter)
        {
            throw new NotImplementedException();
        }

        public Task<GetRatingDTO> GetRatingByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<bool>> ReplyRatingAsync(string reply, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<bool>> UpdateRatingAsync(UpdateRatingDTO updateRatingDTO, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }
    }
}
