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
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class RatingService : IRatingService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Rating> _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IBaseRepository<User> userRepository,
                             IBaseRepository<Modal> modalRepository,
                             IBaseRepository<Order> orderRepository,
                             IBaseRepository<Rating> ratingRepository,
                             IMapper mapper)
        {
            _userRepository = userRepository;
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
            newRating.CreateBy = thisUserObj.userId;
            newRating.Status = RatingStatusEnum.NONE.ToString();

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

        public async Task<GetRatingDTO> GetRatingByIdAsync(Guid id)
        {
            var existedRating = await _ratingRepository.FindAsync(id);
            if (existedRating == null)
            {
                throw new NotFoundException("Không tìm thấy rating này");
            }

            return _mapper.Map<GetRatingDTO>(existedRating);
        }

        public async Task<dynamic> GetRatingDashboard(ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.FindAsync(thisUserObj.userId);

            // Count modals with orders but no ratings
            int modalNotRatingCount = existedUser.Vouchers
                .SelectMany(v => v.Modals) // Flatten all Modals across all Vouchers
                .Where(m => m.OrderDetails.Any() && !m.Ratings.Any()) // Modals with Orders but no Ratings
                .Count(); // Count the filtered results

            // Count modals with orders where ratings have not been replied to
            int countNotReplied = existedUser.Vouchers
                .SelectMany(v => v.Modals) // Flatten all Modals across all Vouchers
                .Where(m => m.OrderDetails.Any() && // Filter Modals with OrderDetails
                             m.Ratings.Any(r => r.Reply == null)) // Check if any Rating has a null Reply
                .Count(); // Count the filtered results

            // Total ordered modals
            int totalOrderedModals = existedUser.Vouchers
                .SelectMany(v => v.Modals)
                .Where(m => m.OrderDetails.Any()) // Modals with Orders
                .Count();

            // Percentage of ordered modals that have no ratings
            double percentageNotRated = totalOrderedModals == 0
                ? 0
                : Math.Round((double)modalNotRatingCount / totalOrderedModals * 100, 2);

            // Total ratings for all modals
            int totalRatings = existedUser.Vouchers
                .SelectMany(v => v.Modals)
                .SelectMany(m => m.Ratings)
                .Count();

            // Ratings with sellerStar > 3
            int goodRatings = existedUser.Vouchers
                .SelectMany(v => v.Modals)
                .SelectMany(m => m.Ratings)
                .Where(r => r.SellerStar > 3)
                .Count();

            // Percentage of ratings with sellerStar > 3
            double percentageGoodRatings = totalRatings == 0
                ? 0
                : Math.Round((double)goodRatings / totalRatings * 100, 2);

            // Return all results
            return new
            {
                modalNotRatingCount = modalNotRatingCount,
                countNotReplied = countNotReplied,
                percentageNotRated = percentageNotRated,
                percentageGoodRatings = percentageGoodRatings
            };
        }

        public async Task<DynamicResponseModel<GetRatingDTO>> GetSellerRatingAsync(PagingRequest pagingRequest, RatingFilter ratingFilter, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.FindAsync(thisUserObj.userId);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            var query = _modalRepository.GetTable()
                            .Where(v => v.Voucher.SellerId == existedUser.Id) // Filter vouchers by the seller
                            .SelectMany(x => x.Ratings)
                            .ProjectTo<GetRatingDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetRatingDTO>(ratingFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetRatingDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = query.Item1 // Total vouchers count for metadata
                },
                results = await query.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
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

        public Task<ResponseMessage<bool>> ReportRatingAsync(Guid id, string reason, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<bool>> DeleteRatingAsync(Guid id, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }
    }
}
