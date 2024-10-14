using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class OrderService : IOrderService
    {
        private readonly IVoucherCodeRepository _voucherCodeRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IVoucherRepository voucherRepository,
                                IOrderRepository orderRepository,
                                IOrderDetailRepository orderDetailRepository,
                                IVoucherCodeRepository voucherCodeRepository,
                                IMapper mapper)
        {
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
                    if (existedOrderDetail.VoucherId != existedVoucherCode.VoucherId)
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

        public async Task<Guid?> CreateOrderAsync(CreateOrderDTO createOrderDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var order = _mapper.Map<Order>(createOrderDTO);

                foreach (var orderDetail in order.OrderDetails)
                {
                    var voucher = _voucherRepository.GetByIdAsync(orderDetail.VoucherId).Result;
                    if (voucher == null)
                    {
                        throw new NotFoundException($"Không tìm thấy voucher với ID {orderDetail.VoucherId}");
                    }
                    if (orderDetail.Quantity > voucher.Quantity)
                    {
                        throw new QuantityExcessException($"Voucher {voucher.Id} vượt quá số lượng tồn kho");
                    }

                    orderDetail.VoucherId = voucher.Id;
                    orderDetail.CreateBy = Guid.Parse(thisUserObj.userId);
                    orderDetail.UnitPrice = voucher.OriginalPrice;
                }

                order.CreateBy = Guid.Parse(thisUserObj.userId);
                order.TotalPrice = order.OrderDetails.Sum(x => x.FinalPrice);

                var orderId = await _orderRepository.AddAsync(order);

                return orderId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo order");
            }
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

        public async Task<IList<GetOrderDTO>> GetOrdersAsync()
        {
            IQueryable<GetOrderDTO> result;
            try
            {
                result = _orderRepository.GetTable()
                            .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return result.ToList();
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

