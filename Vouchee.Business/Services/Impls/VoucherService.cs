using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Business.Utils;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class VoucherService : IVoucherService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Media> _mediaRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<Brand> _brandReposiroty;
        private readonly IBaseRepository<Promotion> _promotionRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<OrderDetail> _orderDetailRepository;
        private readonly IMapper _mapper;

        public VoucherService(IBaseRepository<User> userRepository,
                                IBaseRepository<Media> mediaRepository,
                                IBaseRepository<Supplier> supplierRepository,
                                IBaseRepository<Brand> brandReposiroty,
                                IBaseRepository<Promotion> promotionRepository,
                                IBaseRepository<Category> categoryRepository,
                                IFileUploadService fileUploadService,
                                IBaseRepository<Voucher> voucherRepository,
                                IBaseRepository<OrderDetail> orderDetailRepository,
                                IMapper mapper)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _supplierRepository = supplierRepository;
            _brandReposiroty = brandReposiroty;
            _promotionRepository = promotionRepository;
            _categoryRepository = categoryRepository;
            _fileUploadService = fileUploadService;
            _voucherRepository = voucherRepository;
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<dynamic>> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO, ThisUserObj thisUserObj)
        {
            Voucher voucher = _mapper.Map<Voucher>(createVoucherDTO);

            foreach (var categoryId in createVoucherDTO.categoryId)
            {
                var existedCategory = await _categoryRepository.FindAsync(categoryId, true);
                if (existedCategory == null)
                {
                    throw new NotFoundException($"Không thấy category với id {categoryId}");
                }
                voucher.Categories.Add(existedCategory);
            }

            var existedBrand = await _brandReposiroty.FindAsync((Guid)createVoucherDTO.brandId);
            if (existedBrand == null)
            {
                throw new NotFoundException($"Không tìm thấy brand với id {createVoucherDTO.brandId}");
            }

            var existedSupplier = await _supplierRepository.FindAsync((Guid)createVoucherDTO.supplierId);
            if (existedSupplier == null)
            {
                throw new NotFoundException($"Không tìm thấy supplier với id {createVoucherDTO.supplierId}");
            }

            voucher.SellerId = thisUserObj.userId;

            // MODAL 

            if (voucher.Modals != null && voucher.Modals.Count != 0)
            {
                int index = 0;
                foreach (var m in voucher.Modals)
                {
                    m.CreateBy = thisUserObj.userId;
                    m.Index = index;
                    index++;
                }
            }

            // IMAGE
            if (createVoucherDTO.images?.Count != 0)
            {
                int index = 0;
                foreach (var image in createVoucherDTO.images)
                {
                    Media media = new()
                    {
                        Status = ObjectStatusEnum.ACTIVE.ToString(),
                        Url = image,
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Index = index++,
                    };
                    voucher.Medias.Add(media);
                }
            }

            voucher = await _voucherRepository.Add(voucher);

            return new ResponseMessage<dynamic>()
            {
                message = "Tạo voucher thành công",
                result = true,
                value = voucher.Id
            };
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            bool result = false;
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher != null)
            {
                result = await _voucherRepository.DeleteAsync(voucher);
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }
            return result;
        }

        public async Task<IList<GetVoucherDTO>> GetNewestVouchers(int numberOfVoucher)
        {
            var result = _voucherRepository.GetTable()
                                .OrderByDescending(v => v.CreateDate)
                                .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                                .Where(x => x.stock > 0 && x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true)
                                .Take(numberOfVoucher != 0 ? numberOfVoucher : 10);

            return await result.ToListAsync();
        }

        public async Task<IList<GetVoucherDTO>> GetTopSaleVouchers(int numberOfVoucher)
        {
            IQueryable<GetVoucherDTO> result;

            result = _voucherRepository.GetTable().Include(x => x.Modals)
                                                    .ThenInclude(x => x.OrderDetails)
                        .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                        .Where(x => x.stock > 0 && x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true)
                        .OrderByDescending(x => x.totalQuantitySold)
                        .Take(numberOfVoucher != 0 ? numberOfVoucher : 10);

            return await result.ToListAsync();
        }

        public async Task<dynamic> GetVoucherByIdAsync(Guid id, PagingRequest pagingRequest)
        {
            var existedVoucher = await _voucherRepository.GetByIdAsync(id,
                                                query => query.Include(x => x.Brand)
                                                                    .ThenInclude(x => x.Addresses)
                                                                .Include(x => x.Supplier)
                                                                .Include(x => x.Categories)
                                                                    .ThenInclude(x => x.VoucherType)
                                                                .Include(x => x.Seller)
                                                                .Include(x => x.Modals)
                                                                    .ThenInclude(x => x.VoucherCodes)
                                                                .Include(x => x.Medias)
                                                                .Include(x => x.Promotions));

            if (existedVoucher != null)
            {
                var voucher = _mapper.Map<GetDetailVoucherDTO>(existedVoucher);
                var total = voucher.addresses.Count();

                var pagedAddresses = voucher.addresses
                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                    .Take(pagingRequest.pageSize).ToList();

                voucher.addresses = pagedAddresses;

                return new
                {
                    metaData = new
                    {
                        page = pagingRequest.page,
                        size = pagingRequest.pageSize,
                        total = total
                    },
                    results = voucher
                };
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }
        }

        public async Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO)
        {
            var existedVoucher = await _voucherRepository.GetByIdAsync(id);
            if (existedVoucher != null)
            {
                var entity = _mapper.Map<Voucher>(updateVoucherDTO);
                return await _voucherRepository.UpdateAsync(entity);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy voucher");
            }
        }
        
        public async Task<ResponseMessage<GetVoucherDTO>> UpdateVoucherStatusAsync(Guid id, VoucherStatusEnum voucherStatus)
        {
            var existedVoucher = await _voucherRepository.GetByIdAsync(id, isTracking: true);
            if (existedVoucher != null)
            {
                existedVoucher.Status = voucherStatus.ToString();
                await _voucherRepository.UpdateAsync(existedVoucher);
                return new ResponseMessage<GetVoucherDTO>()
                {
                    message = "Đổi Status thành công",
                    result = true,
                    value = _mapper.Map<GetVoucherDTO>(existedVoucher)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy voucher");
            }
        }
        
        public async Task<ResponseMessage<GetVoucherDTO>> UpdateVoucherisActiveAsync(Guid id, bool isActive)
        {
            var existedVoucher = await _voucherRepository.GetByIdAsync(id, isTracking: true);
            if (existedVoucher != null)
            {
                /*existedVoucher.IsActive = (existedVoucher.IsActive == true) ? false : true;*/
                existedVoucher.IsActive = isActive;
                await _voucherRepository.UpdateAsync(existedVoucher);
                return new ResponseMessage<GetVoucherDTO>()
                {
                    message = "Đổi isActive thành công",
                    result = true,
                    value = _mapper.Map<GetVoucherDTO>(existedVoucher)
                };
            }
            else
            {
                throw new NotFoundException("Không tìm thấy voucher");
            }
        }
        
        public async Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherAsync(PagingRequest pagingRequest,
                                                                        VoucherFilter voucherFilter,
                                                                        SortVoucherEnum sortVoucherEnum)
        {
            (int, IQueryable<GetVoucherDTO>) result;

            var query = _voucherRepository.GetTable()
                            .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetVoucherDTO>(voucherFilter))
                            .Where(x =>
                                (voucherFilter.categoryIDs == null || !voucherFilter.categoryIDs.Any() || x.categories.Any(c => voucherFilter.categoryIDs.Contains(c.id.Value))) &&
                                (string.IsNullOrEmpty(voucherFilter.title) || x.title.Contains(voucherFilter.title)) &&
                                (!voucherFilter.status.HasValue || x.status == voucherFilter.status.ToString()) &&
                                (!voucherFilter.isActive.HasValue || x.isActive == voucherFilter.isActive.Value) &&
                                (voucherFilter.supplierIDs == null || !voucherFilter.supplierIDs.Any() || voucherFilter.supplierIDs.Contains((Guid)x.supplierId)) &&
                                (voucherFilter.minPrice == null || x.sellPrice >= voucherFilter.minPrice) &&
                                (voucherFilter.maxPrice == null || x.sellPrice <= voucherFilter.maxPrice)
                            );

            if (voucherFilter.isInStock != null && voucherFilter.isInStock == true)
            {
                query = query.Where(x => x.stock > 0);
            }

            query = sortVoucherEnum switch
            {
                SortVoucherEnum.NEWEST => query.OrderByDescending(x => x.createDate),
                SortVoucherEnum.SELLEST => query.OrderByDescending(x => x.totalQuantitySold),
                SortVoucherEnum.PRICE_ASCENDING => query.OrderBy(x => x.sellPrice),
                SortVoucherEnum.PRICE_DESCENDING => query.OrderByDescending(x => x.sellPrice),
                _ => query
            };

            result = query.PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetVoucherDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1
                },
                results = await result.Item2.ToListAsync() 
            };
        }

        public async Task<DynamicResponseModel<GetNearestVoucherDTO>> GetNearestVouchersAsync(PagingRequest pagingRequest,
                                                                                                DistanceFilter distanceFilter,
                                                                                                VoucherFilter voucherFilter,
                                                                                                IList<Guid>? categoryIds)
        {
            var result = _voucherRepository.GetTable().Include(x => x.Brand)
                                                            .ThenInclude(x => x.Addresses)
                        .ProjectTo<GetNearestVoucherDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetNearestVoucherDTO>(voucherFilter))
                        .Where(x => categoryIds == null || !categoryIds.Any() || x.categories.Any(c => categoryIds.Contains(c.id.Value)))
                        .Where(x => x.stock > 0 && x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true)
                        .AsEnumerable()
                        .Select(v =>
                        {
                            v.addresses = v.addresses
                                .Where(a => a.lon.HasValue && a.lat.HasValue)
                                .Select(a =>
                                {
                                    a.distance = DistanceHelper.CalculateDistance(distanceFilter.lat, distanceFilter.lon, a.lat.Value, a.lon.Value);
                                    return a;
                                })
                                .OrderBy(d => d.distance)
                                .Take(distanceFilter.numberOfAddress != 0 ? distanceFilter.numberOfAddress : 10);
                            return v;
                        });

            // không xài được PagingIQueryable, chưa hiểu tại sao
            var pagedVouchers = result
                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                    .Take(pagingRequest.pageSize);

            return new DynamicResponseModel<GetNearestVoucherDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Count() // Total vouchers count for metadata
                },
                results = pagedVouchers.ToList() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<IList<GetVoucherDTO>> GetSalestVouchers(int numberOfVoucher)
        {
            var result = _voucherRepository.GetTable()
                    .OrderByDescending(v => v.CreateDate)
                    .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                    .Where(x => x.stock > 0 && x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true && x.percentDiscount != null )
                    .OrderByDescending(x => x.percentDiscount);

            return await result.ToListAsync();
        }

        public async Task<DynamicResponseModel<GetVoucherSellerDTO>> GetVoucherBySellerId(Guid sellerId,
                                                                                        PagingRequest pagingRequest,
                                                                                        VoucherFilter voucherFilter,
                                                                                        IList<Guid>? categoryIds)
        {
            var existedSeller = await _userRepository.FindAsync(sellerId, false);

            if (existedSeller == null)
            {
                throw new NotFoundException("Không tìm thấy seller này");
            }

            (int, IQueryable<GetVoucherSellerDTO>) result;

            result = _voucherRepository.GetTable().Include(x => x.Seller)
                                                    .Include(x => x.Brand)
                                                    .Where(x => (categoryIds == null || !categoryIds.Any() || x.Categories.Any(c => categoryIds.Contains(c.Id)))
                                                            && x.SellerId == sellerId)
                                                    .ProjectTo<GetVoucherSellerDTO>(_mapper.ConfigurationProvider)
                                                    .DynamicFilter(_mapper.Map<GetVoucherSellerDTO>(voucherFilter))
                                                    .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);


            var response = new DynamicResponseModel<GetVoucherSellerDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync()
            };

            return response;
        }
    }
}
