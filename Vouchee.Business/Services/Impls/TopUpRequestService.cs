﻿using AutoMapper;
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
            return null;

            //var user = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Wallets), isTracking: true);
            //var buyerWallet = user.Wallets.FirstOrDefault(x => x.Type == WalletTypeEnum.BUYER.ToString());
            
            //if (buyerWallet == null)
            //{
            //    throw new Exception("User này chưa có ví người mua");
            //}

            //TopUpRequest topUpRequest = _mapper.Map<TopUpRequest>(createTopUpRequestDTO);
            //topUpRequest.CreateBy = thisUserObj.userId;
            //topUpRequest.WalletTransaction = new()
            //{
            //    CreateBy = thisUserObj.userId,
            //    CreateDate = DateTime.Now,
            //    Status = WalletTransactionStatusEnum.PENDING.ToString(),
            //    Amount = topUpRequest.Amount,
            //    BuyerWalletId = buyerWallet.Id
            //};

            //var result = await _topUpRequestRepository.AddAsync(topUpRequest);
            //return new ResponseMessage<Guid>()
            //{
            //    message = "Tạo top up request thành công",
            //    result = true,
            //    value = (Guid) result
            //};
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

        public async Task<ResponseMessage<GetWalletDTO>> UpdateTopUpRequest(Guid id, bool success = false, string description = null, ThisUserObj currentUser = null)
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

            existedTopUpRequest.Status = success ? TopUpRequestStatusEnum.DONE.ToString() : TopUpRequestStatusEnum.ERROR.ToString();
            existedTopUpRequest.Description = description;
            existedTopUpRequest.UpdateDate = DateTime.Now;
            existedTopUpRequest.UpdateBy = currentUser.userId;
            existedTopUpRequest.WalletTransaction.UpdateDate = DateTime.Now;
            existedTopUpRequest.WalletTransaction.UpdateBy = currentUser.userId; 

            if (success)
            {
                existedTopUpRequest.WalletTransaction.BuyerWallet.Balance += existedTopUpRequest.Amount;
                existedTopUpRequest.WalletTransaction.Status = TopUpRequestStatusEnum.DONE.ToString();
                existedTopUpRequest.WalletTransaction.BuyerWallet.UpdateDate = DateTime.Now;
                existedTopUpRequest.WalletTransaction.BuyerWallet.UpdateBy = currentUser.userId;
            }

            var result = await _topUpRequestRepository.UpdateAsync(existedTopUpRequest);

            return new ResponseMessage<GetWalletDTO>
            {
                message = $"Cập nhật ví {(success ? "Thành công" : "Thất bại")}",
                result = success,
                value = _mapper.Map<GetWalletDTO>(existedTopUpRequest.WalletTransaction.BuyerWallet)
            };
        }
    }
}