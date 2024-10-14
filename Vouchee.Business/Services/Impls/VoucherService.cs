using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

                voucher.Status = VoucherStatusEnum.ACTIVE.ToString();
                voucher.CreateBy = Guid.Parse(thisUserObj.userId);


                var voucherId = await _voucherRepository.AddAsync(voucher);

                if (createVoucherDTO.image != null && voucherId != null)
                {
                    voucher.Image = await _fileUploadService.UploadImageToFirebase(createVoucherDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.VOUCHER);

                    await _voucherRepository.UpdateAsync(voucher);
                }

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

        public Task<IList<GetAllVoucherDTO>> GetBestSoldVouchers()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<GetNearestVoucherDTO>> GetNearestVouchers(decimal lon, decimal lat)
        {
            decimal maxDistanceKm = 2;
            decimal R = 6371; // Bán kính Trái Đất (km)

            // Chuyển độ sang radian
            decimal ToRadians(decimal value) => (decimal)(Math.PI / 180) * value;

            // Lấy tất cả vouchers và ánh xạ sang DTO
            var vouchers = _voucherRepository.GetTable()
                .ProjectTo<GetNearestVoucherDTO>(_mapper.ConfigurationProvider)
                .ToList(); // Materialize dữ liệu

            // Tính toán và lọc các voucher với địa chỉ gần nhất
            return vouchers
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
                            a.distance = Math.Round(distance, 2) + "km"; // Làm tròn 2 chữ số

                            return new { Address = a, Distance = distance };
                        })
                        .Where(a => a.Distance <= maxDistanceKm)
                        .OrderBy(a => a.Distance)
                        .Select(a => a.Address)
                        .FirstOrDefault();

                    if (nearestAddress != null)
                    {
                        return new GetNearestVoucherDTO
                        {
                            title = voucher.title,
                            categories = voucher.categories,
                            image = voucher.image,
                            originalPrice = voucher.originalPrice,
                            salePrice = voucher.salePrice,
                            id = voucher.id,
                            addresses = new List<GetAllAddressDTO> { nearestAddress }
                        };
                    }
                    return null;
                })
                .Where(v => v != null) // Loại bỏ các voucher không có địa chỉ gần
                .Take(10) // Lấy 10 voucher gần nhất
                .ToList();
        }


        public Task<IList<GetAllVoucherDTO>> GetNearestVouchers(PagingRequest pagingRequest, decimal lon, decimal lat)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<GetAllVoucherDTO>> GetNewestVouchers()
        {
            IQueryable<GetAllVoucherDTO> result;
            try
            {
                result = _voucherRepository.GetTable()
                                            .OrderByDescending(x => x.CreateDate)  
                                            .Take(8)
                                            .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải voucher");
            }
            return result.ToList();
        }

        public Task<IList<GetAllVoucherDTO>> GetTopSaleVouchers()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<GetBestBuyVoucherDTO>> GetTopSaleVouchers(PagingRequest pagingRequest)
        {
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
            var vouchers = _voucherRepository.GetTable()
                .Where(v => orderDetails.Select(od => od.VoucherId).Contains(v.Id))
                .ProjectTo<GetBestBuyVoucherDTO>(_mapper.ConfigurationProvider)
                .ToList();

            // Attach the total quantity sold to each voucher and sort the result
            var result = vouchers.Select(voucher => new GetBestBuyVoucherDTO
            {
                id = voucher.id,
                title = voucher.title,
                image = voucher.image,
                originalPrice = voucher.originalPrice,
                salePrice = voucher.salePrice,
                TotalQuantitySold = orderDetails.First(od => od.VoucherId == voucher.id).TotalQuantitySold,
            })
            .OrderByDescending(v => v.TotalQuantitySold)
            .ToList();

            return result;
        }



        public async Task<GetDetailVoucherDTO> GetVoucherByIdAsync(Guid id)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(id,
                                        query => query.Include(x => x.VoucherCodes)
                                                        .Include(x => x.Brand)
                                                        .Include(x => x.Supplier)
                                                        .Include(x => x.Addresses));
                if (voucher != null)
                {
                    GetDetailVoucherDTO voucherDTO = _mapper.Map<GetDetailVoucherDTO>(voucher);
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
                                                                                        VoucherFilter voucherFiler, 
                                                                                        decimal lon, 
                                                                                        decimal lat, 
                                                                                        decimal maxDistance,
                                                                                        List<Guid>? categoryID)
        {
            (int, IQueryable<GetAllVoucherDTO>) result;
            try
            {
                decimal R = 6371; // Earth's radius in kilometers
                result = _voucherRepository.GetTable()
                            .ProjectTo<GetAllVoucherDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetAllVoucherDTO>(voucherFiler))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

                if (categoryID != null && categoryID.Count > 0)
                {
                    // Filter vouchers by the provided categoryID list
                    result.Item2 = result.Item2
                        .Where(v => v.categories.Any(c => categoryID.Contains((Guid)c.id)));
                }

                IList<GetAllVoucherDTO> vouchers = new List<GetAllVoucherDTO>();

                if (lon != 0 && lat != 0 && maxDistance != 0)
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
                                                    a.distance = Math.Round(distance, 2) + "km"; // Round to 2 decimal places

                                                    return new { Address = a, Distance = distance };
                                                })
                                                .Where(a => a.Distance <= maxDistance)
                                                .OrderBy(a => a.Distance)
                                                .Select(a => a.Address)
                                                .FirstOrDefault();

                                            if (nearestAddress != null)
                                            {
                                                return new GetAllVoucherDTO
                                                {
                                                    categories = voucher.categories,
                                                    id = voucher.id,
                                                    image = voucher.image,
                                                    originalPrice = voucher.originalPrice,
                                                    salePrice = voucher.salePrice,
                                                    title = voucher.title,
                                                    addresses = new List<GetAllAddressDTO> { nearestAddress }
                                                };
                                            }
                                            return null;
                                        })
                                        .Where(v => v != null) // Filter out vouchers without nearby addresses
                                        .Take(10) // Limit to the 10 nearest vouchers
                                        .ToList();
                }

                var vouchersList = vouchers.Count != 0 ? vouchers.ToList() : result.Item2.ToList();  // Materialize the query into a list

                if (result.Item2.Count() != 0)
                {
                    // check available promotion
                    foreach (var voucher in vouchersList)
                    {
                        var currentDate = DateTime.Now;
                        var promotions = _voucherRepository.GetByIdAsync(voucher.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

                        var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                        if (availablePromotion != null)
                        {
                            voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.PercentDiscount / 100);
                            voucher.percentDiscount = availablePromotion.PercentDiscount;
                        }
                    }
                }
                else if (vouchers.Count() != 0)
                {
                    // check available promotion
                    foreach (var voucher in vouchersList)
                    {
                        var currentDate = DateTime.Now;
                        var promotions = _voucherRepository.GetByIdAsync(voucher.id, includeProperties: query => query.Include(x => x.Promotions)).Result.Promotions;

                        var availablePromotion = promotions.FirstOrDefault(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate);

                        if (availablePromotion != null)
                        {
                            voucher.salePrice = voucher.originalPrice - (voucher.originalPrice * availablePromotion.PercentDiscount / 100);
                        }
                        else
                        {
                            voucher.salePrice = 0;
                        }
                    }
                }


                return new DynamicResponseModel<GetAllVoucherDTO>()
                {
                    metaData = new MetaData()
                    {
                        page = pagingRequest.page,
                        size = pagingRequest.pageSize,
                        total = vouchersList.Count()
                    },
                    results = vouchersList // Return only the filtered voucher list
                };
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("An unexpected error occurred while loading vouchers");
            }

            return null;
        }

        public async Task<IList<VoucherDTO>> GetSalestVouchers(PagingRequest pagingRequest)
        {
            throw new NotImplementedException();
        }

        public class VoucherDTO
        {
            public Guid id { get; set; }

            public string? title { get; set; }
            public decimal originalPrice { get; set; }
            public decimal discountValue { get; set; }
            public decimal salePrice { get; set; }
            public int? quantity { get; set; }
            public string? image { get; set; }
        }
    }
}
