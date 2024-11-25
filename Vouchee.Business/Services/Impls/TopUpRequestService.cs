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
        private readonly IBaseRepository<MoneyRequest> _topUpRequestRepository;
        private readonly IMapper _mapper;

        public TopUpRequestService(IBaseRepository<User> userRepository,
                                    IBaseRepository<MoneyRequest> topUpRequestRepository, 
                                    IMapper mapper)
        {
            _userRepository = userRepository;
            _topUpRequestRepository = topUpRequestRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<string>> CreateTopUpRequest(CreateTopUpRequestDTO createTopUpRequestDTO, ThisUserObj thisUserObj)
        {
            var user = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.BuyerWallet), isTracking: true);

            if (user.BuyerWallet == null)
            {
                throw new Exception("User này chưa có ví người mua");
            }

            MoneyRequest topUpRequest = _mapper.Map<MoneyRequest>(createTopUpRequestDTO);
            topUpRequest.CreateBy = thisUserObj.userId;
            topUpRequest.TopUpWalletTransaction = new()
            {
                Type = "AMOUNT_IN",
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                Status = WalletTransactionStatusEnum.PENDING.ToString(),
                Amount = topUpRequest.Amount,
                BuyerWalletId = user.BuyerWallet.Id,
            };

            var result = await _topUpRequestRepository.AddReturnString(topUpRequest);
            return new ResponseMessage<string>()
            {
                message = "Tạo top up request thành công",
                result = true,
                value = result
            };
        }

        public async Task<GetTopUpRequestDTO> GetTopUpRequestById(string id)
        {
            var existedTopUpRequest = await _topUpRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.TopUpWalletTransaction)
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
            var existedTopUpRequest = await _topUpRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.TopUpWalletTransaction)
                                                                                                                                .ThenInclude(x => x.BuyerWallet), isTracking: true);

            if (existedTopUpRequest == null)
            {
                throw new NotFoundException("Không tìm thấy request này");
            }

            if (existedTopUpRequest.TopUpWalletTransaction.BuyerWallet == null)
            {
                throw new NotFoundException("Người này chưa có ví người mua");
            }

            existedTopUpRequest.Status = partnerTransactionId != Guid.Empty ? TopUpRequestStatusEnum.DONE.ToString() : TopUpRequestStatusEnum.ERROR.ToString();
            existedTopUpRequest.UpdateDate = DateTime.Now;
            existedTopUpRequest.UpdateBy = currentUser.userId;
            existedTopUpRequest.TopUpWalletTransaction.UpdateDate = DateTime.Now;
            existedTopUpRequest.TopUpWalletTransaction.UpdateBy = currentUser.userId; 

            if (partnerTransactionId != Guid.Empty)
            {
                existedTopUpRequest.TopUpWalletTransaction.BuyerWallet.Balance += existedTopUpRequest.Amount;
                existedTopUpRequest.TopUpWalletTransaction.Status = TopUpRequestStatusEnum.DONE.ToString();
                existedTopUpRequest.TopUpWalletTransaction.BuyerWallet.UpdateDate = DateTime.Now;
                existedTopUpRequest.TopUpWalletTransaction.BuyerWallet.UpdateBy = currentUser.userId;
                existedTopUpRequest.TopUpWalletTransaction.PartnerTransactionId = partnerTransactionId;
            }

            var result = await _topUpRequestRepository.UpdateAsync(existedTopUpRequest);

            return new ResponseMessage<GetSellerWallet>
            {
                message = $"Cập nhật ví {(partnerTransactionId != Guid.Empty ? "Thành công" : "Thất bại")}",
                result = partnerTransactionId != Guid.Empty,
                value = _mapper.Map<GetSellerWallet>(existedTopUpRequest.TopUpWalletTransaction.BuyerWallet)
            };
        }
    }
}
