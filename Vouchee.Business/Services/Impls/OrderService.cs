using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class OrderService : IOrderService
    {
        private readonly ICartService _cartService;

        private readonly IUserRepository _userRepository;
        private readonly IVoucherCodeRepository _voucherCodeRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IUserRepository userRepository,
                                ICartService cartService,
                                IVoucherRepository voucherRepository,
                                IOrderRepository orderRepository,
                                IOrderDetailRepository orderDetailRepository,
                                IVoucherCodeRepository voucherCodeRepository,
                                IMapper mapper)
        {
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
                var existedOrderDetail = await _orderDetailRepository.FindAsync(orderDetailId);
                if (existedOrderDetail == null)
                {
                    throw new NotFoundException("Không tìm thấy order detail");
                }

                foreach (var voucherCodeId in voucherCodeList.voucherCodeIds)
                {
                    var existedVoucherCode = await _voucherCodeRepository.FindAsync(voucherCodeId);
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
                }

                if (!await _orderDetailRepository.UpdateAsync(existedOrderDetail))
                {
                    throw new UpdateObjectException("Lỗi không xác định khi cập nhật dữ liệu");
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Guid?> CreateOrderAsync(ThisUserObj thisUserObj)
        {
            return null;

            //CartDTO cartDTO = await _cartService.GetCartsAsync(thisUserObj);
    
            //Order order = new()
            //{
            //    PaymentType = "CASH",
            //    Status = "PENDING", // Fixed typo from "PENING" to "PENDING"
            //    CreateBy = thisUserObj.userId,
            //    CreateDate = DateTime.Now,
            //    OrderDetails = new List<OrderDetail>() // Assuming OrderDetails is a collection
            //};

            //// Loop through the vouchers in the cart and create corresponding OrderDetails
            //foreach (var voucher in cartDTO.sellers)
            //{
            //    order.OrderDetails.Add(new OrderDetail
            //    {
            //        VoucherId = voucher.id, // Assuming the voucher has an Id
            //        Quantity = voucher.quantity, // Set quantity (if applicable)
            //        UnitPrice = (decimal) voucher.sellPrice, // Assuming each voucher has a Price property
            //    });
            //}

            //order.TotalPrice = order.OrderDetails.Sum(x => x.TotalPrice);
            //order.DiscountValue = 0;

            //Guid? orderId = await _orderRepository.AddAsync(order);

            //foreach (var voucher in cartDTO.sellers)
            //{
            //    await _cartService.RemoveItemAsync((Guid)voucher.id, thisUserObj);

            //    Voucher voucherInstance = GetCurrentUser(thisUserObj.userId).Result.Carts.FirstOrDefault(x => x.VoucherId == voucher.id).Voucher;
            //    voucherInstance.Quantity -= voucher.quantity;

            //    await _voucherRepository.UpdateAsync(voucherInstance);
            //}

            //return orderId;
        }

        public async Task<User> GetCurrentUser(Guid userId)
        {
            User? userInstance = null;
            IQueryable<User> users = _userRepository.CheckLocal();

            if (users != null && users.Count() != 0)
            {
                userInstance = users.FirstOrDefault(x => x.Id == userId);
            }

            if (userInstance == null)
            {
                userInstance = await _userRepository.GetByIdAsync(userId, includeProperties: x => x.Include(x => x.Carts)
                                                                                                    .ThenInclude(x => x.Modal));
            }

            return userInstance;
        }

        //public async Task<Guid?> CreateOrderAsync(CreateOrderDTO createOrderDTO, ThisUserObj thisUserObj)
        //{
        //    try
        //    {
        //        var order = _mapper.Map<Order>(createOrderDTO);

        //        foreach (var orderDetail in order.OrderDetails)
        //        {
        //            var voucher = _voucherRepository.GetByIdAsync(orderDetail.VoucherId).Result;
        //            if (voucher == null)
        //            {
        //                throw new NotFoundException($"Không tìm thấy voucher với ID {orderDetail.VoucherId}");
        //            }
        //            if (orderDetail.Quantity > voucher.Quantity)
        //            {
        //                throw new QuantityExcessException($"Voucher {voucher.Id} vượt quá số lượng tồn kho");
        //            }

        //            orderDetail.VoucherId = voucher.Id;
        //            orderDetail.CreateBy = Guid.Parse(thisUserObj.userId);
        //            orderDetail.UnitPrice = voucher.OriginalPrice;
        //        }

        //        order.CreateBy = Guid.Parse(thisUserObj.userId);
        //        order.TotalPrice = order.OrderDetails.Sum(x => x.FinalPrice);

        //        var orderId = await _orderRepository.AddAsync(order);

        //        return orderId;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerService.Logger(ex.Message);
        //        throw new CreateObjectException("Lỗi không xác định khi tạo order");
        //    }
        //}

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
                throw new DeleteObjectException("Lỗi không xác định khi xóa order");
            }
        }

        public async Task<GetOrderDTO> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải order");
            }
        }

        public async Task<DynamicResponseModel<GetOrderDTO>> GetOrdersAsync(PagingRequest pagingRequest, OrderFilter orderFilter, ThisUserObj? thisUserObj)
        {
            (int, IQueryable<GetOrderDTO>) result;

            if (thisUserObj != null && thisUserObj.roleId.Equals(thisUserObj.buyerRoleId))
            {
                result = _orderRepository.GetTable().Where(x => x.CreateBy.Equals(thisUserObj.userId))
                            .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetOrderDTO>(orderFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            else
            {
                result = _orderRepository.GetTable()
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
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật order");
            }
        }
    }
}

