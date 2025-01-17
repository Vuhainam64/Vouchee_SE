using AutoMapper;
using AutoMapper.QueryableExtensions;
using Firebase.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class ModalService : IModalService
    {
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IMapper _mapper;

        public ModalService(IBaseRepository<VoucherCode> voucherCodeRepository,
                            IBaseRepository<Order> orderRepository,
                            IBaseRepository<Voucher> voucherRepository,
                            IFileUploadService fileUploadService,
                            IBaseRepository<Modal> modalRepository,
                            IMapper mapper)
        {
            _voucherCodeRepository = voucherCodeRepository;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _fileUploadService = fileUploadService;
            _modalRepository = modalRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateModalAsync(Guid voucherId, CreateModalDTO createModalDTO, ThisUserObj thisUserObj)
        {
            Modal modal = _mapper.Map<Modal>(createModalDTO);

            var existVoucher = await _voucherRepository.FindAsync(voucherId, false);

            if (existVoucher == null)
            {
                throw new NotFoundException("Không tìm thấy voucher với id");
            }

            modal.VoucherId = voucherId;
            modal.CreateBy = thisUserObj.userId;

            return await _modalRepository.AddAsync(modal);
        }

        public async Task<ResponseMessage<bool>> DeleteModalAsync(Guid id)
        {
            //Modal modal = await _modalRepository.FindAsync(id, false);
            //if (modal != null)
            //{
            //    return await _modalRepository.DeleteAsync(modal);
            //}
            //return false;

            throw new NotImplementedException();
        }

        public async Task<dynamic> GetModalByIdAsync(Guid id, PagingRequest pagingRequest)
        {
            Modal existedModal = await _modalRepository.GetByIdAsync(id, includeProperties: x => x
                                                                        .Include(x => x.VoucherCodes)
        .Include(x => x.Ratings));

            if (existedModal != null)
            {
                var modal = _mapper.Map<GetDetailModalDTO>(existedModal);
                var total = modal.voucherCodes.Count();

                var pagedVoucherCode = modal.voucherCodes
                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                    .Take(pagingRequest.pageSize).ToList();

                modal.voucherCodes = pagedVoucherCode;

                return new
                {
                    metaData = new
                    {
                        page = pagingRequest.page,
                        size = pagingRequest.pageSize,
                        total = total
                    },
                    results = modal
                };
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }
        }

        public async Task<DynamicResponseModel<GetModalDTO>> GetModalsAsync(PagingRequest pagingRequest, ModalFilter modalFilter)
        {
            List<GetModalDTO> list;
            (int, IQueryable<GetModalDTO>) result;
            try
            {
                result = _modalRepository.GetTable()
                            .ProjectTo<GetModalDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetModalDTO>(modalFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

                list = result.Item2.ToList();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetModalDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = list// Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetOrderedModalDTO>> GetOrderedModals(Guid buyerId, PagingRequest pagingRequest, VoucherCodeFilter voucherCodeFilter)
        {
            var query = _voucherCodeRepository.GetTable()
                .Where(vc => vc.Order.CreateBy == buyerId); // Filter by buyerId

            if (voucherCodeFilter != null)
            {
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (voucherCodeFilter.status.HasValue)
                {
                    switch (voucherCodeFilter.status.Value)
                    {
                        case VoucherCodeStatusEnum.UNUSED:
                            // Filter for UNUSED vouchers within the specified date range
                            if (voucherCodeFilter.startDate.HasValue && voucherCodeFilter.endDate.HasValue)
                            {
                                query = query.Where(vc =>
                                    vc.StartDate <= voucherCodeFilter.startDate.Value &&
                                    voucherCodeFilter.endDate.Value >= vc.EndDate);
                            }
                            else if (voucherCodeFilter.startDate.HasValue)
                            {
                                query = query.Where(vc => vc.StartDate <= voucherCodeFilter.startDate.Value);
                            }
                            else if (voucherCodeFilter.endDate.HasValue)
                            {
                                query = query.Where(vc => voucherCodeFilter.endDate.Value >= vc.EndDate);
                            }
                            break;

                        case VoucherCodeStatusEnum.EXPIRED:
                            // Filter for EXPIRED vouchers
                            query = query.Where(vc => vc.EndDate < currentDate);
                            break;

                        default:
                            // Filter by exact status
                            query = query.Where(vc => vc.Status == voucherCodeFilter.status.Value.ToString());
                            break;
                    }
                }
                else
                {
                    // Handle cases where only dates are provided
                    if (voucherCodeFilter.startDate.HasValue && voucherCodeFilter.endDate.HasValue)
                    {
                        query = query.Where(vc =>
                            vc.StartDate >= voucherCodeFilter.startDate.Value &&
                            voucherCodeFilter.endDate.Value >= vc.EndDate);
                    }
                    else if (voucherCodeFilter.startDate.HasValue)
                    {
                        query = query.Where(vc => vc.StartDate <= voucherCodeFilter.startDate.Value);
                    }
                    else if (voucherCodeFilter.endDate.HasValue)
                    {
                        query = query.Where(vc => voucherCodeFilter.endDate.Value <= vc.EndDate);
                    }
                    else if (voucherCodeFilter.Title != null)
                    {
                        query = query.Where(vc => vc.Modal.Title.Equals(voucherCodeFilter.Title.ToString()));
                    }
                }
            }

            var groupedData = await query
                .GroupBy(vc => vc.Modal) // Group by Modal entity
                .Select(group => new
                {
                    Modal = group.Key, // The Modal entity
                    VoucherCodeCount = group.Count(), // Count of VoucherCodes for each Modal
                    VoucherCodes = group.Select(vc => vc).ToList()
                })
                .ToListAsync();

            // Project grouped data directly to GetOrderedModalDTO using AutoMapper
            var list = groupedData.Select(data =>
            {
                var dto = _mapper.Map<GetOrderedModalDTO>(data.Modal);
                if(!dto.ratings.IsNullOrEmpty())
                {
                    dto.isRating = "Đã Rating";
                }
                else
                {
                    dto.isRating = "Chưa Rating";
                }
                
                dto.voucherCodeCount = data.VoucherCodeCount;
                dto.voucherCodes = data.VoucherCodes
                                        .Select(vc => _mapper.Map<GetVoucherCodeModalDTO>(vc))
                                        .ToList();
                return dto;
            }).ToList();

            // Apply paging after the mapping
            var pagedList = list.Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                                .Take(pagingRequest.pageSize)
                                .ToList();

            return new DynamicResponseModel<GetOrderedModalDTO>
            {
                metaData = new MetaData
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = list.Count // Total number of records before paging
                },
                results = pagedList // Paged result
            };
        }


        //public async Task<DynamicResponseModel<GetPendingModalDTO>> GetPendingModals(Guid sellerId, PagingRequest pagingRequest, ModalFilter modalFilter)
        //{
        //    (int, IQueryable<GetPendingModalDTO>) result;

        //    result = _modalRepository.GetTable()
        //        .Include(x => x.OrderDetails)
        //        .ThenInclude(od => od.Modal)
        //            .ThenInclude(m => m.Voucher)
        //        .Where(x => x.Voucher.SellerId == sellerId)
        //        .Where(x => x.OrderDetails.Count != 0 && x.OrderDetails.All(x => x.Status == "PENDING"))
        //        .ProjectTo<GetPendingModalDTO>(_mapper.ConfigurationProvider)
        //        .DynamicFilter(_mapper.Map<GetPendingModalDTO>(modalFilter))
        //        .PagingIQueryable(pagingRequest.page,
        //                          pagingRequest.pageSize,
        //                          PageConstant.LIMIT_PAGING,
        //                          PageConstant.DEFAULT_PAPING);


        //    return new DynamicResponseModel<GetPendingModalDTO>()
        //    {
        //        metaData = new MetaData()
        //        {
        //            page = pagingRequest.page,
        //            size = pagingRequest.pageSize,
        //            total = result.Item1
        //        },
        //        results = await result.Item2.ToListAsync()
        //    };
        //}

        public async Task<ResponseMessage<bool>> UpdateModalAsync(Guid id, UpdateModalDTO updateModalDTO, ThisUserObj thisUserObj)
        {
            var existedModal = await _modalRepository.GetByIdAsync(id, isTracking: true);
            if (existedModal == null)
            {
                throw new NotFoundException("Không tìm thấy modal này");
            }

            existedModal = _mapper.Map(updateModalDTO, existedModal);

            await _modalRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật modal thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<GetModalDTO>> UpdateModalisActiveAsync(Guid id, bool isActive)
        {
            var existedModal = await _modalRepository.GetByIdAsync(id, isTracking: true);
            if (existedModal != null)
            {
                existedModal.IsActive = isActive;
                await _modalRepository.UpdateAsync(existedModal);
                return new ResponseMessage<GetModalDTO>()
                {
                    message = "Đổi isActive thành công",
                    result = true,
                    value = _mapper.Map<GetModalDTO>(existedModal)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy modal");

            }
        }

        public async Task<ResponseMessage<GetModalDTO>> UpdateModalStatusAsync(Guid id, VoucherStatusEnum modalStatus)
        {
            var existedModal = await _modalRepository.GetByIdAsync(id, isTracking: true);
            if (existedModal != null)
            {
                existedModal.Status = modalStatus.ToString();
                await _modalRepository.UpdateAsync(existedModal);
                return new ResponseMessage<GetModalDTO>()
                {
                    message = "Đổi status thành công",
                    result = true,
                    value = _mapper.Map<GetModalDTO>(existedModal)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy modal");
            }
        }
    }
}
