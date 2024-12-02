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
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class RatingService : IRatingService
    {
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Rating> _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IBaseRepository<Modal> modalRepository,
                             IBaseRepository<Order> orderRepository,
                             IBaseRepository<Rating> ratingRepository,
                             IMapper mapper)
        {
            _modalRepository = modalRepository;
            _orderRepository = orderRepository;
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateRatingAsync(CreateRatingDTO createRatingDTO, ThisUserObj thisUserObj)
        {
            var existedOrder = await _orderRepository.GetByIdAsync(createRatingDTO.orderId, includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                                        .ThenInclude(x => x.Modal)
                                                                                                                            .ThenInclude(x => x.Voucher)
                                                                                            , isTracking: true);
            
            if (existedOrder == null)
            {
                throw new NotFoundException("Không tìm thấy đơn hàng này");
            }
            if (existedOrder.OrderDetails.FirstOrDefault(x => x.ModalId == createRatingDTO.modalId) == null)
            {
                throw new NotFoundException("Không tìm thấy modal này trong đơn hàng này");
            }
            if (existedOrder.Rating != null)
            {
                throw new ConflictException("Order này đã có rating rùi");
            }

            var newRating = _mapper.Map<Rating>(createRatingDTO);

            int count = 0;
            foreach (var media in newRating.Medias)
            {
                media.CreateBy = thisUserObj.userId;
                media.Index = count++;
            }

            existedOrder.Rating = newRating;

            await _orderRepository.SaveChanges();

            return new ResponseMessage<Guid>()
            {
                message = "Tạo rating thành công",
                result = true,
                value = existedOrder.Rating.Id
            };
        }

        public async Task<DynamicResponseModel<GetRatingDTO>> GetRatingAsync(PagingRequest pagingRequest, RatingFilter ratingFilter)
        {
            (int, IQueryable<GetRatingDTO>) result;

            result = _ratingRepository.GetTable()
                        .ProjectTo<GetRatingDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetRatingDTO>(ratingFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetRatingDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<GetRatingDTO> GetRatingByIdAsync(Guid id)
        {
            var existedRating = await _ratingRepository.FindAsync(id);
            if (existedRating == null)
            {
                throw new NotFoundException("Không tìm thấy rating này");
            }

            return _mapper.Map<GetRatingDTO>(existedRating);
        }

        public async Task<ResponseMessage<bool>> ReplyRatingAsync(Guid id, string reply, ThisUserObj thisUserObj)
        {
            var existedRating = await _ratingRepository.GetByIdAsync(id, isTracking: true);
            if (existedRating == null)
            {
                throw new NotFoundException("Không tìm thấy rating này");
            }

            existedRating.Reply = reply;
            existedRating.ReplyDate = DateTime.Now;
            existedRating.ReplyBy = thisUserObj.userId;

            await _ratingRepository.UpdateAsync(existedRating);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateRatingAsync(Guid id, UpdateRatingDTO updateRatingDTO, ThisUserObj thisUserObj)
        {
            var existedRating = await _ratingRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Modal)
                                                                                                    .ThenInclude(x => x.Voucher)
                                                                            , isTracking: true);
            if (existedRating == null)
            {
                throw new NotFoundException("Không tìm thấy rating này");
            }

            existedRating = _mapper.Map(updateRatingDTO, existedRating);
            existedRating.UpdateBy = thisUserObj.userId;

            await _ratingRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = true
            };
        }
    }
}
