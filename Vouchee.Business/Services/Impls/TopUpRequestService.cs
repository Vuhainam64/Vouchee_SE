using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

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
            var user = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Wallets), isTracking: true);
            var buyerWallet = user.Wallets.FirstOrDefault(x => x.Type == WalletTypeEnum.BUYER.ToString());
            
            if (buyerWallet == null)
            {
                throw new Exception("User này chưa có ví người mua");
            }

            TopUpRequest topUpRequest = _mapper.Map<TopUpRequest>(createTopUpRequestDTO);
            topUpRequest.WalletTransaction = new()
            {
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                Status = WalletTransactionStatusEnum.PENDING.ToString(),
                Amount = topUpRequest.Amount,
                BuyerWalletId = buyerWallet.Id
            };

            var result = await _topUpRequestRepository.AddAsync(topUpRequest);
            return new ResponseMessage<Guid>()
            {
                message = "Tạo top up request thành công",
                result = true,
                value = (Guid) result
            };
        }

        public async Task<GetTopUpRequestDTO> GetTopUpRequestById(Guid id)
        {
            var existedTopUpRequest = await _topUpRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.WalletTransaction));

            if (existedTopUpRequest == null)
            {
                throw new NotFoundException("Không tìm thấy");
            }

            return _mapper.Map<GetTopUpRequestDTO>(existedTopUpRequest);
        }
    }
}
