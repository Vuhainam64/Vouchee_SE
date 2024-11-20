using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;

namespace Vouchee.Business.Services.Impls
{
    public class PartnerTransactionService : IPartnerTransactionService
    {
        private readonly IBaseRepository<MoneyRequest> _topUpRequestRepository;
        private readonly IBaseRepository<Wallet> _wallerRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<PartnerTransaction> _partnerTransactionRepository;
        private readonly IMapper _mapper;

        public PartnerTransactionService(IBaseRepository<MoneyRequest> topUpRequestRepository,
                                         IBaseRepository<Wallet> wallerRepository,
                                         IBaseRepository<User> userRepository,
                                         IBaseRepository<Order> orderRepository,
                                         IBaseRepository<PartnerTransaction> partnerTransactionRepository,
                                         IMapper mapper)
        {
            _topUpRequestRepository = topUpRequestRepository;
            _wallerRepository = wallerRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _partnerTransactionRepository = partnerTransactionRepository;
            _mapper = mapper;
        }

        public async Task<dynamic> CreatePartnerTransactionAsync(CreateSePayPartnerInTransactionDTO createPartnerTransaction)
        {
            try
            {
                string input = createPartnerTransaction.code;

                // Unified regex for ORDER and TOPUP
                Regex regex = new Regex(@"\b(ORD|TOP)([a-zA-Z0-9]{1,})\b");

                PartnerTransaction partnerTransaction = _mapper.Map<PartnerTransaction>(createPartnerTransaction);
                partnerTransaction.PartnerTransactionId = createPartnerTransaction.id;
                partnerTransaction.PartnerName = "SePAY";

                if (createPartnerTransaction.transferType == PartnerTransactionTypeEnum.@in.ToString())
                {
                    partnerTransaction.AmountIn = createPartnerTransaction.transferAmount;
                }

                var partnerTransactionId = await _partnerTransactionRepository.AddAsync(partnerTransaction);

                if (partnerTransactionId != Guid.Empty && !string.IsNullOrEmpty(input))
                {
                    // Match and replace the content
                    Match match = regex.Match(input);
                    if (match.Success)
                    {
                        string transactionType = match.Groups[1].Value; // ORDER or TOPUP
                        string identifier = match.Groups[2].Value;     // The alphanumeric identifier

                        // Replace the content in the input
                        string convertedContent = $"{transactionType}-{identifier}";
                        createPartnerTransaction.content = convertedContent;

                        // Proceed based on transaction type
                        if (transactionType == "ORD")
                        {
                            string orderId = identifier;

                            var existedOrder = await _orderRepository.GetByIdAsync(orderId, includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                                                 .ThenInclude(x => x.Modal.Voucher.Seller)
                                                                                                                             .Include(x => x.Buyer)
                                                                                                                                 .ThenInclude(x => x.BuyerWallet),
                                                                                                                             isTracking: true);

                            if (existedOrder == null)
                            {
                                throw new NotFoundException($"Không tìm thấy order {orderId} trong db");
                            }

                            // Update order details
                            existedOrder.PartnerTransactionId = partnerTransactionId;
                            existedOrder.Status = OrderStatusEnum.PAID.ToString();

                            if (existedOrder.UsedVPoint > 0)
                            {
                                existedOrder.Buyer.VPoint -= existedOrder.UsedVPoint;
                            }

                            existedOrder.Buyer.VPoint += existedOrder.FinalPrice;

                            if (existedOrder.UsedBalance > 0)
                            {
                                existedOrder.Buyer.BuyerWallet.Balance -= existedOrder.UsedBalance;
                                existedOrder.Buyer.BuyerWallet.BuyerWalletTransactions.Add(new()
                                {
                                    Type = "AMOUNT_OUT",
                                    CreateBy = existedOrder.Buyer.Id,
                                    CreateDate = DateTime.Now,
                                    Status = WalletTransactionStatusEnum.DONE.ToString(),
                                    Amount = existedOrder.UsedBalance,
                                    OrderId = existedOrder.Id
                                });
                            }

                            // Process seller wallet updates
                            foreach (var seller in existedOrder.OrderDetails.GroupBy(x => x.Modal.Voucher.SellerId))
                            {
                                var amount = seller.Sum(x => x.FinalPrice);

                                var existedSeller = await _userRepository.GetByIdAsync(seller.Key, includeProperties: x => x.Include(x => x.SellerWallet), isTracking: true);

                                if (existedSeller.SellerWallet == null)
                                {
                                    throw new NotFoundException($"{existedSeller.Id} chưa có ví seller");
                                }

                                existedSeller.SellerWallet.Balance += amount;
                                existedSeller.SellerWallet.SellerWalletTransactions.Add(new()
                                {
                                    Type = "AMOUNT_IN",
                                    CreateBy = existedOrder.Buyer.Id,
                                    CreateDate = DateTime.Now,
                                    Status = WalletTransactionStatusEnum.DONE.ToString(),
                                    Amount = amount,
                                    OrderId = existedOrder.Id,
                                });
                            }

                            await _userRepository.SaveChanges();

                            await _orderRepository.SaveChanges();

                            return new
                            {
                                message = "Tạo transaction thành công",
                                result = true,
                                value = partnerTransactionId,
                                success = true
                            };
                        }
                        else if (transactionType == "TOP")
                        {
                            string topUpRequestId = identifier;

                            var existedTopUpRequest = await _topUpRequestRepository.GetByIdAsync(topUpRequestId, includeProperties: x => x.Include(x => x.TopUpWalletTransaction)
                                                                                                                                 .ThenInclude(x => x.BuyerWallet),
                                                                                                                                 isTracking: true);

                            if (existedTopUpRequest == null)
                            {
                                throw new NotFoundException($"Không tìm thấy top up request của người này");
                            }

                            existedTopUpRequest.Status = WalletTransactionStatusEnum.PAID.ToString();
                            existedTopUpRequest.UpdateDate = DateTime.Now;

                            existedTopUpRequest.TopUpWalletTransaction.PartnerTransactionId = partnerTransactionId;
                            existedTopUpRequest.TopUpWalletTransaction.UpdateDate = DateTime.Now;
                            existedTopUpRequest.TopUpWalletTransaction.Status = WalletTransactionStatusEnum.PAID.ToString();
                            existedTopUpRequest.TopUpWalletTransaction.BuyerWallet.Balance += (int)partnerTransaction.AmountIn;
                            existedTopUpRequest.TopUpWalletTransaction.BuyerWallet.UpdateDate = DateTime.Now;

                            await _topUpRequestRepository.SaveChanges();

                            return new
                            {
                                message = "Tạo transaction thành công",
                                result = true,
                                value = partnerTransactionId,
                                success = true
                            };
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                throw new Exception("An error occurred during transaction processing.", ex);
            }
        }
    }
}
