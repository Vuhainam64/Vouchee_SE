﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Wordprocessing;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class RefundRequestService : IRefundRequestService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly ISendEmailService _sendEmailService;

        private readonly IBaseRepository<Data.Models.Entities.Notification> _notificationRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<Media> _mediaRepository;
        private readonly IBaseRepository<RefundRequest> _refundRequestRepository;
        private readonly IMapper _mapper;

        public RefundRequestService(IFileUploadService fileUploadService,
                                    IBaseRepository<Data.Models.Entities.Notification> notiifcationRepository,
                                    ISendEmailService sendEmailService,
                                    IBaseRepository<Order> orderRepository,
                                    IBaseRepository<User> userRepository,
                                    IBaseRepository<VoucherCode> voucherCodeRepository,
                                    IBaseRepository<Media> mediaRepository,
                                    IBaseRepository<RefundRequest> refundRequestRepository,
                                    IMapper mapper)
        {
            _fileUploadService = fileUploadService;
            _notificationRepository = notiifcationRepository;
            _sendEmailService = sendEmailService;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _voucherCodeRepository = voucherCodeRepository;
            _mediaRepository = mediaRepository;
            _refundRequestRepository = refundRequestRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateRefundRequestAsync(CreateRefundRequestDTO createRefundRequestDTO, ThisUserObj thisUserObj)
        {
            var existedVoucherCode = await _voucherCodeRepository.GetByIdAsync(createRefundRequestDTO.voucherCodeId, 
                                                                                includeProperties: x => x.Include(x => x.Modal)
                                                                                                            .ThenInclude(x => x.Voucher)
                                                                                                                .ThenInclude(x => x.Supplier), 
                                                                                isTracking: true);

            if (existedVoucherCode == null)
            {
                throw new NotFoundException("Không tìm thấy id của voucher code này");
            }

            if (existedVoucherCode.OrderId == null)
            {
                throw new ConflictException("Voucher code này chưa được order");
            }

            var existedRefundRequest = await _refundRequestRepository.GetFirstOrDefaultAsync(x => x.VoucherCodeId == createRefundRequestDTO.voucherCodeId);

            if (existedRefundRequest != null)
            {
                throw new ConflictException("Voucher code này đã có refund request từ trước");
            }

            existedVoucherCode.Status = VoucherCodeStatusEnum.SUSPECTED.ToString();

            await _voucherCodeRepository.SaveChanges();

            var refundRequest = _mapper.Map<RefundRequest>(createRefundRequestDTO);
            refundRequest.CreateBy = thisUserObj.userId;

            int index = 0;

            foreach (var image in createRefundRequestDTO.images)
            {
                var imageUrl = await _fileUploadService.UploadImageToFirebase(image, thisUserObj.userId.ToString(), StoragePathEnum.OTHER);

                Media media = new()
                {
                    Url = imageUrl,
                    CreateBy = thisUserObj.userId,
                    CreateDate = DateTime.Now,
                    Index = index++,
                };

                refundRequest.Medias.Add(media);
            }

            var result = await _refundRequestRepository.AddAsync(refundRequest);

            var suppliers = await _userRepository.GetWhereAsync(x => x.SupplierId == existedVoucherCode.Modal.Voucher.Supplier.Id);

            foreach (var supplier in suppliers)
            {
                await _sendEmailService.SendEmailAsync(supplier.Email, "Có yêu cầu refund mới", $"Có yêu càu refund {result}");
            }

            return new ResponseMessage<Guid>()
            {
                message = "Tạo refund request thành công, bạn chờ 15' để supplier xác nhận nhé",
                result = true,
                value = (Guid) result
            };
        }

        public async Task<ResponseMessage<bool>> DeleteRefundRequestAsync(Guid id)
        {
            var existedRefundRequest = await _refundRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.WalletTransaction)
                                                                                                                    .Include(x => x.Medias), isTracking: true);
            if (existedRefundRequest == null)
            {
                throw new NotFoundException("Không tìm thấy refund request này");
            }

            if (existedRefundRequest.Status != RefundRequestStatusEnum.PENDING.ToString())
            {
                throw new ConflictException("Refund request này đã được xử lý");
            }

            foreach (var media in existedRefundRequest.Medias.ToList())
            {
                await _mediaRepository.DeleteAsync(media);
            }

            await _refundRequestRepository.DeleteAsync(existedRefundRequest);

            return new ResponseMessage<bool>()
            {
                message = "Đã xóa refund request thành công",
                result = true,
                value = true
            }; 
        }

        public async Task<DynamicResponseModel<GetRefundRequestDTO>> GetAllRefundRequestAsync(PagingRequest pagingRequest, RefundRequestFilter refundRequestFilter)
        {
            (int, IQueryable<GetRefundRequestDTO>) result;

            result = _refundRequestRepository.GetTable()
                        .ProjectTo<GetRefundRequestDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetRefundRequestDTO>(refundRequestFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetRefundRequestDTO>()
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

        public async Task<DynamicResponseModel<GetRefundRequestDTO>> GetSupplierRefundRequestAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest, RefundRequestFilter refundRequestFilter)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Supplier));

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("User này không thuộc supplier nào");
            }

            (int, IQueryable<GetRefundRequestDTO>) result;

            result = _refundRequestRepository.GetTable()
                                                .Where(x => x.VoucherCode.Modal.Voucher.SupplierId == existedUser.SupplierId)
                                                .ProjectTo<GetRefundRequestDTO>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(_mapper.Map<GetRefundRequestDTO>(refundRequestFilter))
                                                .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetRefundRequestDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync()
            };
        }

        public async Task<GetRefundRequestDTO> GetRefundRequestByIdAsync(Guid id)
        {
            var existedRefundRequest = await _refundRequestRepository.GetByIdAsync(id);

            if (existedRefundRequest == null)
            {
                throw new NotFoundException("Không tìm thấy refund request này");
            }

            return _mapper.Map<GetRefundRequestDTO>(existedRefundRequest);
        }

        public async Task<ResponseMessage<bool>> UpdateRefundRequestAsync(Guid id, UpdateRefundRequestDTO updateRefundRequestDTO, ThisUserObj thisUserObj)
        {
            var existedRefundRequest = await _refundRequestRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.WalletTransaction)
                                                                                                                    .Include(x => x.Medias), isTracking: true);

            if (existedRefundRequest == null)
            {
                throw new NotFoundException("Không tìm thấy refund request này");
            }

            if (existedRefundRequest.Status != RefundRequestStatusEnum.PENDING.ToString())
            {
                throw new ConflictException("Refund request này đã được xử lý");
            }

            var existedVoucherCode = await _voucherCodeRepository.GetByIdAsync(updateRefundRequestDTO.voucherCodeId);

            if (existedVoucherCode == null)
            {
                throw new NotFoundException("Không tìm thấy voucher code này");
            }

            if (existedVoucherCode.OrderId == null)
            {
                throw new ConflictException("Voucher code này chưa được order");
            }

            existedRefundRequest = _mapper.Map(updateRefundRequestDTO, existedRefundRequest);
            existedRefundRequest.UpdateBy = thisUserObj.userId;

            foreach (var media in existedRefundRequest.Medias.ToList())
            {
                await _mediaRepository.DeleteAsync(media);
            }

            //int index = 0;
            //foreach (var image in updateRefundRequestDTO.images)
            //{
            //    Media media = new()
            //    {
            //        Url = image,
            //        CreateBy = thisUserObj.userId,
            //        CreateDate = DateTime.Now,
            //        Index = index++,
            //    };

            //    existedRefundRequest.Medias.Add(media);
            //}

            await _refundRequestRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật refund request thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateRefundRequestStatusAsync(Guid id, RefundRequestStatusEnum refundRequestStatusEnum, string reason, ThisUserObj thisUserObj)
        {
            var existedRefundRequest = await _refundRequestRepository.FindAsync(id, isTracking: true);

            if (existedRefundRequest == null)
            {
                throw new NotFoundException("Không tìm thấy refund request này");
            }

            if (existedRefundRequest.Status != RefundRequestStatusEnum.PENDING.ToString())
            {
                throw new ConflictException("Refund request này đã được xử lý");
            }

            var existedOrder = await _orderRepository.GetByIdAsync(existedRefundRequest.VoucherCode.OrderId,
                                                                    includeProperties: x => x.Include(x => x.Buyer)
                                                                                                .ThenInclude(x => x.BuyerWallet)
                                                                                                , isTracking: true);

            if (existedOrder == null)
            {
                throw new NotFoundException("Không tìm thấy order này");
            }

            if (refundRequestStatusEnum == RefundRequestStatusEnum.ACCEPTED)
            {
                existedRefundRequest.VoucherCode.Modal.Voucher.Supplier.SupplierWallet.Balance -= existedRefundRequest.VoucherCode.Modal.SellPrice;
                existedRefundRequest.Reason = $"Refund request này đã được chấp thuận và đã hoàn {existedRefundRequest.VoucherCode.Modal.SellPrice} về ví mua";
                existedRefundRequest.Status = RefundRequestStatusEnum.ACCEPTED.ToString();
                existedRefundRequest.UpdateBy = thisUserObj.userId;
                existedRefundRequest.UpdateDate = DateTime.Now;

                existedRefundRequest.WalletTransaction = new()
                {
                    Status = "PAID",
                    Type = WalletTransactionTypeEnum.REFUND.ToString(),
                    CreateDate = DateTime.Now,
                    CreateBy = thisUserObj.userId,
                    Note = "Hoàn tiền về ví mua",
                    BeforeBalance = existedOrder.Buyer.BuyerWallet.Balance,
                    Amount = existedRefundRequest.VoucherCode.Modal.SellPrice,
                    AfterBalance = existedOrder.Buyer.BuyerWallet.Balance + existedRefundRequest.VoucherCode.Modal.SellPrice,
                };

                existedOrder.Buyer.BuyerWallet.Balance += existedRefundRequest.VoucherCode.Modal.SellPrice;
                existedOrder.Buyer.BuyerWallet.UpdateDate = DateTime.Now;
                existedOrder.Buyer.BuyerWallet.UpdateBy = thisUserObj.userId;

                Data.Models.Entities.Notification notification = new()
                {
                    Body = $"Voucher code {existedRefundRequest.VoucherCode.Modal.Title} đã được hoàn tiền về ví mua",
                    CreateDate = DateTime.Now,
                    ReceiverId = existedOrder.Buyer.Id,
                    Seen = false,
                    Title = "Hoàn tiền thành công",
                };

                await _notificationRepository.AddAsync(notification);

                await _sendEmailService.SendEmailAsync(existedOrder.Buyer.Email, "Cập nhật trạng thái refund", "Refund của bạn đã được chấp thuận");

                await _orderRepository.SaveChanges();

                await _refundRequestRepository.SaveChanges();

                return new ResponseMessage<bool>()
                {
                    message = "Đã cập nhật trạng thái thành công",
                    result = true,
                    value = true
                };
            }
            else if (refundRequestStatusEnum == RefundRequestStatusEnum.DECLINED)
            {
                existedRefundRequest.Status = RefundRequestStatusEnum.DECLINED.ToString();
                existedRefundRequest.Reason = reason;
                existedRefundRequest.UpdateDate = DateTime.Now;
                existedRefundRequest.UpdateBy = thisUserObj.userId;

                await _sendEmailService.SendEmailAsync(existedOrder.Buyer.Email, "Cập nhật trạng thái refund", "Refund của bạn đã bị từ chối");

                await _refundRequestRepository.SaveChanges();
                return new ResponseMessage<bool>()
                {
                    message = "Đã cập nhật trạng thái thành công",
                    result = true,
                    value = true
                };
            }

            return null;
        }

        public async Task<DynamicResponseModel<GetRefundRequestDTO>> GetBuyerRefundRequestAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest, RefundRequestFilter refundRequestFilter)
        {
            (int, IQueryable<GetRefundRequestDTO>) result;

            result = _refundRequestRepository.GetTable()
                        .Where(x => x.VoucherCode.Order.CreateBy == thisUserObj.userId)
                        .ProjectTo<GetRefundRequestDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetRefundRequestDTO>(refundRequestFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetRefundRequestDTO>()
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
