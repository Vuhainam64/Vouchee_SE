using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Dictionary;
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

        public async Task<ResponseMessage<Guid>> CreateOrderAsync(ThisUserObj thisUserObj, bool usingPoint = false)
        {
            var user = await _userRepository.GetByIdAsync(thisUserObj.userId, isTracking: true);

            CartDTO cartDTO = await _cartService.GetCartsAsync(thisUserObj, false);

            Order order = new()
            {
                PaymentType = "BANKING",
                Status = OrderStatusEnum.PENDING.ToString(),
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                OrderDetails = new List<OrderDetail>()
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

                            existedModal.Carts.Remove(existedModal.Carts.FirstOrDefault(c => c.BuyerId == thisUserObj.userId));

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
            }

            order.TotalPrice = order.OrderDetails.Sum(x => x.TotalPrice);
            order.PointUp = order.FinalPrice;

            if (usingPoint)
            {
                order.PointDown = user.VPoint;
                user.VPoint = 0;
            }
            else
            {
                user.VPoint += order.FinalPrice;
            }

            await _userRepository.UpdateAsync(user);

            var orderId = await _orderRepository.AddAsync(order);
            if (orderId == Guid.Empty)
            {
                throw new Exception("Failed to create order.");
            }

            return new ResponseMessage<Guid>
            {
                message = "Tạo order thành công",
                result = true,
                value = (Guid)orderId
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
    }
}

