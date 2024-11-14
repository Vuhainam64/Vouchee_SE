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
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<PartnerTransaction> _partnerTransactionRepository;
        private readonly IMapper _mapper;

        public PartnerTransactionService(IBaseRepository<User> userRepository, 
                                            IBaseRepository<Order> orderRepository, 
                                            IBaseRepository<PartnerTransaction> partnerTransactionRepository, 
                                            IMapper mapper)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _partnerTransactionRepository = partnerTransactionRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreatePartnerTransactionAsync(CreateSePayPartnerInTransactionDTO createPartnerTransaction)
        {
            try
            {
                Regex orderRegex = new Regex(@"\bORDER-([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");
                Regex topupRegex = new Regex(@"\bTOPUP-([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

                PartnerTransaction partnerTransaction = _mapper.Map<PartnerTransaction>(createPartnerTransaction);
                partnerTransaction.PartnerTransactionId = createPartnerTransaction.id;
                partnerTransaction.PartnerName = "SePAY";

                if (createPartnerTransaction.transferType == PartnerTransactionTypeEnum.@in.ToString())
                {
                    partnerTransaction.AmountIn = createPartnerTransaction.transferAmount;
                }

                var partnerTransactionId = await _partnerTransactionRepository.AddAsync(partnerTransaction);

                if (partnerTransactionId != Guid.Empty)
                {
                    if (createPartnerTransaction.content != null)
                    {
                        // ORDER
                        Match orderMatch = orderRegex.Match(createPartnerTransaction.content);
                        if (orderMatch.Success && orderMatch.Groups.Count > 1)
                        {
                            string orderId = orderMatch.Groups[1].Value;
                            if (orderId == null)
                            {
                                throw new NotFoundException($"Không tìm thấy mã đơn hàng {orderId} trong content");
                            }

                            var existedOrder = await _orderRepository.GetByIdAsync(Guid.Parse(orderId), includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                                                        .ThenInclude(x => x.Modal.Voucher.Seller)
                                                                                                                                    .Include(x => x.Buyer)
                                                                                                                                        .ThenInclude(x => x.BuyerWallet), 
                                                                                                                                    isTracking: true);

                            if (existedOrder == null)
                            {
                                throw new NotFoundException($"Không tìm thấy order {orderId} trong db");
                            }

                            existedOrder.PartnerTransactionId = partnerTransactionId;
                            existedOrder.Status = OrderStatusEnum.PAID.ToString();

                            // trừ tiền của người mua 
                            if (existedOrder.UsedBalance > 0)
                            {

                                existedOrder.Buyer.BuyerWallet.Balance -= existedOrder.UsedBalance;
                                existedOrder.Buyer.BuyerWallet.BuyerWalletTransactions.Add(new()
                                {
                                    Type = false,
                                    CreateBy = existedOrder.Buyer.Id,
                                    CreateDate = DateTime.Now,
                                    Status = WalletTransactionStatusEnum.DONE.ToString(),
                                    Amount = existedOrder.Buyer.BuyerWallet.Balance,
                                });
                            }

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
                                    Type = true,
                                    CreateBy = existedOrder.Buyer.Id,
                                    CreateDate = DateTime.Now,
                                    Status = WalletTransactionStatusEnum.DONE.ToString(),
                                    Amount = amount,
                                    OrderId = existedOrder.Id,
                                });
                            }
                        }

                        await _userRepository.SaveChanges();

                        if (await _orderRepository.SaveChanges())
                        {
                            return new ResponseMessage<Guid>()
                            {
                                message = "Tạo transaction thành công",
                                result = true,
                                value = (Guid)partnerTransactionId,
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
