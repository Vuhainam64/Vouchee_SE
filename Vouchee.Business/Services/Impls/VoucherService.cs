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
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class VoucherService : IVoucherService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IBrandRepository _brandReposiroty;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public VoucherService(IImageRepository imageRepository,
                                ISupplierRepository supplierRepository,
                                IBrandRepository brandRepository,
                                IPromotionRepository promotionRepository,
                                ICategoryRepository categoryRepository,
                                IFileUploadService fileUploadService,
                                IVoucherRepository voucherRepository,
                                IOrderDetailRepository orderDetailRepository,
                                IMapper mapper)
        {
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

        public async Task<Guid?> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO, ThisUserObj thisUserObj)
        {
            try
            {
                Voucher voucher = _mapper.Map<Voucher>(createVoucherDTO);

                foreach (var categoryId in createVoucherDTO.categoryId)
                {
                    var existedCategory = await _categoryRepository.FindAsync(categoryId);
                    if (existedCategory == null)
                    {
                        throw new NotFoundException($"Không thấy category với id {categoryId}");
                    }
                    _voucherRepository.Attach(existedCategory);
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

                voucher.SellerID = thisUserObj.userId;

                if (createVoucherDTO.productImageUrl != null && createVoucherDTO.productImageUrl.Count != 0)
                {
                    foreach (var productImage in createVoucherDTO.productImageUrl)
                    {
                        Media image = new()
                        {
                            Url = productImage,
                            Status = ObjectStatusEnum.ACTIVE.ToString(),
                            CreateBy = thisUserObj.userId,
                            CreateDate = DateTime.Now,
                            Type = MediaEnum.PRODUCT.ToString(),
                        };

                        //voucher.Medias.Add(image);
                    }
                }

                if (createVoucherDTO.advertisingImageUrl != null)
                {
                    Media image = new()
                    {
                        Url = createVoucherDTO.advertisingImageUrl,
                        Status = ObjectStatusEnum.ACTIVE.ToString(),
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Type = MediaEnum.ADVERTISEMENT.ToString()
                    };

                    //if (image.Url != null)
                    //{
                    //    voucher.Medias.Add(image);
                    //}
                }

                if (createVoucherDTO.videoUrl != null)
                {
                    Media image = new()
                    {
                        Url = createVoucherDTO.videoUrl,
                        Status = ObjectStatusEnum.ACTIVE.ToString(),
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Type = MediaEnum.VIDEO.ToString()
                    };

                    //if (image.Url != null)
                    //{
                    //    voucher.Medias.Add(image);
                    //}
                }

                if (createVoucherDTO.modals != null && createVoucherDTO.modals.Count != 0)
                {
                    foreach (var modal in createVoucherDTO.modals)
                    {
                        Modal newModal = _mapper.Map<Modal>(modal);
                        newModal.CreateBy = thisUserObj.userId;
                        voucher.Modals.Add(newModal);
                    }
                }

                var voucherId = await _voucherRepository.AddAsync(voucher);

                return voucherId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo voucher");
            }
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

        //public Task<IList<GetAllVoucherDTO>> GetBestSoldVouchers()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<IList<GetNearestVoucherDTO>> GetNearestVouchers(decimal lon, decimal lat)
        {
            try
            {
                decimal R = 6371; // Earth's radius in kilometers
                decimal ToRadians(decimal value) => (decimal)(Math.PI / 180) * value;

                // Retrieve all vouchers and map to DTO
                var vouchers = _voucherRepository.GetTable()
                    //.Include(x => x.Medias)
                    .Include(x => x.Supplier)
                    .Include(x => x.Categories)
                    .Include(x => x.Brand)
                        .ThenInclude(x => x.Addresses)
                    .ProjectTo<GetNearestVoucherDTO>(_mapper.ConfigurationProvider)
                    .ToList(); // Materialize the data

                // Calculate nearest vouchers based on addresses
                var nearestVouchers = vouchers
                    .Select(voucher =>
                    {
                        var nearestAddress = voucher.addresses
                            .Where(a => a.lat.HasValue && a.lon.HasValue)
                            .Select(a =>
                            {
                                decimal dLat = ToRadians(lat - (decimal)a.lat);
                                decimal dLon = ToRadians(lon - (decimal)a.lon);
                                decimal lat1 = ToRadians(lat);
                                decimal lat2 = ToRadians((decimal)a.lat);

                                decimal aVal = (decimal)(Math.Sin((double)dLat / 2) * Math.Sin((double)dLat / 2) +
                                                         Math.Cos((double)lat1) * Math.Cos((double)lat2) *
                                                         Math.Sin((double)dLon / 2) * Math.Sin((double)dLon / 2));

                                decimal c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)aVal), Math.Sqrt(1 - (double)aVal));

                                decimal distance = R * c; // Calculate distance

                                return new
                                {
                                    Address = a,
                                    Distance = Math.Round(distance, 2) // Round distance to 2 decimal places
                                };
                            })
                            .OrderBy(a => a.Distance) // Sort by closest distance
                            .Take(5);

                        if (nearestAddress != null)
                        {
                            return new GetNearestVoucherDTO
                            {
                                id = voucher.id,
                                title = voucher.title,
                                categories = voucher.categories,
                                image = voucher.medias.Count != 0 ? voucher.medias.FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT).url : null,
                                originalPrice = voucher.originalPrice,
                                sellPrice = voucher.sellPrice,
                                salePrice = voucher.salePrice,
                                medias = voucher.medias,
                                brandId = voucher.brandId,
                                brandImage = voucher.brandImage,
                                brandName = voucher.brandName,
                                supplierId = voucher.supplierId,
                                supplierImage = voucher.supplierImage,
                                supplierName = voucher.supplierName,
                                quantity = voucher.quantity,
                                rating = voucher.rating,
                                addresses = nearestAddress
                                        .Select(na => new GetAllAddressDTO
                                        {
                                            id = na.Address.id,
                                            name = na.Address.name,
                                            lat = na.Address.lat,
                                            lon = na.Address.lon,
                                            distance = Math.Round(na.Distance, 2) // Round distance to 2 decimal places
                                        })
                                        .ToList() // Map to address DTOs and create a list of nearest 5 addresses
                            };
                        }
                        return null;
                    })
                    .Where(v => v != null) // Filter out vouchers without a nearest address
                    .Take(8) // Limit to the 8 nearest vouchers
                    .ToList();

                var result2 = nearestVouchers;

                foreach (var test in result2)
                {
                    var currentDate = DateTime.Now;
                    var promotions = _voucherRepository.GetByIdAsync(test.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

                    var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                    if (availablePromotion != null)
                    {
                        test.salePrice = test.originalPrice - (test.originalPrice * availablePromotion.PercentDiscount / 100);
                        test.percentDiscount = availablePromotion.PercentDiscount;
                    }
                }

                return nearestVouchers;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("An unexpected error occurred while loading nearest vouchers");
            }
        }

        public async Task<IList<GetNewestVoucherDTO>> GetNewestVouchers()
        {
            IList<GetNewestVoucherDTO> getNewestVoucherDTOs;
            IQueryable<GetNewestVoucherDTO> result;
            try
            {
                result = _voucherRepository.GetTable()
                                            //.Include(x => x.Medias)
                                            .Include(x => x.Supplier)
                                            .Include(x => x.Categories)
                                            .Include(x => x.Brand)
                                            .OrderByDescending(x => x.CreateDate)
                                            .Take(8)
                                            .ProjectTo<GetNewestVoucherDTO>(_mapper.ConfigurationProvider);

                getNewestVoucherDTOs = result.ToList();

                if (result != null && result.Count() != 0)
                {

                    foreach (var voucher in getNewestVoucherDTOs)
                    {
                        var currentDate = DateTime.Now;
                        var promotions = _voucherRepository.GetByIdAsync(voucher.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

                        var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                        if (availablePromotion != null)
                        {
                            voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.PercentDiscount / 100);
                            voucher.percentDiscount = availablePromotion.PercentDiscount;
                        }

                        voucher.image = voucher.medias.Count != 0 ? voucher.medias.FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT).url : null;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return getNewestVoucherDTOs;
        }

        public async Task<IList<GetBestBuyVoucherDTO>> GetTopSaleVouchers()
        {
            IQueryable<GetAllVoucherDTO> voucher;

            // Get all vouchers and their total quantity sold
            var orderDetails = _orderDetailRepository.GetTable()
                .GroupBy(od => od.VoucherId)
                .Select(group => new
                {
                    VoucherId = group.Key,
                    TotalQuantitySold = group.Sum(od => od.Quantity)
                })
                .OrderByDescending(vs => vs.TotalQuantitySold)
                /*.Skip((pagingRequest.PageNumber - 1) * pagingRequest.PageSize)
                .Take(pagingRequest.PageSize)*/
                .ToList();

            // Join the vouchers and return with sorted totalQuantitySold
            voucher = _voucherRepository.GetTable()
                //.Include(x => x.Medias)
                .Include(x => x.Supplier)
                .Include(x => x.Categories)
                .Include(x => x.Brand)
                .Where(v => orderDetails.Select(od => od.VoucherId).Contains(v.Id))
                .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider);

            // Attach the total quantity sold to each voucher and sort the result
            var result = voucher.ToList().Select(voucher => new GetBestBuyVoucherDTO
            {
                id = voucher.id,
                title = voucher.title,
                image = voucher.image = voucher.medias.Count != 0 ? voucher.medias.FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT).url : null,
                originalPrice = voucher.originalPrice,
                sellPrice = voucher.sellPrice,
                TotalQuantitySold = orderDetails.First(od => od.VoucherId == voucher.id).TotalQuantitySold,
                categories = voucher.categories,
                brandId = voucher.brandId,
                brandImage = voucher.brandImage,
                brandName = voucher.brandName,
                medias = voucher.medias,
                supplierId = voucher.supplierId,
                supplierImage = voucher.supplierImage,
                supplierName = voucher.supplierName,
                quantity = voucher.quantity,
                rating = voucher.rating,
            })
            .OrderByDescending(v => v.TotalQuantitySold)
            .ToList();

            var result2 = result;

            foreach (var test in result2)
            {
                var currentDate = DateTime.Now;
                var promotions = _voucherRepository.GetByIdAsync(test.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

                var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                if (availablePromotion != null)
                {
                    test.salePrice = test.originalPrice - (test.originalPrice * availablePromotion.PercentDiscount / 100);
                    test.percentDiscount = availablePromotion.PercentDiscount;
                }
            }

            return result2;
        }



        public async Task<GetDetailVoucherDTO> GetVoucherByIdAsync(Guid id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id,
                                    query => query.Include(x => x.VoucherCodes)
                                                    .Include(x => x.Brand)
                                                        .ThenInclude(x => x.Addresses)
                                                    .Include(x => x.Supplier)
                                                    //.Include(x => x.Addresses)
                                                    .Include(x => x.Categories)
                                                        .ThenInclude(x => x.VoucherType)
                                                    //.Include(x => x.Medias)
                                                    //.Include(x => x.VoucherType)
                                                    .Include(x => x.Seller));

            if (voucher != null)
            {
                GetDetailVoucherDTO voucherDTO = _mapper.Map<GetDetailVoucherDTO>(voucher);

                var currentDate = DateTime.Now;
                var promotions = _voucherRepository.GetByIdAsync(voucherDTO.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

                var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                if (availablePromotion != null)
                {
                    voucherDTO.salePrice = voucherDTO.originalPrice - (voucherDTO.originalPrice * availablePromotion.PercentDiscount / 100);
                    voucherDTO.percentDiscount = availablePromotion.PercentDiscount;
                }


                voucherDTO.image = voucherDTO.medias.Count != 0 ? voucherDTO.medias.FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT).url : null;

                return voucherDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }
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

        public async Task<DynamicResponseModel<GetAllVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                            VoucherFilter voucherFilter,
                                                                            decimal lon,
                                                                            decimal lat,
                                                                            List<Guid>? categoryID)
        {
            (int, IQueryable<GetAllVoucherDTO>) result;
            try
            {
                decimal R = 6371; // Earth's radius in kilometers
                result.Item2 = _voucherRepository.GetTable(includeProperties: x => x/*.Include(x => x.Medias)*/
                                                                                    .Include(x => x.Supplier)
                                                                                    .Include(x => x.Categories)
                                                                                    .Include(x => x.Brand)
                                                                                        .ThenInclude(x => x.Addresses))
                    .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider)
                    .DynamicFilter(_mapper.Map<GetAllVoucherDTO>(voucherFilter));

                if (categoryID != null && categoryID.Count > 0)
                {
                    // Filter vouchers by the provided categoryID list
                    result.Item2 = result.Item2
                        .Where(v => v.categories.Any(c => categoryID.Contains((Guid)c.id)));
                }

                IList<GetAllVoucherDTO?> vouchers = new List<GetAllVoucherDTO>();

                if (lon != 0 && lat != 0)
                {
                    decimal ToRadians(decimal value) => (decimal)(Math.PI / 180) * value;

                    vouchers = result.Item2.ToList()
                        .Select(voucher =>
                        {
                            // Create intermediate variables for logging
                            var voucherId = voucher.id;
                            var addresses = voucher.addresses;

                            if (addresses == null || !addresses.Any())
                            {
                                LoggerService.Logger($"Voucher {voucherId} has no addresses.");
                                return null; // Skip this voucher if no addresses are available
                            }

                            var nearestAddress = addresses
                                .Where(a => a.lat.HasValue && a.lon.HasValue)
                                .Select(a =>
                                {
                                    decimal? addressLat = a.lat;
                                    decimal? addressLon = a.lon;

                                    if (!addressLat.HasValue || !addressLon.HasValue)
                                    {
                                        LoggerService.Logger($"Address {a.id} has invalid latitude or longitude.");
                                        return null;
                                    }

                                    decimal dLat = ToRadians(lat - (decimal)addressLat);
                                    decimal dLon = ToRadians(lon - (decimal)addressLon);
                                    decimal lat1 = ToRadians(lat);
                                    decimal lat2 = ToRadians((decimal)addressLat);

                                    decimal aVal = (decimal)(Math.Sin((double)dLat / 2) * Math.Sin((double)dLat / 2) +
                                                             Math.Cos((double)lat1) * Math.Cos((double)lat2) *
                                                             Math.Sin((double)dLon / 2) * Math.Sin((double)dLon / 2));

                                    decimal c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)aVal), Math.Sqrt(1 - (double)aVal));

                                    decimal distance = R * c;

                                    LoggerService.Logger($"Address {a.id} has distance {distance} km from the user.");

                                    return new { Address = a, Distance = distance };
                                })
                                .Where(a => a != null) // Filter out null addresses
                                .OrderBy(a => a.Distance)  // Sort by the closest distance
                                .Take(5)  // Take the 5 nearest addresses
                                .ToList();

                            if (nearestAddress.Any())
                            {
                                LoggerService.Logger($"Voucher {voucher.id} has {nearestAddress.Count} nearest addresses.");

                                // Create the DTO object
                                var x = new GetAllVoucherDTO
                                {
                                    categories = voucher.categories,
                                    id = voucher.id,
                                    medias = voucher.medias,
                                    image = voucher.image = voucher.medias.Count != 0 ? voucher.medias.FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT)?.url : null,
                                    originalPrice = voucher.originalPrice,
                                    sellPrice = voucher.sellPrice,
                                    salePrice = voucher.salePrice,
                                    title = voucher.title,
                                    brandId = voucher.id,
                                    brandName = voucher.brandName,
                                    brandImage = voucher.brandImage,
                                    percentDiscount = voucher.percentDiscount,
                                    supplierId = voucher.supplierId,
                                    supplierImage = voucher.supplierImage,
                                    supplierName = voucher.supplierName,
                                    quantity = voucher.quantity,
                                    rating = voucher.rating,
                                    addresses = nearestAddress
                                        .Select(na => new GetAllAddressDTO
                                        {
                                            id = na.Address.id,
                                            name = na.Address.name,
                                            lat = na.Address.lat,
                                            lon = na.Address.lon,
                                            distance = Math.Round(na.Distance, 2) // Round distance to 2 decimal places
                                        })
                                        .ToList() // Map to address DTOs and create a list of nearest 5 addresses
                                };

                                return x;
                            }
                            else
                            {
                                // Log when no nearest addresses are found
                                LoggerService.Logger($"Voucher {voucher.id} has no nearby addresses.");
                            }

                            return null;

                        })
                        .Where(v => v != null) // Filter out vouchers without nearby addresses
                        .Take(10) // Limit to the 10 nearest vouchers
                        .ToList();
                }

                var vouchersList = vouchers.Count != 0 ? vouchers : result.Item2.ToList();  // Materialize the query into a list

                // Check for available promotions
                foreach (var voucher in vouchersList)
                {
                    var currentDate = DateTime.Now;
                    LoggerService.Logger($"Checking promotions for voucher {voucher.id}.");

                    var promotions = await _voucherRepository.GetByIdAsync(voucher.id, includeProperties: query => query.Include(x => x.Promotions));

                    if (promotions == null || promotions.Promotions == null)
                    {
                        LoggerService.Logger($"Voucher {voucher.id} has no promotions.");
                        continue;
                    }

                    var availablePromotion = promotions.Promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                    if (availablePromotion != null)
                    {
                        voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.PercentDiscount / 100);
                        voucher.percentDiscount = availablePromotion.PercentDiscount;
                        voucher.image = voucher.medias.Count != 0 ? voucher.medias.FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT).url : null;
                        LoggerService.Logger($"Voucher {voucher.id} has an active promotion with {availablePromotion.PercentDiscount}% discount.");
                    }
                }

                // Apply pagination
                var pagedVouchers = vouchersList
                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                    .Take(pagingRequest.pageSize)
                    .ToList();

                return new DynamicResponseModel<GetAllVoucherDTO>()
                {
                    metaData = new MetaData()
                    {
                        page = pagingRequest.page,
                        size = pagingRequest.pageSize,
                        total = vouchersList.Count() // Total vouchers count for metadata
                    },
                    results = pagedVouchers // Return the paged voucher list with nearest address and distance
                };
            }
            catch (Exception ex)
            {
                LoggerService.Logger($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw new LoadException("An unexpected error occurred while loading vouchers.");
            }

            return null;
        }


        public async Task<IList<GetNewestVoucherDTO>> GetSalestVouchers()
        {
            // check all active promotion
            DateTime currenDate = DateTime.Now;
            List<GetNewestVoucherDTO> vouchers = new();

            List<Promotion> promotions = _promotionRepository.GetTable()
                                                                .Include(x => x.Vouchers)
                                                                    .ThenInclude(v => v.Categories) // Include categories of the vouchers
                                                                .Include(x => x.Vouchers)
                                                                    .ThenInclude(v => v.Brand) // Include the brand of the vouchers
                                                                //.Include(x => x.Vouchers)
                                                                //    .ThenInclude(x => x.Medias)
                                                                .Include(x => x.Vouchers)
                                                                    .ThenInclude(x => x.Supplier)
                                                                .ToList()
                                                                .Where(x => x.StartDate <= currenDate && currenDate <= x.EndDate)
                                                                .ToList();
            if (promotions.Count != 0)
            {
                foreach (var promotion in promotions)
                {
                    if (promotion.Vouchers.Count != 0)
                    {
                        foreach (var voucher in promotion.Vouchers)
                        {
                            var existedVoucher = _mapper.Map<GetNewestVoucherDTO>(voucher);
                            existedVoucher.salePrice = existedVoucher.originalPrice - (existedVoucher.originalPrice * promotion.PercentDiscount / 100);
                            existedVoucher.percentDiscount = promotion.PercentDiscount;
                            //existedVoucher.image = voucher.Medias.Count != 0 ? voucher.Medias.FirstOrDefault(x => x.Type == MediaEnum.ADVERTISEMENT.ToString()).Url : null;
                            //vouchers.Add(existedVoucher);
                        }
                    }
                }
                return vouchers;
            }

            return null;
        }
    }
}
