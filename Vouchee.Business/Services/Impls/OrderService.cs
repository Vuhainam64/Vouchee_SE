﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
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
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IVoucherRepository voucherRepository,
                                IOrderRepository orderRepository,
                                IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
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
                    orderDetail.UnitPrice = voucher.Price;
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

        public async Task<DynamicResponseModel<GetOrderDTO>> GetOrdersAsync(PagingRequest pagingRequest,
                                                                                    OrderFilter voucherFiler,
                                                                                    SortOrderEnum sortOrderEnum)
        {
            (int, IQueryable<GetOrderDTO>) result;
            try
            {
                result = _orderRepository.GetTable()
                            .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetOrderDTO>(voucherFiler))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return new DynamicResponseModel<GetOrderDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1
                },
                results = result.Item2.ToList()
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

