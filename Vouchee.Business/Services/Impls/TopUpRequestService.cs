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
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class TopUpRequestService : ITopUpRequestService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<TopUpRequest> _topUpRequestRepository;
        private readonly IMapper _mapper;

        public TopUpRequestService(IBaseRepository<User> userRepository,
                                    IBaseRepository<TopUpRequest> topUpRequestRepository, 
                                    IMapper mapper)
        {
            _userRepository = userRepository;
            _topUpRequestRepository = topUpRequestRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateTopUpRequest(CreateTopUpRequestDTO createTopUpRequestDTO, ThisUserObj thisUserObj)
        {
            var user = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.BuyerWallet), isTracking: true);

            if (user.BuyerWallet == null)
            {
                throw new Exception("User này chưa có ví người mua");
            }

            TopUpRequest topUpRequest = _mapper.Map<TopUpRequest>(createTopUpRequestDTO);
            topUpRequest.CreateBy = thisUserObj.userId;
            topUpRequest.WalletTransaction = new()
            {
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                Status = WalletTransactionStatusEnum.PENDING.ToString(),
                Amount = topUpRequest.Amount,
                BuyerWalletId = user.BuyerWallet.Id,
            };

            var result = await _topUpRequestRepository.AddAsync(topUpRequest);
            return new ResponseMessage<Guid>()
            {
                message = "Tạo top up request thành công",
                result = true,
                value = (Guid)result
            };
        }

        public async Task<GetTopUpRequestDTO> GetTopUpRequestById(Guid id)
        {
            var existedTopUpRequest = await _topUpRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.WalletTransaction)
                                                                                                                                .ThenInclude(x => x.BuyerWallet));

            if (existedTopUpRequest == null)
            {
                throw new NotFoundException("Không tìm thấy");
            }

            return _mapper.Map<GetTopUpRequestDTO>(existedTopUpRequest);
        }

        public async Task<DynamicResponseModel<GetTopUpRequestDTO>> GetTopUpRequestsAsync(PagingRequest pagingRequest, TopUpRequestFilter topUpRequestFilter)
        {
            (int, IQueryable<GetTopUpRequestDTO>) result;
            try
            {
                result = _topUpRequestRepository.GetTable()
                            .ProjectTo<GetTopUpRequestDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetTopUpRequestDTO>(topUpRequestFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetTopUpRequestDTO>()
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

        public async Task<ResponseMessage<GetSellerWallet>> UpdateTopUpRequest(Guid id, Guid partnerTransactionId ,ThisUserObj currentUser = null)
        {
            var existedTopUpRequest = await _topUpRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.WalletTransaction)
                                                                                                                                .ThenInclude(x => x.BuyerWallet), isTracking: true);

            if (existedTopUpRequest == null)
            {
                throw new NotFoundException("Không tìm thấy request này");
            }

            if (existedTopUpRequest.WalletTransaction.BuyerWallet == null)
            {
                throw new NotFoundException("Người này chưa có ví người mua");
            }

            existedTopUpRequest.Status = partnerTransactionId != Guid.Empty ? TopUpRequestStatusEnum.DONE.ToString() : TopUpRequestStatusEnum.ERROR.ToString();
            existedTopUpRequest.Description = partnerTransactionId != Guid.Empty ? "Nạp ví thành công" : "Nạp ví thất bại";
            existedTopUpRequest.UpdateDate = DateTime.Now;
            existedTopUpRequest.UpdateBy = currentUser.userId;
            existedTopUpRequest.WalletTransaction.UpdateDate = DateTime.Now;
            existedTopUpRequest.WalletTransaction.UpdateBy = currentUser.userId; 

            if (partnerTransactionId != Guid.Empty)
            {
                existedTopUpRequest.WalletTransaction.BuyerWallet.Balance += existedTopUpRequest.Amount;
                existedTopUpRequest.WalletTransaction.Status = TopUpRequestStatusEnum.DONE.ToString();
                existedTopUpRequest.WalletTransaction.BuyerWallet.UpdateDate = DateTime.Now;
                existedTopUpRequest.WalletTransaction.BuyerWallet.UpdateBy = currentUser.userId;
                existedTopUpRequest.WalletTransaction.PartnerTransactionId = partnerTransactionId;
            }

            var result = await _topUpRequestRepository.UpdateAsync(existedTopUpRequest);

            return new ResponseMessage<GetSellerWallet>
            {
                message = $"Cập nhật ví {(partnerTransactionId != Guid.Empty ? "Thành công" : "Thất bại")}",
                result = partnerTransactionId != Guid.Empty,
                value = _mapper.Map<GetSellerWallet>(existedTopUpRequest.WalletTransaction.BuyerWallet)
            };
        }
    }
}
