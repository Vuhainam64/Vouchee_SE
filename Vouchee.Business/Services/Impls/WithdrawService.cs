using AutoMapper;
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
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IMapper _mapper;

        public WithdrawService(IBaseRepository<User> userRepository,
                               IBaseRepository<MoneyRequest> moneyRequestRepository,
                               IMapper mapper)
        {
            _userRepository = userRepository;
            _moneyRequestRepository = moneyRequestRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<string>> CreateWithdrawRequestAsync(WalletTypeEnum walletTypeEnum, CreateWithdrawRequestDTO createWithdrawRequestDTO, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.FindAsync(thisUserObj.userId);

            MoneyRequest moneyRequest = new()
            {
                Status = MoneyRequestEnum.PENDING.ToString(),
                Amount = createWithdrawRequestDTO.amount,
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                UserId = thisUserObj.userId,
            };

            WalletTransaction walletTransaction = new()
            {
                Status = WalletTransactionStatusEnum.PENDING.ToString(),
                Type = "AMOUNT_OUT",
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
            };

            moneyRequest.WithdrawWalletTransaction = walletTransaction;

            if (walletTypeEnum == WalletTypeEnum.BUYER)
            {
                moneyRequest.WithdrawWalletTransaction.BuyerWalletId = existedUser.BuyerWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví mua";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.BuyerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.BuyerWallet.Balance - createWithdrawRequestDTO.amount;
            }
            else
            {
                moneyRequest.WithdrawWalletTransaction.SellerWalletId = existedUser.SellerWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví bán";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.SellerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.SellerWallet.Balance - createWithdrawRequestDTO.amount;
            }

            string? result = await _moneyRequestRepository.AddReturnString(moneyRequest);

            return new ResponseMessage<string>()
            {
                message = "Tạo yêu cầu rút tiền thành công",
                result = true,
                value = result
            };
        }

        public Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter)
        {
            throw new NotImplementedException();
        }

        public Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public async Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id)
        {
            var existedWithdrawRequest = await _moneyRequestRepository.GetByIdAsync(id);
            if (existedWithdrawRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu rút tiền này");
            }

            return _mapper.Map<GetWithdrawRequestDTO>(existedWithdrawRequest);
        }
    }
}
