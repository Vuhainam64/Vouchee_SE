using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class OrderService : IOrderService
    {
        private readonly ISendEmailService _sendEmailService; 
        private readonly INotificationService _notificationService;
        private readonly ICartService _cartService;

        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly IBaseRepository<Promotion> _promotionRepository;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<OrderDetail> _orderDetailRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(ISendEmailService sendEmailService,
                            INotificationService notificationService,
                            ICartService cartService,
                            IBaseRepository<Supplier> supplierRepository,
                            IBaseRepository<Notification> notificationRepository,
                            IBaseRepository<Promotion> promotionRepository,
                            IBaseRepository<Modal> modalRepository,
                            IBaseRepository<User> userRepository,
                            IBaseRepository<VoucherCode> voucherCodeRepository,
                            IBaseRepository<OrderDetail> orderDetailRepository,
                            IBaseRepository<Voucher> voucherRepository,
                            IBaseRepository<Order> orderRepository,
                            IMapper mapper)
        {
            _sendEmailService = sendEmailService;
            _notificationService = notificationService;
            _cartService = cartService;
            _supplierRepository = supplierRepository;
            _notificationRepository = notificationRepository;
            _promotionRepository = promotionRepository;
            _modalRepository = modalRepository;
            _userRepository = userRepository;
            _voucherCodeRepository = voucherCodeRepository;
            _orderDetailRepository = orderDetailRepository;
            _voucherRepository = voucherRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<string>> CreateOrderAsync(ThisUserObj thisUserObj, CheckOutViewModel checkOutViewModel)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var user = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.BuyerWallet), isTracking: true);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (user.BuyerWallet == null)
            {
                throw new NotFoundException("Người dùng này chưa có ví buyer");
            }

            DetailCartDTO cartDTO = await _cartService.GetCheckoutCartsAsync(thisUserObj, checkOutViewModel);

            Order order = new()
            {
                Status = OrderStatusEnum.PENDING.ToString()
            };

            if (cartDTO.finalPrice == 0)
            {
                order.Status = OrderStatusEnum.PAID.ToString();
                order.CreateBy = thisUserObj.userId;
                order.CreateDate = DateTime.Now;
                order.OrderDetails = [];
                order.Note = "Đơn hàng thanh toán thành công";
            }
            else
            {
                order.Status = OrderStatusEnum.PENDING.ToString();
                order.CreateBy = thisUserObj.userId;
                order.CreateDate = DateTime.Now;
                order.OrderDetails = [];
                order.Note = "Đơn hàng chờ thanh toán";
            }

            // duyet tung seller
            foreach (var seller in cartDTO.sellers)
            {
                var result = false;
                var groupedModals = seller.modals.GroupBy(x => x.voucherId);

                // gop tung modal co cung voucher id
                foreach (var modals in groupedModals)
                {
                    var existedVoucher = await _voucherRepository.GetByIdAsync(modals.Key,
                                                                                    isTracking: true,
                                                                                    includeProperties: x => x.Include(x => x.Modals)
                                                                                                                    .ThenInclude(x => x.Carts)
                                                                                                                .Include(x => x.Modals)
                                                                                                                    .ThenInclude(x => x.VoucherCodes));

                    if (existedVoucher != null)
                    {
                        // duyet tung modal 
                        foreach (var cartModal in modals)
                        {
                            var existedModal = existedVoucher.Modals.FirstOrDefault(x => x.Id == cartModal.id);

                            // kiem tra ton kho cua modal
                            if (cartModal.quantity > existedModal?.Stock)
                            {
                                throw new ConflictException($"Bạn đặt {cartModal.quantity} {cartModal.title} nhưng trong khi chỉ còn {existedModal.Stock}");
                            }

                            Guid? promotionId = null;

                            if (cartModal.shopPromotionId != null)
                            {
                                var existedPromotion = await _promotionRepository.GetByIdAsync(cartModal.shopPromotionId, isTracking: true);

                                if (existedPromotion == null)
                                {
                                    throw new NotFoundException($"Không tìm thấy promotion {cartModal.shopPromotionId}");
                                }

                                // Check if the promotion has expired
                                if (existedPromotion.EndDate.HasValue && today > existedPromotion.EndDate.Value)
                                {
                                    throw new InvalidOperationException($"Promotion {cartModal.shopPromotionId} đã hết hạn. Hạn cuối: {existedPromotion.EndDate.Value:yyyy-MM-dd}");
                                }

                                // Optional: Check if the promotion has started
                                if (existedPromotion.StartDate.HasValue && today < existedPromotion.StartDate.Value)
                                {
                                    throw new InvalidOperationException($"Promotion {cartModal.shopPromotionId} chưa bắt đầu. Ngày bắt đầu: {existedPromotion.StartDate.Value:yyyy-MM-dd}");
                                }

                                if (existedPromotion.Stock == 0)
                                {
                                    throw new ConflictException($"Promotion {cartModal.shopPromotionId} đã hết");
                                }

                                existedModal.Carts.Remove(existedModal.Carts.FirstOrDefault(c => c.ModalId == cartModal.id));

                                existedPromotion.Stock -= 1;

                                await _promotionRepository.SaveChanges();
                            }

                            order.OrderDetails.Add(new OrderDetail
                            {
                                ModalId = existedModal.Id,
                                Quantity = cartModal.quantity,
                                UnitPrice = existedModal.SellPrice,
                                //Status = OrderStatusEnum.PENDING.ToString(),
                                CreateDate = DateTime.Now,
                                CreateBy = thisUserObj.userId,
                                ShopDiscountPercent = (int)cartModal.shopDiscountPercent,
                                ShopDiscountMoney = (int)cartModal.shopDiscountMoney,
                                ShopPromotionId = cartModal.shopPromotionId,
                            });
                        }
                    }

                    result = await _voucherRepository.UpdateAsync(existedVoucher);
                }
            }

            order.DiscountPrice = order.OrderDetails.Sum(x => x.DiscountPrice);
            order.TotalPrice = order.OrderDetails.Sum(x => x.TotalPrice);
            order.UsedBalance = checkOutViewModel.use_balance;
            order.UsedVPoint = checkOutViewModel.use_VPoint;
            order.GiftEmail = checkOutViewModel.gift_email;

            // gửi thông báo ở đây

            var orderId = await _orderRepository.AddReturnString(order);

            var existedOrder = await _orderRepository.GetByIdAsync(orderId, includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                                        .ThenInclude(x => x.Modal.Voucher.Seller)
                                                                                                                        .Include(x => x.Buyer)
                                                                                                                        .ThenInclude(x => x.BuyerWallet)
                                                                                                                            .ThenInclude(x => x.BuyerWalletTransactions)
                                                                                                                        .Include(x => x.OrderDetails)
                                                                                                                        .ThenInclude(x => x.Modal)
                                                                                                                            .ThenInclude(x => x.Voucher),
                                                                                                                        isTracking: true);

            if (existedOrder.Status.Equals(OrderStatusEnum.PAID.ToString()))
            {
                WalletTransaction walletTransaction = new()
                {
                    Status = "PAID",
                    Type = WalletTransactionTypeEnum.ADMIN.ToString(),
                    Amount = existedOrder.FinalPrice * 10 / 100,
                    CreateDate = DateTime.Now,
                    Note = $"Ngân hàng của Nam được nhận {existedOrder.FinalPrice * 10 / 100} từ đơn hàng {existedOrder.Id}",
                };

                existedOrder.WalletTransactions.Add(walletTransaction);

                await _sendEmailService.SendEmailAsync("advouchee@gmail.com", "Tiền về ngân hàng của Nam", $"Ngân hàng của Nam được nhận {existedOrder.FinalPrice * 10 / 100} từ đơn hàng {existedOrder.Id}");

                foreach (var existedOrderDetail in existedOrder.OrderDetails.GroupBy(x => x.Modal.VoucherId))
                {
                    // gop tung modal co cung voucher id
                    var existedVoucher = await _voucherRepository.GetByIdAsync(existedOrderDetail.Key,
                                                                                        isTracking: true,
                                                                                        includeProperties: x => x.Include(x => x.Modals)
                                                                                                                    .ThenInclude(x => x.Carts)
                                                                                                                  .Include(x => x.Modals)
                                                                                                                    .ThenInclude(x => x.VoucherCodes));

                    // duyet tung modal 
                    foreach (var cartModal in existedOrderDetail)
                    {
                        var existedModal = existedVoucher.Modals.FirstOrDefault(x => x.Id == cartModal.ModalId);

                        //existedModal.Stock -= cartModal.Quantity;

                        var voucherCodes = _voucherCodeRepository.GetTable()
                                                                    .Where(x => x.OrderId == null && x.ModalId == existedModal.Id && x.EndDate >= today)
                                                                    .Where(x => x.NewCode != null)
                                                                    .OrderBy(x => x.EndDate)
                                                                    .Take(cartModal.Quantity)
                                                                    .AsTracking();

                        foreach (var voucherCode in voucherCodes)
                        {
                            voucherCode.OrderId = existedOrder.Id;
                            //voucherCode.Status = VoucherCodeStatusEnum.CONVERTING.ToString();
                        }

                        await _voucherCodeRepository.SaveChanges();

                        //existedVoucher.Stock -= cartModal.Quantity;

                        existedModal.Carts.Remove(existedModal.Carts.FirstOrDefault(c => c.ModalId == cartModal.ModalId));
                    }

                    await _voucherRepository.UpdateAsync(existedVoucher);
                }

                // Update existedOrder details
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
            }

            CreateNotificationDTO createNotificationDTO = new()
            {
                body = "Cảm ơn bạn đã dặt hơn hàng\n" +
                        $"ID tham khảo: {existedOrder.Id}",
                title = "Đơn hàng mới",
                receiverId = thisUserObj.userId,
            };

            await _sendEmailService.SendEmailAsync(existedOrder.Buyer.Email, "Đơn hang mới", existedOrder.Id);

            await _notificationService.CreateNotificationAsync(createNotificationDTO);

            return new ResponseMessage<string>
            {
                message = "Tạo order thành công",
                result = true,
                value = orderId
            };
        }

        public async Task<GetDetailOrderDTO> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                .ThenInclude(x => x.Modal)
                                                                                                    .ThenInclude(x => x.Voucher)
                                                                                                        .ThenInclude(x => x.Brand)
                                                                                            .Include(x => x.VoucherCodes));
            if (order != null)
            {
                var orderDTO = _mapper.Map<GetDetailOrderDTO>(order);
                return orderDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy order với id {id}");
            }
        }

        public async Task<DynamicResponseModel<GetOrderDTO>> GetOrdersAsync(PagingRequest pagingRequest, OrderFilter orderFilter, ThisUserObj? thisUserObj)
        {
            (int, IQueryable<GetOrderDTO>) result;
            if (thisUserObj == null)
            {
                result = _orderRepository.GetTable()
                        .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetOrderDTO>(orderFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            else
            {
                result = _orderRepository.GetTable().Where(x => x.CreateBy.Equals(thisUserObj.userId))
                        .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetOrderDTO>(orderFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            

            return new DynamicResponseModel<GetOrderDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetOrderDetailDTO>> GetSellerOrderAsync(
                                                                                        PagingRequest pagingRequest,
                                                                                        OrderDetailFilter orderDetailFilter,
                                                                                        ThisUserObj thisUserObj)
        {
            // Base query for OrderDetails
            var query = _orderRepository.GetTable()
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.Modal.Voucher.SellerId == thisUserObj.userId);

            // Apply OrderDetailFilter conditions
            if (!string.IsNullOrEmpty(orderDetailFilter.orderId))
            {
                query = query.Where(od => od.OrderId == orderDetailFilter.orderId);
            }

            if (orderDetailFilter.startDate.HasValue)
            {
                query = query.Where(od => od.Order.CreateDate >= orderDetailFilter.startDate.Value);
            }

            if (orderDetailFilter.endDate.HasValue)
            {
                query = query.Where(od => od.Order.CreateDate <= orderDetailFilter.endDate.Value);
            }

            if (orderDetailFilter.sortOrderEnum != null)
            {
                DateTime now = DateTime.UtcNow.Date; // Truncate time part
                query = orderDetailFilter.sortOrderEnum switch
                {
                    SortOrderEnum.TODAY => query.Where(od => od.Order.CreateDate.Value.Date == now),
                    SortOrderEnum.YESTERDAY => query.Where(od => od.Order.CreateDate.Value.Date == now.AddDays(-1)),
                    SortOrderEnum.ONE_WEEK_BEFORE => query.Where(od => od.Order.CreateDate <= now.AddDays(-7)),
                    SortOrderEnum.ONE_MONTH_BEFORE => query.Where(od => od.Order.CreateDate <= now.AddMonths(-1)),
                    _ => query
                };
            }

            // Apply paging to OrderDetails
            var pagedOrderDetails = await query
                .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                .Take(pagingRequest.pageSize)
                .ProjectTo<GetOrderDetailDTO>(_mapper.ConfigurationProvider) // Map to DTO
                .ToListAsync();

            // Calculate total count of OrderDetails
            var totalOrderDetailsCount = await query.CountAsync();

            // Return result
            return new DynamicResponseModel<GetOrderDetailDTO>
            {
                metaData = new()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = totalOrderDetailsCount
                },
                results = pagedOrderDetails
            };
        }

        public async Task<DynamicResponseModel<GetVoucherCodeDTO>> GetSellerOrderedVoucherCodeAsync(string orderId, Guid modalId, PagingRequest pagingRequest)
        {
            (int, IQueryable<GetVoucherCodeDTO>) result;

            result = _voucherCodeRepository.GetTable()
                        .Where(x => x.ModalId == modalId && x.OrderId == orderId)
                        .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider)
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetVoucherCodeDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
            };
        }
    }
}

