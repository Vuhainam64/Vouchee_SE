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
using Vouchee.Data.Migrations;
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
        private readonly IPromotionRepository _promotionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public VoucherService(IPromotionRepository promotionRepository,
                                ICategoryRepository categoryRepository,
                                IFileUploadService fileUploadService,
                                IVoucherRepository voucherRepository,
                                IOrderDetailRepository orderDetailRepository,
                                IMapper mapper)
        {
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

                voucher.Status = VoucherStatusEnum.SELLING.ToString();
                voucher.CreateBy = Guid.Parse(thisUserObj.userId);


                var voucherId = await _voucherRepository.AddAsync(voucher);

                //if (createVoucherDTO.image != null && voucherId != null)
                //{
                //    voucher.Image = await _fileUploadService.UploadImageToFirebase(createVoucherDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.VOUCHER);

                //    await _voucherRepository.UpdateAsync(voucher);
                //}

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
                            .FirstOrDefault(); // Get the nearest address

                        if (nearestAddress != null)
                        {
                            return new GetNearestVoucherDTO
                            {
                                id = voucher.id,
                                title = voucher.title,
                                categories = voucher.categories,
                                image = voucher.images.FirstOrDefault().imageUrl,
                                originalPrice = voucher.originalPrice,
                                salePrice = voucher.salePrice,
                                addresses = new List<GetAllAddressDTO>
                                {
                                    new GetAllAddressDTO
                                    {
                                        id = nearestAddress.Address.id,
                                        addressName = nearestAddress.Address.addressName,
                                        lat = nearestAddress.Address.lat,
                                        lon = nearestAddress.Address.lon,
                                        distance = nearestAddress.Distance // Include distance here at the address level
                                    }
                                }
                            };
                        }
                        return null;
                    })
                    .Where(v => v != null) // Filter out vouchers without a nearest address
                    .Take(8) // Limit to the 8 nearest vouchers
                    .ToList();

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
                                            .Include(x => x.Images)
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

                        voucher.image = voucher.images.FirstOrDefault().imageUrl;
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
                .Include(x => x.Categories)
                .Include(x => x.Brand)
                .Where(v => orderDetails.Select(od => od.VoucherId).Contains(v.Id))
                .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider);

            // Attach the total quantity sold to each voucher and sort the result
            var result = voucher.ToList().Select(voucher => new GetBestBuyVoucherDTO
            {
                id = voucher.id,
                title = voucher.title,
                image = voucher.images.FirstOrDefault().imageUrl,
                originalPrice = voucher.originalPrice,
                salePrice = voucher.salePrice,
                TotalQuantitySold = orderDetails.First(od => od.VoucherId == voucher.id).TotalQuantitySold,
                categories = voucher.categories,
                brandId = voucher.brandId,
                brandImage = voucher.brandImage,
                brandName = voucher.brandName
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
                }
                else
                {
                    test.salePrice = 0;
                }
            }

            return result2;
        }



        public async Task<GetDetailVoucherDTO> GetVoucherByIdAsync(Guid id)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(id,
                                        query => query.Include(x => x.VoucherCodes)
                                                        .Include(x => x.Brand)
                                                        .Include(x => x.Supplier)
                                                        .Include(x => x.Addresses)
                                                        .Include(x => x.Categories)
                                                        .Include(x => x.Images)
                                                        .Include(x => x.VoucherType)
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
                        voucherDTO.percenDiscount = availablePromotion.PercentDiscount;
                        voucherDTO.image = voucherDTO.images.FirstOrDefault().imageUrl;
                    }

                    return voucherDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy voucher với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
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
                result.Item2 = _voucherRepository.GetTable()
                    .Include(x => x.Images)
                    .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider)
                    .DynamicFilter(_mapper.Map<GetAllVoucherDTO>(voucherFilter));

                if (categoryID != null && categoryID.Count > 0)
                {
                    // Filter vouchers by the provided categoryID list
                    result.Item2 = result.Item2
                        .Where(v => v.categories.Any(c => categoryID.Contains((Guid)c.id)));
                }

                IList<GetAllVoucherDTO> vouchers = new List<GetAllVoucherDTO>();

                if (lon != 0 && lat != 0)
                {
                    decimal ToRadians(decimal value) => (decimal)(Math.PI / 180) * value;

                    vouchers = result.Item2.ToList()
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

                                    decimal distance = R * c;

                                    return new { Address = a, Distance = distance };
                                })
                                .OrderBy(a => a.Distance)  // Sort by the closest distance
                                .FirstOrDefault(); // Get the nearest address and its distance

                            if (nearestAddress != null)
                            {
                                return new GetAllVoucherDTO
                                {
                                    categories = voucher.categories,
                                    id = voucher.id,
                                    images = voucher.images,
                                    image = voucher.images.FirstOrDefault().imageUrl,
                                    originalPrice = voucher.originalPrice,
                                    salePrice = voucher.salePrice,
                                    title = voucher.title,
                                    brandId = voucher.id,
                                    brandName = voucher.brandName,
                                    brandImage = voucher.brandImage,
                                    addresses = new List<GetAllAddressDTO>
                                    {
                                        new GetAllAddressDTO
                                        {
                                            id = nearestAddress.Address.id,
                                            addressName = nearestAddress.Address.addressName,
                                            lat = nearestAddress.Address.lat,
                                            lon = nearestAddress.Address.lon,
                                            distance = Math.Round(nearestAddress.Distance, 2) // Round distance to 2 decimal places
                                        }
                                    }
                                };
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
                    var promotions = await _voucherRepository.GetByIdAsync(voucher.id, includeProperties: query => query.Include(x => x.Promotions));
                    var availablePromotion = promotions.Promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                    if (availablePromotion != null)
                    {
                        voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.PercentDiscount / 100);
                        voucher.percentDiscount = availablePromotion.PercentDiscount;
                        voucher.image = voucher.images.FirstOrDefault().imageUrl;
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
                LoggerService.Logger(ex.Message);
                throw new LoadException("An unexpected error occurred while loading vouchers");
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
                                                                .Include(x => x.Vouchers)
                                                                    .ThenInclude(x => x.Images)
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
                            existedVoucher.image = existedVoucher.images.FirstOrDefault().imageUrl;
                            vouchers.Add(existedVoucher);
                        }
                    }
                }
                return vouchers;
            }

            return null;
        }
    }
}
