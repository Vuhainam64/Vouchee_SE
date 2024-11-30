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
        private readonly INotificationService _notificationService;
        private readonly ICartService _cartService;

        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<OrderDetail> _orderDetailRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(INotificationService notificationService,
                            ICartService cartService,
                            IBaseRepository<Modal> modalRepository,
                            IBaseRepository<User> userRepository,
                            IBaseRepository<VoucherCode> voucherCodeRepository,
                            IBaseRepository<OrderDetail> orderDetailRepository,
                            IBaseRepository<Voucher> voucherRepository,
                            IBaseRepository<Order> orderRepository,
                            IMapper mapper)
        {
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

                            existedModal.Carts.Remove(existedModal.Carts.FirstOrDefault(c => c.ModalId == cartModal.id));

                            order.OrderDetails.Add(new OrderDetail
                            {
                                ModalId = existedModal.Id,
                                Quantity = cartModal.quantity,
                                UnitPrice = existedModal.SellPrice,
                                Status = OrderStatusEnum.PENDING.ToString(),
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

            await _notificationService.CreateNotificationAsync(Guid.Parse("DEEE9638-DA34-4230-BE77-34137AA5FCFF"), createNotificationDTO);

            return new ResponseMessage<string>
            {
                message = "Tạo order thành công",
                result = true,
                value = orderId
            };
        }

        public async Task<GetDetailSellerOrderDTO> GetDetailSellerOrderAsync(string orderId, ThisUserObj thisUserObj)
        {
            var existedOrder = await _orderRepository.GetByIdAsync(orderId, includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                        .ThenInclude(x => x.Modal)
                                                                                                            .ThenInclude(x => x.Voucher));

            if (existedOrder == null)
            {
                throw new NotFoundException("Không tìm thấy order này");
            }

            existedOrder.OrderDetails = existedOrder.OrderDetails
                                            .Where(od => od.Modal.Voucher.SellerId == thisUserObj.userId)
                                            .ToList();

            GetDetailSellerOrderDTO getDetailSellerOrderDTO = new()
            {
                orderDetails = _mapper.Map<IList<GetOrderDetailDTO>>(existedOrder.OrderDetails),
                createDate = existedOrder.CreateDate
            };

            return getDetailSellerOrderDTO;
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

        public async Task<DynamicResponseModel<GetOrderDTO>> GetSellerOrderAsync(PagingRequest pagingRequest, OrderFilter orderFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetOrderDTO>) result;

            result = _orderRepository.GetTable()
                                        .Where(o => o.OrderDetails.Any(od => od.Modal.Voucher.SellerId == thisUserObj.userId))
                                        .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(_mapper.Map<GetOrderDTO>(orderFilter))
                                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

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
    }
}

