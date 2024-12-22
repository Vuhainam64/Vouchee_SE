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
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IMapper _mapper;

        public WithdrawService(IBaseRepository<WalletTransaction> walletTransactionRepository,
                               IBaseRepository<User> userRepository,
                               IBaseRepository<MoneyRequest> moneyRequestRepository,
                               IMapper mapper)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _userRepository = userRepository;
            _moneyRequestRepository = moneyRequestRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<string>> CreateWithdrawRequestAsync(WalletTypeEnum walletTypeEnum, CreateWithdrawRequestDTO createWithdrawRequestDTO, ThisUserObj thisUserObj)
        {
            var generateId = Guid.NewGuid();
            var existedUser = await _userRepository.FindAsync(thisUserObj.userId, isTracking: true);

            MoneyRequest moneyRequest = new()
            {
                Status = MoneyRequestEnum.PENDING.ToString(),
                Amount = createWithdrawRequestDTO.amount,
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                UserId = thisUserObj.userId,
                Type = MoneyRequestTypeEnum.WITHDRAW.ToString(),
            };

            WalletTransaction walletTransaction = new()
            {
                Status = WalletTransactionStatusEnum.PENDING.ToString(),
                Type = MoneyRequestTypeEnum.WITHDRAW.ToString(),
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
            };

            moneyRequest.WithdrawWalletTransaction = walletTransaction;

            if (walletTypeEnum == WalletTypeEnum.BUYER)
            {
                if (createWithdrawRequestDTO.amount > existedUser.BuyerWallet.Balance)
                {
                    throw new ConflictException($"Số tiền trong ví hiện tại không đủ, số dư hiện tại {existedUser.BuyerWallet.Balance}");
                }

                moneyRequest.WithdrawWalletTransaction.BuyerWalletId = existedUser.BuyerWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví mua";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.BuyerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.BuyerWallet.Balance - createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.UpdateId = generateId;

                existedUser.BuyerWallet.Balance -= createWithdrawRequestDTO.amount;
                existedUser.BuyerWallet.UpdateDate = DateTime.Now;
                existedUser.BuyerWallet.UpdateBy = thisUserObj.userId;
            }
            else if (walletTypeEnum == WalletTypeEnum.SELLER)
            {
                if (createWithdrawRequestDTO.amount > existedUser.BuyerWallet.Balance)
                {
                    throw new ConflictException($"Số tiền trong ví hiện tại không đủ, số dư hiện tại {existedUser.SellerWallet.Balance}");
                }

                moneyRequest.WithdrawWalletTransaction.SellerWalletId = existedUser.SellerWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví bán";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.SellerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.SellerWallet.Balance - createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.UpdateId = generateId;

                existedUser.SellerWallet.Balance -= createWithdrawRequestDTO.amount;
                existedUser.SellerWallet.UpdateDate = DateTime.Now;
                existedUser.SellerWallet.UpdateBy = thisUserObj.userId;
            }
            else if (walletTypeEnum == WalletTypeEnum.SUPPLIER)
            {
                if (createWithdrawRequestDTO.amount > existedUser.BuyerWallet.Balance)
                {
                    throw new ConflictException($"Số tiền trong ví hiện tại không đủ, số dư hiện tại {existedUser.Supplier.SupplierWallet.Balance}");
                }

                moneyRequest.WithdrawWalletTransaction.SupplierWalletId = existedUser.Supplier.SupplierWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví supplier";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.SellerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.SellerWallet.Balance - createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.UpdateId = generateId;

                existedUser.Supplier.SupplierWallet.Balance -= createWithdrawRequestDTO.amount;
                existedUser.Supplier.SupplierWallet.UpdateDate = DateTime.Now;
                existedUser.Supplier.SupplierWallet.UpdateBy = thisUserObj.userId;
            }

            await _userRepository.SaveChanges();

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

        public async Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetWithdrawRequestDTO>) result;

            result = _moneyRequestRepository.GetTable()
                        .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()) && x.UserId == thisUserObj.userId)
                        .ProjectTo<GetWithdrawRequestDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetWithdrawRequestDTO>(withdrawRequestFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetWithdrawRequestDTO>()
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

        public async Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id)
        {
            var existedWithdrawRequest = await _moneyRequestRepository.GetByIdAsync(id);
            if (existedWithdrawRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu rút tiền này");
            }

            return _mapper.Map<GetWithdrawRequestDTO>(existedWithdrawRequest);
        }

        public async Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWithdrawWalletTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable()
                        .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()) && x.BuyerWallet.BuyerId == thisUserObj.userId)
                        .ProjectTo<GetWalletTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetWalletTransactionDTO>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetWalletTransactionDTO>()
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

        public async Task<ResponseMessage<bool>> UpdateWithdrawRequest(string id, WithdrawRequestStatusEnum withdrawRequestStatusEnum, ThisUserObj thisUserObj)
        {
            var withdrawRequest = await _moneyRequestRepository.GetByIdAsync(id, isTracking: true);

            if (withdrawRequest == null)
            {
                throw new NotFoundException("Không tìm thấy request với id này");
            }

            withdrawRequest.Status = withdrawRequestStatusEnum.ToString();
            withdrawRequest.UpdateDate = DateTime.Now;
            withdrawRequest.UpdateBy = thisUserObj.userId;

            await _moneyRequestRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = true
            };
        }
    }
}
