using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class PartnerTransactionService : IPartnerTransactionService
    {
        private readonly ISendEmailService _sendEmailService;
        private readonly INotificationService _notificationService;

        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IBaseRepository<Wallet> _wallerRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<PartnerTransaction> _partnerTransactionRepository;
        private readonly IMapper _mapper;

        public PartnerTransactionService(IBaseRepository<Supplier> supplierRepository,
                                         ISendEmailService sendEmailService,
                                         INotificationService notificationService,
                                         IBaseRepository<VoucherCode> voucherCodeRepository,
                                         IBaseRepository<Notification> notificationRepository,
                                         IBaseRepository<Voucher> voucherRepository,
                                         IBaseRepository<MoneyRequest> topUpRequestRepository,
                                         IBaseRepository<Wallet> wallerRepository,
                                         IBaseRepository<User> userRepository,
                                         IBaseRepository<Order> orderRepository,
                                         IBaseRepository<PartnerTransaction> partnerTransactionRepository,
                                         IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _sendEmailService = sendEmailService;
            _notificationService = notificationService;
            _voucherCodeRepository = voucherCodeRepository;
            _notificationRepository = notificationRepository;
            _voucherRepository = voucherRepository;
            _moneyRequestRepository = topUpRequestRepository;
            _wallerRepository = wallerRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _partnerTransactionRepository = partnerTransactionRepository;
            _mapper = mapper;
        }

        public async Task<dynamic> CreatePartnerTransactionAsync(CreateSePayPartnerInTransactionDTO createPartnerTransaction)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            string input = createPartnerTransaction.code;

            // Unified regex for ORDER and TOPUP
            Regex regex = new Regex(@"\b(ORD|TOP|WIT)([a-zA-Z0-9]{1,})\b");

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
                                                                                                                                .ThenInclude(x => x.BuyerWalletTransactions)
                                                                                                                         .Include(x => x.OrderDetails)
                                                                                                                            .ThenInclude(x => x.Modal)
                                                                                                                                .ThenInclude(x => x.Voucher),
                                                                                                                         isTracking: true);

                        if (existedOrder == null)
                        {
                            throw new NotFoundException($"Không tìm thấy order {orderId} trong db");
                        }

                        if (partnerTransaction.TransactionDate > existedOrder.CreateDate?.AddMinutes(5))
                        {
                            CreateNotificationDTO errorNoti = new()
                            {
                                title = "Thanh toán đơn hàng lỗi",
                                body = $"Đơn hàng {orderId} đã hết hạn lúc {existedOrder.CreateDate.Value.AddMinutes(5)}, số tiền bạn đã nạp trước đó sẽ được hoàn về ví mua và số dư vpoint và số dư ví sẽ không bị trừ",
                                receiverId = existedOrder.Buyer.Id
                            };

                            existedOrder.Status = OrderStatusEnum.EXPIRED.ToString();
                            existedOrder.UpdateDate = DateTime.Now;
                            existedOrder.Buyer.BuyerWallet.UpdateDate = DateTime.Now;
                            existedOrder.Buyer.BuyerWallet.BuyerWalletTransactions.Add(new()
                            {
                                Status = BuyerWalletTransactionStatusEnum.TRANSACTION_SUCCESS.ToString(),
                                Type = WalletTransactionTypeEnum.EXPIRED_ORDER.ToString(),
                                BeforeBalance = existedOrder.Buyer.BuyerWallet.Balance,
                                Amount = (int) createPartnerTransaction.transferAmount,
                                AfterBalance = existedOrder.Buyer.BuyerWallet.Balance + (int)createPartnerTransaction.transferAmount,
                                CreateDate = DateTime.Now,
                                Note = "Hoàn tiền về ví do hết hạn giao dịch",
                                PartnerTransactionId = partnerTransactionId,
                                OrderId = existedOrder.Id,
                            });
                            existedOrder.Buyer.BuyerWallet.Balance += (int)createPartnerTransaction.transferAmount;
                            existedOrder.Note = $"Đơn hàng hủy do hết hạn giao dịch, đã hoàn {createPartnerTransaction.transferAmount}";

                            foreach (var orderDetail in existedOrder.OrderDetails)
                            {
                                if (orderDetail.ShopPromotion != null)
                                {
                                    orderDetail.ShopPromotion.Stock += 1;
                                    orderDetail.ShopPromotion.UpdateDate = DateTime.Now;
                                }
                            }

                            await _sendEmailService.SendEmailAsync(existedOrder.Buyer.Email, "Trạng thái đơn hàng", $"Đơn hàng {existedOrder.Id} lỗi do hết thời gian");

                            await _notificationService.CreateNotificationAsync(errorNoti);

                            throw new ConflictException($"Order này đã hết hạn lúc {existedOrder.CreateDate.Value.AddMinutes(2)}");
                        }

                        CreateNotificationDTO successNoti = new()
                        {
                            title = "Thanh toán đơn hàng thành công",
                            body = $"Đơn hàng {orderId}",
                            receiverId = existedOrder.Buyer.Id
                        };

                        WalletTransaction walletTransaction = new()
                        {
                            Status = "PAID",
                            Type = WalletTransactionTypeEnum.ADMIN.ToString(),
                            Amount = existedOrder.FinalPrice * 10 / 100,
                            CreateDate = DateTime.Now,
                            Note = $"Ngân hàng của Nam được nhận { existedOrder.FinalPrice * 10 / 100 } từ đơn hàng { existedOrder.Id }",
                        };

                        existedOrder.WalletTransactions.Add( walletTransaction );

                        await _notificationService.CreateNotificationAsync(successNoti);

                        await _sendEmailService.SendEmailAsync("advouchee@gmail.com", "Tiền về ngân hàng của Nam", $"Ngân hàng của Nam được nhận {existedOrder.FinalPrice * 10 / 100} từ đơn hàng {existedOrder.Id}");
                        
                        foreach (var orderDetail in existedOrder.OrderDetails.GroupBy(x => x.Modal.VoucherId))
                        {
                            // gop tung modal co cung voucher id
                            var existedVoucher = await _voucherRepository.GetByIdAsync(orderDetail.Key,
                                                                                                isTracking: true,
                                                                                                includeProperties: x => x.Include(x => x.Modals)
                                                                                                                            .ThenInclude(x => x.Carts)
                                                                                                                          .Include(x => x.Modals)
                                                                                                                            .ThenInclude(x => x.VoucherCodes));

                            // duyet tung modal 
                            foreach (var cartModal in orderDetail)
                            {
                                var existedModal = existedVoucher.Modals.FirstOrDefault(x => x.Id == cartModal.ModalId);

                                // kiem tra ton kho cua modal
                                if (cartModal.Quantity > existedModal?.Stock)
                                {
                                    throw new ConflictException($"Bạn đặt {cartModal.Quantity} {cartModal.Modal.Title} nhưng trong khi chỉ còn {existedModal.Stock}. Tiền đã được hoàn trả về ví");
                                }

                                //existedModal.Stock -= cartModal.Quantity;

                                var voucherCodes = _voucherCodeRepository.GetTable()
                                                                            .Where(x => x.OrderId == null && x.ModalId == existedModal.Id && x.EndDate >= today)
                                                                            .Where(x => x.NewCode != null)
                                                                            .OrderBy(x => x.EndDate)
                                                                            .Take(cartModal.Quantity)
                                                                            .AsTracking();

                                foreach (var voucherCode in voucherCodes)
                                {
                                    voucherCode.OrderId = orderId;
                                    //voucherCode.Status = VoucherCodeStatusEnum.CONVERTING.ToString();
                                }

                                await _voucherCodeRepository.SaveChanges();

                                //existedVoucher.Stock -= cartModal.Quantity;

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
                                Type = WalletTransactionTypeEnum.BUYER_ORDER.ToString(),
                                CreateBy = existedOrder.Buyer.Id,
                                CreateDate = DateTime.Now,
                                Status = BuyerWalletTransactionStatusEnum.TRANSACTION_SUCCESS.ToString(),
                                Amount = existedOrder.UsedBalance,
                                OrderId = existedOrder.Id,
                                BeforeBalance = existedOrder.Buyer.BuyerWallet.Balance,
                                AfterBalance = existedOrder.Buyer.BuyerWallet.Balance - existedOrder.UsedBalance,
                                Note = $"Rút {existedOrder.UsedBalance} để thanh toán đơn hàng {existedOrder.Id} "
                            });
                            existedOrder.Buyer.BuyerWallet.Balance -= existedOrder.UsedBalance;
                        }

                        await _sendEmailService.SendEmailAsync(existedOrder.Buyer.Email, "Thông báo trạng thái đơn hàng", $"{existedOrder.Id} đã thanh toán thành công");

                        // Process seller wallet updates
                        foreach (var seller in existedOrder.OrderDetails.GroupBy(x => x.Modal.Voucher.SellerId))
                        {
                            var amount = seller.Sum(x => x.FinalPrice) - (seller.Sum(x => x.FinalPrice) * 10 / 100);

                            var existedSeller = await _userRepository.GetByIdAsync(seller.Key, includeProperties: x => x.Include(x => x.SellerWallet)
                                                                                                                            .ThenInclude(x => x.SellerWalletTransactions)
                                                                                                                            , isTracking: true);

                            if (existedSeller.SellerWallet == null)
                            {
                                throw new NotFoundException($"{existedSeller.Id} chưa có ví seller");
                            }

                            existedSeller.SellerWallet.SellerWalletTransactions.Add(new()
                            {
                                Type = WalletTransactionTypeEnum.SELLER_ORDER.ToString(),
                                CreateBy = existedOrder.Buyer.Id,
                                CreateDate = DateTime.Now,
                                Status = SellerWalletTransactionStatusEnum.DONE.ToString(),
                                Amount = amount,
                                OrderId = existedOrder.Id,
                                BeforeBalance = existedSeller.SellerWallet.Balance,
                                AfterBalance = existedSeller.SellerWallet.Balance + amount,
                                Note = $"Nhận {amount} từ đơn {existedOrder.Id}"
                            });

                            existedSeller.SellerWallet.Balance += amount;

                            string description = $"Đơn hàng số {existedOrder.Id}\n";

                            foreach (var modal in seller)
                            {
                                description += $"Modal: {modal.ModalId} - số lượng: {modal.Quantity}\n";
                            }

                            description += $"Tổng tiền sau khi trừ 10% tiền dịch vụ: {amount}";

                            Notification sellerNotification = new()
                            {
                                ReceiverId = existedSeller.Id,
                                //CreateBy = existedOrder.CreateBy,
                                CreateDate = DateTime.Now,
                                Title = "THÔNG BÁO CÓ ĐƠN HÀNG MỚI",
                                Body = description,
                                Seen = false,
                            };

                            await _sendEmailService.SendEmailAsync(existedSeller.Email, "Thông báo có đơn hàng mới từ người mua", $"{description} và đã được chuyển {amount} về ví");

                            await _notificationRepository.AddAsync(sellerNotification);
                        }

                        foreach (var supplier in existedOrder.OrderDetails.GroupBy(x => x.Modal.Voucher.SupplierId))
                        {
                            var amount = supplier.Sum(x => x.FinalPrice) * 10 / 100;

                            var existedSupplier = await _supplierRepository.GetByIdAsync(supplier.Key, includeProperties: x => x.Include(x => x.SupplierWallet)
                                                                                                                                        .ThenInclude(x => x.SupplierWalletTransactions)
                                                                                                                                    , isTracking: true);

                            existedSupplier.SupplierWallet.SupplierWalletTransactions.Add(new()
                            {
                                Status = WalletTransactionStatusEnum.PAID.ToString(),
                                Type = WalletTransactionTypeEnum.SUPPLIER_ORDER.ToString(),
                                OrderId = existedOrder.Id,
                                BeforeBalance = existedSupplier.SupplierWallet.Balance,
                                Amount = amount,
                                AfterBalance = existedSupplier.SupplierWallet.Balance + amount,
                                CreateDate = DateTime.Now
                            });
                            existedSupplier.SupplierWallet.Balance += amount;

                            foreach (var supplierToSendEmail in existedSupplier.Users)
                            {
                                await _sendEmailService.SendEmailAsync(supplierToSendEmail.Email, $"Số tiền {amount} được thanh toán", $"Nhà cung cấp {existedSupplier.Name} đã được trả {amount} cho đơn hàng {existedOrder.Id}");
                            }

                            await _supplierRepository.SaveChanges();
                        }

                        existedOrder.Note = "Đơn hàng thanh toán thành công";
                        existedOrder.UpdateDate = DateTime.Now;

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

                        var existedTopUpRequest = await _moneyRequestRepository.GetByIdAsync(topUpRequestId, includeProperties: x => x.Include(x => x.User.BuyerWallet), isTracking: true);

                        if (existedTopUpRequest == null)
                        {
                            throw new NotFoundException($"Không tìm thấy top up request của người này");
                        }

                        existedTopUpRequest.Status = WalletTransactionStatusEnum.PAID.ToString();
                        existedTopUpRequest.UpdateDate = DateTime.Now;
                        existedTopUpRequest.TopUpWalletTransaction = new()
                        {
                            PartnerTransactionId = partnerTransactionId,
                            CreateDate = DateTime.Now,
                            Status = WalletTransactionStatusEnum.PAID.ToString(),
                            Type = WalletTransactionTypeEnum.TOPUP.ToString(),
                            AfterBalance = existedTopUpRequest.User.BuyerWallet.Balance + existedTopUpRequest.Amount,
                            Amount = existedTopUpRequest.Amount,
                            BeforeBalance = existedTopUpRequest.User.BuyerWallet.Balance,
                            BuyerWalletId = existedTopUpRequest.User.BuyerWallet.Id,
                            Note = $"Nạp {existedTopUpRequest.Amount} vào ví mua",
                        };

                        existedTopUpRequest.User.BuyerWallet.Balance += existedTopUpRequest.Amount;
                        existedTopUpRequest.User.BuyerWallet.UpdateDate = DateTime.Now;

                        await _sendEmailService.SendEmailAsync(existedTopUpRequest.User.Email, "Cập nhật biến động số dư", $"Bạn đã nạp thành công {existedTopUpRequest.Amount} vào tài khoản");

                        await _moneyRequestRepository.SaveChanges();

                        return new
                        {
                            message = "Tạo transaction thành công",
                            result = true,
                            value = partnerTransactionId,
                            success = true
                        };
                    }
                    else if (transactionType == "WIT")
                    {
                        var existedWithdrawRequest = await _moneyRequestRepository.GetByIdAsync(identifier, includeProperties: x => x.Include(x => x.WithdrawWalletTransaction)
                                                                                                                                        .ThenInclude(x => x.BuyerWallet)
                                                                                                                                            .ThenInclude(x => x.Buyer)
                                                                                                                                        .Include(x => x.WithdrawWalletTransaction)
                                                                                                                                            .ThenInclude(x => x.SellerWallet)
                                                                                                                                                .ThenInclude(x => x.Seller)
                                                                                                                                                , isTracking: true);

                        if (existedWithdrawRequest == null)
                        {
                            throw new NotFoundException("Không tìm thấy lệnh rút tiền này");
                        }

                        existedWithdrawRequest.Status = "PAID";
                        existedWithdrawRequest.UpdateDate = DateTime.Now;

                        existedWithdrawRequest.WithdrawWalletTransaction.Status = "PAID";
                        existedWithdrawRequest.WithdrawWalletTransaction.PartnerTransactionId = partnerTransactionId;

                        if (existedWithdrawRequest.WithdrawWalletTransaction.BuyerWallet != null)
                        {
                            existedWithdrawRequest.WithdrawWalletTransaction.BuyerWallet.Balance -= (int) createPartnerTransaction.transferAmount;
                            existedWithdrawRequest.WithdrawWalletTransaction.BuyerWallet.UpdateDate = DateTime.Now;

                            await _sendEmailService.SendEmailAsync(existedWithdrawRequest.WithdrawWalletTransaction.BuyerWallet.Buyer.Email, "Cập nhật trạng thái rút tiền", $"Yêu cầu rút {existedWithdrawRequest.Amount} đã được rút từ ví mua về ngân hàng của bạn");
                        }
                        else if (existedWithdrawRequest.WithdrawWalletTransaction.SellerWallet != null)
                        {
                            existedWithdrawRequest.WithdrawWalletTransaction.SellerWallet.Balance -= (int)createPartnerTransaction.transferAmount;
                            existedWithdrawRequest.WithdrawWalletTransaction.SellerWallet.UpdateDate = DateTime.Now;

                            await _sendEmailService.SendEmailAsync(existedWithdrawRequest.WithdrawWalletTransaction.SellerWallet.Seller.Email, "Cập nhật trạng thái rút tiền", $"Yêu cầu rút {existedWithdrawRequest.Amount} đã được rút từ ví bán về ngần hàng của bạn");
                        }

                        await _moneyRequestRepository.SaveChanges();

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

        public async Task<DynamicResponseModel<GetPartnerTransactionDTO>> GetPartnerTransactionAsync(PagingRequest pagingRequest, PartnerTransactionFilter partnerTransactionFilter)
        {
            (int, IQueryable<GetPartnerTransactionDTO>) result;

            result = _partnerTransactionRepository.GetTable()
                        .ProjectTo<GetPartnerTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetPartnerTransactionDTO>(partnerTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetPartnerTransactionDTO>()
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
    }
}
