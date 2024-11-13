using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
        private readonly ICartService _cartService;

        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<OrderDetail> _orderDetailRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IBaseRepository<Modal> modalRepository,
                                IBaseRepository<User> userRepository,
                                ICartService cartService,
                                IBaseRepository<Voucher> voucherRepository,
                                IBaseRepository<Order> orderRepository,
                                IBaseRepository<OrderDetail> orderDetailRepository,
                                IBaseRepository<VoucherCode> voucherCodeRepository,
                                IMapper mapper)
        {
            _modalRepository = modalRepository;
            _userRepository = userRepository;
            _cartService = cartService;
            _voucherCodeRepository = voucherCodeRepository;
            _orderDetailRepository = orderDetailRepository;
            _voucherRepository = voucherRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<bool> AssignCodeToOrderAsync(Guid orderDetailId, VoucherCodeList voucherCodeList)
        {
            try
            {
                var existedOrderDetail = await _orderDetailRepository.FindAsync(orderDetailId, false);
                if (existedOrderDetail == null)
                {
                    throw new NotFoundException("Không tìm thấy order detail");
                }

                if (voucherCodeList.voucherCodeIds.Count() != existedOrderDetail.Quantity)
                {
                    throw new ConflictException($"Khách hàng đã đặt {existedOrderDetail.Quantity} voucher, bạn sẽ bị trừ điểm uy tín nếu không cung cấp đủ voucher");
                }

                foreach (var voucherCodeId in voucherCodeList.voucherCodeIds)
                {
                    var existedVoucherCode = await _voucherCodeRepository.FindAsync(voucherCodeId, false);
                    if (existedVoucherCode == null)
                    {
                        throw new NotFoundException($"Không tìm thấy voucher code với id {voucherCodeId}");
                    }
                    // check code này có phải là voucher đang đặt không
                    if (existedOrderDetail.ModalId != existedVoucherCode.ModalId)
                    {
                        throw new ConflictException("Voucher code này không thuộc voucher đang đặt");
                    }

                    existedOrderDetail.VoucherCodes.Add(existedVoucherCode);

                    existedVoucherCode.Status = "ASSIGNED";
                    await _voucherCodeRepository.UpdateAsync(existedVoucherCode);
                }

                existedOrderDetail.Status = "DONE";
                await _orderDetailRepository.UpdateAsync(existedOrderDetail);


                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseMessage<Guid>> CreateOrderAsync(ThisUserObj thisUserObj, CheckOutViewModel checkOutViewModel)
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

            if (user.BuyerWallet.Balance < cartDTO.sellers.Sum(x => x.modals.Sum(x => x.finalPrice)))
            {
                throw new Exception("Số dư trong ví không đủ");
            }

            Order order = new()
            {
                PaymentType = PayTypeEnum.BANK.ToString(),
                Status = OrderStatusEnum.PENDING.ToString(),
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                OrderDetails = new List<OrderDetail>()
            };

            // duyet tung seller
            var amountSellers = new Dictionary<Guid, int>();
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
                                                                                                                .ThenInclude(x => x.Carts));

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

                            existedModal.Stock -= cartModal.quantity;
                            existedVoucher.Stock -= cartModal.quantity;

                            existedModal.Carts.Remove(existedModal.Carts.FirstOrDefault(c => c.ModalId == cartModal.id));

                            order.OrderDetails.Add(new OrderDetail
                            {
                                ModalId = existedModal.Id,
                                Quantity = cartModal.quantity,
                                UnitPrice = existedModal.SellPrice,
                                Status = OrderStatusEnum.PENDING.ToString(),
                                CreateDate = DateTime.Now,
                                CreateBy = thisUserObj.userId,
                                PromotionId = cartModal.promotionId
                            });
                        }
                    }

                    result = await _voucherRepository.UpdateAsync(existedVoucher);
                }

                // chuyen tiền từ ví người mua sang ví nhà bán hàng;
                var amount = seller.modals.Sum(x => x.finalPrice);
                amountSellers[seller.sellerId.Value] = amount.Value;
            }

            order.TotalPrice = order.OrderDetails.Sum(x => x.TotalPrice);
            order.PointUp = order.FinalPrice / 1000;

            if (checkOutViewModel.use_VPoint != 0)
            {
                if (user.VPoint != 0)
                {
                    order.PointDown = user.VPoint;
                    user.VPoint = order.PointUp;
                }
                else
                {
                    user.VPoint += order.FinalPrice / 1000;
                }
            }
            else
            {
                user.VPoint += order.FinalPrice / 1000;
            }

            var orderId = await _orderRepository.AddAsync(order);

            if (orderId == Guid.Empty)
            {
                throw new Exception("Failed to create order.");
            }

            // Phải chắc chắn order được tạo
            WalletTransaction transaction = new()
            {
                CreateBy = user.Id,
                CreateDate = DateTime.Now,
                Status = WalletTransactionStatusEnum.DONE.ToString(),
            };

            foreach (var seller in amountSellers)
            {
                var existedSeller = await _userRepository.GetByIdAsync(seller.Key, includeProperties: x => x.Include(x => x.SellerWallet), isTracking: true);

                if (existedSeller == null)
                {
                    throw new NotFoundException($"Không tìm thấy seller {seller.Key}");
                }

                if (existedSeller.SellerWallet == null)
                {
                    throw new NotFoundException($"Không tìm thấy wallet của seller {seller.Key}");
                }

                transaction.Amount = seller.Value;
                transaction.SellerWalletId = existedSeller.Id;
                transaction.OrderId = orderId;

                existedSeller.SellerWallet.SellerWalletTransactions.Add(transaction);
                existedSeller.SellerWallet.Balance += seller.Value;

                transaction.BuyerWalletId = user.BuyerWallet.Id;
                user.BuyerWallet.BuyerWalletTransactions.Add(transaction);
                user.BuyerWallet.Balance -= seller.Value;
            }

            await _userRepository.SaveChanges();

            return new ResponseMessage<Guid>
            {
                message = "Tạo order thành công",
                result = true,
                value = (Guid) orderId
            };
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            try
            {
                var result = false;
                var order = await _orderRepository.GetByIdAsync(id);
                if (order != null)
                {
                    result = await _orderRepository.DeleteAsync(order);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy order với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException(ex.Message);
            }
        }

        public async Task<GetOrderDTO> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.OrderDetails).ThenInclude(x => x.VoucherCodes));
            if (order != null)
            {
                var orderDTO = _mapper.Map<GetOrderDTO>(order);
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

            result = _orderRepository.GetTable().Where(x => x.CreateBy.Equals(thisUserObj.userId))
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

        public async Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDTO updateOrderDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var existedOrder = await _orderRepository.GetByIdAsync(id);
                if (existedOrder != null)
                {
                    var entity = _mapper.Map<Order>(updateOrderDTO);
                    return await _orderRepository.UpdateAsync(entity);
                }
                else
                {
                    throw new NotFoundException("Không tìm thấy order");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException(ex.Message);
            }
        }

        public async Task<ResponseMessage<bool>> UpdateOrderTransactionAsync(Guid id, Guid partnerTransactionId, ThisUserObj thisUserObj)
        {
            try
            {
                var existedOrder = await _orderRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.OrderDetails)
                                                                                                        .ThenInclude(x => x.Modal)
                                                                                                            .ThenInclude(x => x.Voucher)
                                                                                                                .ThenInclude(x => x.Seller)
                                                                                                                    .ThenInclude(x => x.SellerWallet)
                                                                        , isTracking: true);

                if (existedOrder == null)
                {
                    throw new NotFoundException("Không tìm thấy order này");
                }

                var groupedModals = existedOrder.OrderDetails.GroupBy(x => x.Modal.Voucher.SellerId);
                foreach (var groupedModal in groupedModals)
                {
                    var existedSeller = await _userRepository.GetByIdAsync(groupedModal.Key, includeProperties: x => x.Include(x => x.SellerWallet)
                                                                                                                        .ThenInclude(x => x.SellerWalletTransactions)                                    
                                                                            , isTracking: true);

                    if (existedSeller == null)
                    {
                        throw new NotFoundException($"Không tìm thấy seller {groupedModal.Key}");
                    }

                    if (existedSeller.SellerWallet == null)
                    {
                        throw new NotFiniteNumberException($"Không tìm thấy ví seller {groupedModal.Key}");
                    }

                    WalletTransaction walletTransaction = new()
                    {
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Status = WalletTransactionStatusEnum.DONE.ToString(),
                        Amount = groupedModal.Sum(x => x.FinalPrice),
                        OrderId = existedOrder.Id,
                        PartnerTransactionId = partnerTransactionId,
                        SellerWalletId = existedSeller.Id,
                    };

                    existedSeller.SellerWallet.SellerWalletTransactions.Add(walletTransaction);
                    existedSeller.SellerWallet.Balance += groupedModal.Sum(x => x.FinalPrice);
                }

                await _userRepository.SaveChanges();

                existedOrder.Status = partnerTransactionId == Guid.Empty ? OrderStatusEnum.ERROR_AT_TRANSACTION.ToString() : OrderStatusEnum.FINISH_TRANSACTION.ToString();
                existedOrder.PaymentType = PayTypeEnum.BANK.ToString();

                var result = await _orderRepository.UpdateAsync(existedOrder);

                return new ResponseMessage<bool>()
                {
                    message = "Cập nhật trạng thái thành công",
                    result = result,
                    value = result
                };
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException(ex.Message);
            }
        }
    }
}

