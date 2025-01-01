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

        private readonly IBaseRepository<Promotion> _promotionRepository;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<OrderDetail> _orderDetailRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IBaseRepository<Promotion> promotionRepository,
                            ISendEmailService sendEmailService,
                            INotificationService notificationService,
                            ICartService cartService,
                            IBaseRepository<Modal> modalRepository,
                            IBaseRepository<User> userRepository,
                            IBaseRepository<VoucherCode> voucherCodeRepository,
                            IBaseRepository<OrderDetail> orderDetailRepository,
                            IBaseRepository<Voucher> voucherRepository,
                            IBaseRepository<Order> orderRepository,
                            IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _sendEmailService = sendEmailService;
            _notificationService = notificationService;
            _cartService = cartService;
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
            var user = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.BuyerWallet), isTracking: true);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (user.BuyerWallet == null)
            {
                throw new NotFoundException("Người dùng này chưa có ví buyer");
            }

            CartDTO cartDTO = await _cartService.GetCheckoutCartsAsync(thisUserObj, checkOutViewModel);

            Order order = new()
            {
                Status = OrderStatusEnum.PENDING.ToString(),
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                OrderDetails = new List<OrderDetail>(),
                Note = "Đơn hàng chờ thanh toán"
            };

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

                                // Get today's date as DateOnly
                                var today = DateOnly.FromDateTime(DateTime.UtcNow);

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

            CreateNotificationDTO createNotificationDTO = new()
            {
                body = "Cảm ơn bạn đã dặt hơn hàng\n" +
                        $"ID tham khảo: {orderId}",
                title = "Đơn hàng mới",
                receiverId = thisUserObj.userId,
            };

            await _sendEmailService.SendEmailAsync(order.Buyer.Email, "Đơn hang mới", orderId);

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

