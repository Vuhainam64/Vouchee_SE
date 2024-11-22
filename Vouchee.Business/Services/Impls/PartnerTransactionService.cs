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

namespace Vouchee.Business.Services.Impls
{
    public class PartnerTransactionService : IPartnerTransactionService
    {
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<MoneyRequest> _topUpRequestRepository;
        private readonly IBaseRepository<Wallet> _wallerRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<PartnerTransaction> _partnerTransactionRepository;
        private readonly IMapper _mapper;

        public PartnerTransactionService(IBaseRepository<Notification> notificationRepository,
                                         IBaseRepository<Voucher> voucherRepository,
                                         IBaseRepository<MoneyRequest> topUpRequestRepository,
                                         IBaseRepository<Wallet> wallerRepository,
                                         IBaseRepository<User> userRepository,
                                         IBaseRepository<Order> orderRepository,
                                         IBaseRepository<PartnerTransaction> partnerTransactionRepository,
                                         IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _voucherRepository = voucherRepository;
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
                                                                                                                                .ThenInclude(x => x.BuyerWallet)
                                                                                                                             .Include(x => x.OrderDetails)
                                                                                                                                .ThenInclude(x => x.Modal)
                                                                                                                                    .ThenInclude(x => x.Voucher),
                                                                                                                             isTracking: true);

                            if (existedOrder == null)
                            {
                                throw new NotFoundException($"Không tìm thấy order {orderId} trong db");
                            }

                            if (DateTime.Now > existedOrder.CreateDate.Value.AddMinutes(2))
                            {
                                throw new ConflictException($"Order này đã hết hạn lúc {existedOrder.CreateDate.Value.AddMinutes(2)}");
                            }

                            await _notificationRepository.Add(new()
                            {
                                CreateBy = existedOrder.CreateBy,
                                CreateDate = DateTime.Now,
                                Title = "THÔNG BÁO TRẠNG THÁI ĐƠN HÀNG",
                                Description = $"Đơn hàng {orderId} của bạn đã thanh toán",
                                ReceiverId = existedOrder.CreateBy,
                                Seen = false,
                            });

                            foreach (var orderDetail in existedOrder.OrderDetails.GroupBy(x => x.Modal.VoucherId))
                            {
                                var result = false;

                                // gop tung modal co cung voucher id
                                var existedVoucher = await _voucherRepository.GetByIdAsync(orderDetail.Key,
                                                                                                    isTracking: true,
                                                                                                    includeProperties: x => x.Include(x => x.Modals)
                                                                                                                                .ThenInclude(x => x.Carts));

                                // duyet tung modal 
                                foreach (var cartModal in orderDetail)
                                {
                                    var existedModal = existedVoucher.Modals.FirstOrDefault(x => x.Id == cartModal.ModalId);

                                    // kiem tra ton kho cua modal
                                    if (cartModal.Quantity > existedModal?.Stock)
                                    {
                                        throw new ConflictException($"Bạn đặt {cartModal.Quantity} {cartModal.Modal.Title} nhưng trong khi chỉ còn {existedModal.Stock}");
                                    }

                                    existedModal.Stock -= cartModal.Quantity;
                                    existedVoucher.Stock -= cartModal.Quantity;

                                    existedModal.Carts.Remove(existedModal.Carts.FirstOrDefault(c => c.ModalId == cartModal.ModalId));
                                }

                                await _voucherRepository.UpdateAsync(existedVoucher);
                            }

                            // Update order details
                            existedOrder.PartnerTransactionId = partnerTransactionId;
                            existedOrder.Status = OrderStatusEnum.PAID.ToString();

                            if (existedOrder.UsedVPoint > 0)
                            {
                                existedOrder.Buyer.VPoint -= existedOrder.UsedVPoint;
                            }

                            existedOrder.Buyer.VPoint += existedOrder.VPointUp;

                            if (existedOrder.UsedBalance > 0)
                            {
                                existedOrder.Buyer.BuyerWallet.BuyerWalletTransactions.Add(new()
                                {
                                    Type = "AMOUNT_OUT",
                                    CreateBy = existedOrder.Buyer.Id,
                                    CreateDate = DateTime.Now,
                                    Status = WalletTransactionStatusEnum.DONE.ToString(),
                                    Amount = existedOrder.UsedBalance,
                                    OrderId = existedOrder.Id,
                                    BeforeBalance = existedOrder.Buyer.BuyerWallet.Balance,
                                    AfterBalance = existedOrder.Buyer.BuyerWallet.Balance - existedOrder.UsedBalance,
                                });
                                existedOrder.Buyer.BuyerWallet.Balance -= existedOrder.UsedBalance;
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

                                string description = $"Đơn hàng số {existedOrder.Id}\n";

                                foreach (var modal in seller)
                                {
                                    description += $"Modal: {modal.ModalId} - {modal.Quantity}\n";
                                }

                                Notification sellerNotification = new()
                                {
                                    ReceiverId = existedSeller.Id,
                                    CreateBy = existedOrder.CreateBy,
                                    CreateDate = DateTime.Now,
                                    Title = "THÔNG BÁO CÓ ĐƠN HÀNG MỚI",
                                    Description = description,
                                    Seen = false,
                                };

                                await _notificationRepository.AddAsync(sellerNotification);
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
