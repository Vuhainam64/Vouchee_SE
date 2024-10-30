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
using Vouchee.Data.Models.Constants.Enum.Other;
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
    public class VoucherService : IVoucherService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IBrandRepository _brandReposiroty;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public VoucherService(IUserRepository userRepository,
                                IImageRepository imageRepository,
                                ISupplierRepository supplierRepository,
                                IBrandRepository brandRepository,
                                IPromotionRepository promotionRepository,
                                ICategoryRepository categoryRepository,
                                IFileUploadService fileUploadService,
                                IVoucherRepository voucherRepository,
                                IOrderDetailRepository orderDetailRepository,
                                IMapper mapper)
        {
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _supplierRepository = supplierRepository;
            _brandReposiroty = brandRepository;
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
                var existedCategory = await _categoryRepository.FindAsync(categoryId, false);
                if (existedCategory == null)
                {
                    throw new NotFoundException($"Không thấy category với id {categoryId}");
                }

                _voucherRepository.Attach(existedCategory);
                voucher.Categories.Add(existedCategory);
            }

            var existedBrand = await _brandReposiroty.FindAsync((Guid)createVoucherDTO.brandId, false);
            if (existedBrand == null)
            {
                throw new NotFoundException($"Không tìm thấy brand với id {createVoucherDTO.brandId}");
            }

            var existedSupplier = await _supplierRepository.FindAsync((Guid)createVoucherDTO.supplierId, false);
            if (existedSupplier == null)
            {
                throw new NotFoundException($"Không tìm thấy supplier với id {createVoucherDTO.supplierId}");
            }

            voucher.SellerID = thisUserObj.userId;

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
               
            var voucherId = await _voucherRepository.AddAsync(voucher);

            return new ResponseMessage<dynamic>()
            {
                message = "Tạo voucher thành công",
                result = true,
                value = voucherId
            };
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa voucher");
            }
        }

        public async Task<IList<GetVoucherDTO>> GetNewestVouchers(int numberOfVoucher)
        {
            DateTime currentDate = DateTime.Now;
            IList<GetVoucherDTO> result;
            result = await _voucherRepository.GetTable()
                                        .Include(x => x.Medias)
                                        .Include(x => x.Categories)
                                        .Include(x => x.Brand)
                                        .Include(x => x.Promotions.Where(x => x.StartDate <= currentDate && currentDate <= x.EndDate))
                                        .Include(x => x.Modals)
                                            .ThenInclude(x => x.VoucherCodes)
                                        .OrderByDescending(x => x.CreateDate)
                                        .Take(numberOfVoucher != 0 ? numberOfVoucher : 10)
                                        .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                                        .ToListAsync();
            return result;
        }

        public async Task<IList<GetBestSoldVoucherDTO>> GetTopSaleVouchers(int numberOfVoucher)
        {
            //IQueryable<GetBestSoldVoucherDTO> voucher;

            //var orderDetails = _orderDetailRepository.GetTable()
            //    .GroupBy(od => od.ModalId)
            //    .Select(group => new
            //    {
            //        VoucherId = group.Key,
            //        TotalQuantitySold = group.Sum(od => od.Quantity)
            //    })
            //    .OrderByDescending(vs => vs.TotalQuantitySold)
            //    .ToList();

            //voucher = _voucherRepository.GetTable()
            //    .Include(x => x.Medias)
            //    .Include(x => x.Supplier)
            //    .Include(x => x.Categories)
            //    .Include(x => x.Brand)
            //    .Where(v => orderDetails.Select(od => od.VoucherId).Contains(v.Id))
            //    .ProjectTo<GetBestSoldVoucherDTO>(_mapper.ConfigurationProvider);

            //var result = voucher.ToList().Select(voucher => new GetBestSoldVoucherDTO
            //{
            //    video = voucher.video,
            //    id = voucher.id,
            //    title = voucher.title,
            //    image = voucher.modals.FirstOrDefault(x => x.index == 0).image,
            //    originalPrice = voucher.modals.FirstOrDefault(x => x.index == 0).originalPrice,
            //    sellPrice = voucher.modals.FirstOrDefault(x => x.index == 0).sellPrice,
            //    totalQuantitySold = orderDetails.First(od => od.VoucherId == voucher.id).TotalQuantitySold,
            //    categories = voucher.categories,
            //    brandId = voucher.brandId,
            //    brandImage = voucher.brandImage,
            //    brandName = voucher.brandName,
            //    modals = voucher.modals,
            //    supplierId = voucher.supplierId,
            //    supplierImage = voucher.supplierImage,
            //    supplierName = voucher.supplierName,
            //    quantity = voucher.quantity,
            //    rating = voucher.rating,
            //})
            //.OrderByDescending(v => v.totalQuantitySold)
            //.ToList();

            //var result2 = result;

            //foreach (var test in result2)
            //{
            //    var currentDate = DateTime.Now;
            //    var promotions = _voucherRepository.GetByIdAsync(test.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

            //    var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

            //    if (availablePromotion != null)
            //    {
            //        test.salePrice = test.originalPrice - (test.originalPrice * availablePromotion.PercentDiscount / 100);
            //        test.percentDiscount = availablePromotion.PercentDiscount;
            //    }
            //}

            //return result2;

            return null;
        }

        public async Task<GetVoucherDTO> GetVoucherByIdAsync(Guid id)
        {
            return null;

            //var voucher = await _voucherRepository.GetByIdAsync(id,
            //                        query => query.Include(x => x.Brand)
            //                                            .ThenInclude(x => x.Addresses)
            //                                        .Include(x => x.Supplier)
            //                                        .Include(x => x.Categories)
            //                                            .ThenInclude(x => x.VoucherType)
            //                                        .Include(x => x.Seller)
            //                                        .Include(x => x.Modals)
            //                                            .ThenInclude(x => x.VoucherCodes)
            //                                        .Include(x => x.Medias));

            //if (voucher != null)
            //{
            //    GetDetailVoucherDTO voucherDTO = _mapper.Map<GetDetailVoucherDTO>(voucher);

            //    voucherDTO.image = voucher.Modals.FirstOrDefault(x => x.Index == 0).Image;
            //    voucherDTO.originalPrice = voucher.Modals.FirstOrDefault(x => x.Index == 0).OriginalPrice;
            //    voucherDTO.sellPrice = voucher.Modals.FirstOrDefault(x => x.Index == 0).SellPrice;

            //    var currentDate = DateTime.Now;
            //    var promotions = _voucherRepository.GetByIdAsync(voucherDTO.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

            //    var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

            //    if (availablePromotion != null)
            //    {
            //        voucherDTO.salePrice = voucherDTO.originalPrice - (voucherDTO.originalPrice * availablePromotion.PercentDiscount / 100);
            //        voucherDTO.percentDiscount = availablePromotion.PercentDiscount;
            //    }

            //    foreach (var modal in voucherDTO.modals)
            //    {
            //        modal.quantity = modal.voucherCodes.Where(x => x.status == ObjectStatusEnum.ACTIVE.ToString()).Count();
            //    }

            //    voucherDTO.quantity = voucherDTO.modals.Sum(x => x.quantity);

            //    return voucherDTO;
            //}
            //else
            //{
            //    throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            //}
        }

        public async Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật voucher");
            }
        }

        public async Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherAsync(PagingRequest pagingRequest, 
                                                                            VoucherFilter voucherFilter, 
                                                                            IList<Guid>? categoryIds)
        {
            (int, IQueryable<GetVoucherDTO>) result;

            result = _voucherRepository.GetTable()
                        .Include(x => x.Medias)
                        .Include(x => x.Supplier)
                        .Include(x => x.Categories)
                        .Include(x => x.Brand)
                        .Include(x => x.Modals)
                            .ThenInclude(x => x.VoucherCodes)
                        .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetVoucherDTO>(voucherFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetVoucherDTO>()
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

        public async Task<DynamicDistanceResponseModel<GetVoucherDTO>> GetDetailVouchersAsync(DistanceFilter distanceFilter,
                                                                                        VoucherFilter voucherFilter,
                                                                                        IList<Guid>? categoryIds)
        {
            //var result= _voucherRepository.GetTable(includeProperties: x => x
            //            .Include(x => x.Supplier)
            //            .Include(x => x.Categories)
            //            .Include(x => x.Modals)
            //                .ThenInclude(x => x.VoucherCodes)
            //            .Include(x => x.Medias)
            //            .Include(x => x.Brand)
            //                .ThenInclude(x => x.Addresses))
            //            .ProjectTo<GetDetailVoucherDTO>(_mapper.ConfigurationProvider)
            //            .Where(x =>
            //                (string.IsNullOrEmpty(voucherFilter.title) || x.title.ToLower().Contains(voucherFilter.title.ToLower()))
            //                    && (categoryIds == null || !categoryIds.Any() || x.categories.Any(c => categoryIds.Contains(c.id.Value))))
            //            .AsEnumerable()
            //            .Select(v =>
            //            {
            //                v.addresses = v.addresses?
            //                    .Where(a => a.lon.HasValue && a.lat.HasValue) // Filter addresses with lat/lon
            //                    .Select(a =>
            //                    {
            //                        a.distance = DistanceHelper.CalculateDistance(distanceFilter.lat, distanceFilter.lon, a.lat.Value, a.lon.Value);
            //                        return a;
            //                    })
            //                    .OrderBy(d => d.distance)
            //                    .Take(distanceFilter.numberOfAddress);
            //                return v;
            //            })
            //            .Take(distanceFilter.numberOfVoucher);

            //var response = new DynamicDistanceResponseModel<GetDetailVoucherDTO>
            //{
            //    distanceData = new DistanceData
            //    {
            //        numberOfAddress = distanceFilter.numberOfAddress,
            //        numberOfVoucher = distanceFilter.numberOfVoucher
            //    },
            //    results = result.ToList()
            //};

            //foreach (var voucher in response.results)
            //{
            //    voucher.image = voucher.medias.FirstOrDefault(x => x.index == 0)?.url;
            //    voucher.originalPrice = voucher.modals.FirstOrDefault(x => x.index == 0)?.originalPrice;
            //    voucher.sellPrice = voucher.modals.FirstOrDefault(x => x.index == 0)?.sellPrice;
            //    voucher.quantity = voucher.modals.Sum(modal => modal.voucherCodes?.Count);
            //}

            //return response;

            return null;
        }

        public async Task<IList<GetVoucherDTO>> GetSalestVouchers(int numberOfVoucher)
        {
            //DateTime currenDate = DateTime.Now;
            //IList<GetVoucherDTO> vouchers = [];

            //List<Promotion> promotions = _promotionRepository.GetTable()
            //                                                    .Include(x => x.Vouchers)
            //                                                        .ThenInclude(v => v.Categories) // Include categories of the vouchers
            //                                                    .Include(x => x.Vouchers)
            //                                                        .ThenInclude(v => v.Brand) // Include the brand of the vouchers
            //                                                    .Include(x => x.Vouchers)
            //                                                        .ThenInclude(x => x.Medias)
            //                                                    .Include(x => x.Vouchers)
            //                                                        .ThenInclude(x => x.Supplier)
            //                                                    .Include(x => x.Vouchers)
            //                                                        .ThenInclude(x => x.Modals)
            //                                                    .Where(x => x.StartDate <= currenDate && currenDate <= x.EndDate)
            //                                                    .ToList();
            //if (promotions.Count != 0)
            //{
            //    foreach (var promotion in promotions)
            //    {
            //        if (promotion.Vouchers.Count != 0)
            //        {
            //            foreach (var voucher in promotion.Vouchers)
            //            {
            //                var existedVoucher = _mapper.Map<GetVoucherDTO>(voucher);

            //                existedVoucher.sellPrice = existedVoucher.modals.FirstOrDefault(x => x.index == 0).sellPrice;
            //                existedVoucher.originalPrice = existedVoucher.modals.FirstOrDefault(x => x.index == 0).originalPrice;
            //                existedVoucher.salePrice = existedVoucher.originalPrice - (existedVoucher.originalPrice * promotion.PercentDiscount / 100);
            //                existedVoucher.percentDiscount = promotion.PercentDiscount;
            //                existedVoucher.image = voucher.Medias.Count != 0 ? voucher.Medias.FirstOrDefault(x => x.Index == 0).Url : null;
            //                vouchers.Add(existedVoucher);
            //            }
            //        }
            //    }
            //    return vouchers;
            //}

            return null;
        }

        public async Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherBySellerId(Guid sellerId, 
                                                                                        PagingRequest pagingRequest, 
                                                                                        VoucherFilter voucherFilter, 
                                                                                        IList<Guid>? categoryIds)
        {
            //var existedSeller = await _userRepository.FindAsync(sellerId, false);

            //if (existedSeller == null)
            //{
            //    throw new NotFoundException("Không tìm thấy seller này");
            //}

            //(int, IQueryable<GetVoucherDTO>) result;

            //result = _voucherRepository.GetTable().Where(x =>
            //                                                (string.IsNullOrEmpty(voucherFilter.title) || x.Title.ToLower().Contains(voucherFilter.title.ToLower())) 
            //                                                && (categoryIds == null || !categoryIds.Any() || x.Categories.Any(c => categoryIds.Contains(c.Id)))
            //                                            )
            //                                        .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
            //                                        .DynamicFilter(_mapper.Map<GetVoucherDTO>(voucherFilter))
            //                                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);


            //var response = new DynamicResponseModel<GetVoucherDTO>()
            //{
            //    metaData = new MetaData()
            //    {
            //        page = pagingRequest.page,
            //        size = pagingRequest.pageSize,
            //        total = result.Item1 // Total vouchers count for metadata
            //    },
            //    results = result.Item2.ToList()
            //};

            //if (response.results != null && response.results.Count() != 0)
            //{

            //    foreach (var voucher in response.results)
            //    {
            //        voucher.originalPrice = voucher.modals.FirstOrDefault(x => x.index == 0).originalPrice;
            //        voucher.sellPrice = voucher.modals.FirstOrDefault(x => x.index == 0).sellPrice;
            //        voucher.image = voucher.modals.FirstOrDefault(x => x.index == 0).image;

            //        var currentDate = DateTime.Now;
            //        var promotions = _voucherRepository.GetByIdAsync(voucher.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

            //        var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

            //        if (availablePromotion != null)
            //        {
            //            voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.PercentDiscount / 100);
            //            voucher.percentDiscount = availablePromotion.PercentDiscount;
            //        }

            //        foreach (var modal in voucher.modals)
            //        {
            //            modal.quantity = modal.voucherCodes.Where(x => x.status == ObjectStatusEnum.ACTIVE.ToString()).Count();
            //        }

            //        voucher.quantity = voucher.modals.Sum(x => x.quantity);
            //    }
            //}

            //return response;

            return null;
        }

        //public async Task<IList<GetDetailVoucherDTO>> GetNearestVouchers(decimal lon, decimal lat)
        //{
        //    try
        //    {
        //        decimal R = 6371; // Earth's radius in kilometers
        //        decimal ToRadians(decimal value) => (decimal)(Math.PI / 180) * value;

        //        // Retrieve all vouchers and map to DTO
        //        var vouchers = _voucherRepository.GetTable()
        //            .Include(x => x.Medias)
        //            .Include(x => x.Supplier)
        //            .Include(x => x.Categories)
        //            .Include(x => x.Brand)
        //                .ThenInclude(x => x.Addresses)
        //            .ProjectTo<GetDetailVoucherDTO>(_mapper.ConfigurationProvider)
        //            .ToList(); // Materialize the data

        //        // Calculate nearest vouchers based on addresses
        //        var nearestVouchers = vouchers
        //            .Select(voucher =>
        //            {
        //                var nearestAddress = voucher.addresses
        //                    .Where(a => a.lat.HasValue && a.lon.HasValue)
        //                    .Select(a =>
        //                    {
        //                        decimal dLat = ToRadians(lat - (decimal)a.lat);
        //                        decimal dLon = ToRadians(lon - (decimal)a.lon);
        //                        decimal lat1 = ToRadians(lat);
        //                        decimal lat2 = ToRadians((decimal)a.lat);

        //                        decimal aVal = (decimal)(Math.Sin((double)dLat / 2) * Math.Sin((double)dLat / 2) +
        //                                                 Math.Cos((double)lat1) * Math.Cos((double)lat2) *
        //                                                 Math.Sin((double)dLon / 2) * Math.Sin((double)dLon / 2));

        //                        decimal c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)aVal), Math.Sqrt(1 - (double)aVal));

        //                        decimal distance = R * c; // Calculate distance

        //                        return new
        //                        {
        //                            Address = a,
        //                            Distance = Math.Round(distance, 2) // Round distance to 2 decimal places
        //                        };
        //                    })
        //                    .OrderBy(a => a.Distance) // Sort by closest distance
        //                    .Take(5);

        //                if (nearestAddress != null)
        //                {
        //                    return new GetDetailVoucherDTO
        //                    {
        //                        video = voucher.video,
        //                        id = voucher.id,
        //                        title = voucher.title,
        //                        categories = voucher.categories,
        //                        image = voucher.modals.FirstOrDefault(x => x.index == 0).image,
        //                        originalPrice = voucher.modals.FirstOrDefault(x => x.index == 0).originalPrice,
        //                        sellPrice = voucher.modals.FirstOrDefault(x => x.index == 0).sellPrice,
        //                        brandId = voucher.brandId,
        //                        brandImage = voucher.brandImage,
        //                        brandName = voucher.brandName,
        //                        supplierId = voucher.supplierId,
        //                        supplierImage = voucher.supplierImage,
        //                        supplierName = voucher.supplierName,
        //                        quantity = voucher.quantity,
        //                        rating = voucher.rating,
        //                        addresses = nearestAddress
        //                                .Select(na => new GetDistanceAddressDTO
        //                                {
        //                                    id = na.Address.id,
        //                                    name = na.Address.name,
        //                                    lat = na.Address.lat,
        //                                    lon = na.Address.lon,
        //                                    distance = Math.Round(na.Distance, 2) // Round distance to 2 decimal places
        //                                })
        //                                .ToList() // Map to address DTOs and create a list of nearest 5 addresses
        //                    };
        //                }
        //                return null;
        //            })
        //            .Where(v => v != null) // Filter out vouchers without a nearest address
        //            .Take(8) // Limit to the 8 nearest vouchers
        //            .ToList();

        //        var result2 = nearestVouchers;

        //        foreach (var test in result2)
        //        {
        //            var currentDate = DateTime.Now;
        //            var promotions = _voucherRepository.GetByIdAsync(test.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

        //            var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

        //            if (availablePromotion != null)
        //            {
        //                test.salePrice = test.originalPrice - (test.originalPrice * availablePromotion.PercentDiscount / 100);
        //                test.percentDiscount = availablePromotion.PercentDiscount;
        //            }
        //        }

        //        return nearestVouchers;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerService.Logger(ex.Message);
        //        throw new LoadException("An unexpected error occurred while loading nearest vouchers");
        //    }
        //}

        //public async Task<DynamicResponseModel<GetDetailVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
        //                                                                                 VoucherFilter voucherFilter,
        //                                                                                 List<Guid>? categoryID)
        //{
        //    (int, IQueryable<GetDetailVoucherDTO>) result;
        //    try
        //    {
        //        result.Item2 = _voucherRepository.GetTable(includeProperties: x => x
        //                                .Include(x => x.Supplier)
        //                                .Include(x => x.Categories)
        //                                .Include(x => x.Modals)
        //                                    .ThenInclude(x => x.VoucherCodes)
        //                                .Include(x => x.Medias)
        //                                .Include(x => x.Brand)
        //                                    .ThenInclude(x => x.Addresses))
        //                                .ProjectTo<GetDetailVoucherDTO>(_mapper.ConfigurationProvider)
        //                                .Where(x =>
        //                                    (string.IsNullOrEmpty(voucherFilter.title) || x.title.ToLower().Contains(voucherFilter.title.ToLower())) 
        //                                    && (categoryID == null || !categoryID.Any() || x.categories.Any(c => categoryID.Contains(c.id.Value))))
        //                                .AsEnumerable() 
        //                                .Select(v =>
        //                                {
        //                                    v.addresses = v.addresses?
        //                                        .Where(a => a.lon.HasValue && a.lat.HasValue) // Filter addresses with lat/lon
        //                                        .Select(a =>
        //                                        {
        //                                            a.distance = DistanceHelper.CalculateDistance(lat, lon, a.lat.Value, a.lon.Value);
        //                                            return a;
        //                                        })
        //                                        .OrderBy(d => d.distance) 
        //                                        .Take(pagingRequest.pageSize != 0 ? pagingRequest.pageSize : 10);

        //                                    return v; 
        //                                })
        //                                .Take(pagingRequest.page != 0 ? pagingRequest.page : 5)
        //                                .AsQueryable();

        //        var result2 = result.Item2.ToList();

        //        foreach (var voucher in result2)
        //        {
        //            voucher.image = voucher.medias.FirstOrDefault(x => x.index == 0)?.url;
        //            voucher.originalPrice = voucher.modals.FirstOrDefault(x => x.index == 0)?.originalPrice;
        //            voucher.sellPrice = voucher.modals.FirstOrDefault(x => x.index == 0)?.sellPrice;
        //            voucher.quantity = voucher.modals.Sum(modal => modal.voucherCodes?.Count);
        //        }

        //        return new DynamicResponseModel<GetDetailVoucherDTO>
        //        {
        //            metaData = new MetaData
        //            {
        //                page = pagingRequest.page,
        //                size = pagingRequest.pageSize,
        //                total = result.Item2.Count()
        //            },
        //            results = result2
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerService.Logger($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
        //        throw new LoadException("An unexpected error occurred while loading vouchers.");
        //    }
        //}

        //public async Task<IList<GetVoucherDTO>> GetSalestVouchers()
        //{
        //    // check all active promotion
        //    DateTime currenDate = DateTime.Now;
        //    List<GetVoucherDTO> vouchers = new();

        //    List<Promotion> promotions = _promotionRepository.GetTable()
        //                                                        .Include(x => x.Vouchers)
        //                                                            .ThenInclude(v => v.Categories) // Include categories of the vouchers
        //                                                        .Include(x => x.Vouchers)
        //                                                            .ThenInclude(v => v.Brand) // Include the brand of the vouchers
        //                                                        .Include(x => x.Vouchers)
        //                                                            .ThenInclude(x => x.Medias)
        //                                                        .Include(x => x.Vouchers)
        //                                                            .ThenInclude(x => x.Supplier)
        //                                                        .Include(x => x.Vouchers)
        //                                                            .ThenInclude(x => x.Modals)
        //                                                        .ToList()
        //                                                        .Where(x => x.StartDate <= currenDate && currenDate <= x.EndDate)
        //                                                        .ToList();
        //    if (promotions.Count != 0)
        //    {
        //        foreach (var promotion in promotions)
        //        {
        //            if (promotion.Vouchers.Count != 0)
        //            {
        //                foreach (var voucher in promotion.Vouchers)
        //                {
        //                    var existedVoucher = _mapper.Map<GetVoucherDTO>(voucher);

        //                    existedVoucher.sellPrice = existedVoucher.modals.FirstOrDefault(x => x.index == 0).sellPrice;
        //                    existedVoucher.originalPrice = existedVoucher.modals.FirstOrDefault(x => x.index == 0).originalPrice;
        //                    existedVoucher.salePrice = existedVoucher.originalPrice - (existedVoucher.originalPrice * promotion.PercentDiscount / 100);
        //                    existedVoucher.percentDiscount = promotion.PercentDiscount;
        //                    existedVoucher.image = voucher.Medias.Count != 0 ? voucher.Medias.FirstOrDefault(x => x.Index == 0).Url : null;
        //                    vouchers.Add(existedVoucher);
        //                }
        //            }
        //        }
        //        return vouchers;
        //    }

        //    return null;
        //}
    }
}
