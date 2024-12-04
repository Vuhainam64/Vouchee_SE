using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IRatingService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateRatingAsync(CreateRatingDTO createRatingDTO, ThisUserObj thisUserObj);

        // READ
        public Task<DynamicResponseModel<GetRatingDTO>> GetRatingAsync(PagingRequest pagingRequest, RatingFilter ratingFilter);
        public Task<GetRatingDTO> GetRatingByIdAsync(Guid id);
        public Task<dynamic> GetRatingDashboard(ThisUserObj thisUserObj);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateRatingAsync(Guid id, UpdateRatingDTO updateRatingDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> ReplyRatingAsync(Guid id, string reply, ThisUserObj thisUserObj);
    }
}
