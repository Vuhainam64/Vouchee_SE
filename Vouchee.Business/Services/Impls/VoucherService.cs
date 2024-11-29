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
        private readonly IBaseRepository<Order> _orderRepository;
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

        public VoucherService(IBaseRepository<Order> orderRepository,
                              IBaseRepository<User> userRepository,
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
            _orderRepository = orderRepository;
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

        public async Task<ResponseMessage<bool>> DeleteVoucherAsync(Guid id)
        {
            var exkstedVoucher = await _voucherRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Modals)
                                                                                                        .ThenInclude(x => x.OrderDetails)
                                                                                                     .Include(x => x.Modals)
                                                                                                        .ThenInclude(x => x.VoucherCodes)
                                                                                                    .Include(x => x.Modals)
                                                                                                        .ThenInclude(x => x.Ratings)
                                                                                                    .Include(x => x.Modals)
                                                                                                        .ThenInclude(x => x.Carts)
                                                                                                    .Include(x => x.Medias), isTracking: true);

            if (exkstedVoucher == null)
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }

            foreach (var modal in exkstedVoucher.Modals.ToList()) 
            {
                if (modal.OrderDetails.Any())
                {
                    throw new ConflictException("Không thể xóa voucher này vì đã được order");
                }

                foreach (var voucherCode in modal.VoucherCodes.ToList()) 
                {
                    modal.VoucherCodes.Remove(voucherCode);
                }

                foreach (var cart in modal.Carts.ToList()) 
                {
                    modal.Carts.Remove(cart);
                }

                exkstedVoucher.Modals.Remove(modal);
            }

            foreach (var media in exkstedVoucher.Medias.ToList()) 
            {
                exkstedVoucher.Medias.Remove(media);
            }

            await _voucherRepository.DeleteAsync(exkstedVoucher);

            return new ResponseMessage<bool>()
            {
                message = "Xóa voucher thành công",
                result = true,
                value = true
            };
        }

        public async Task<IList<GetVoucherDTO>> GetNewestVouchers(int numberOfVoucher)
        {
            var result = _voucherRepository.GetTable()
                                .Include(x => x.Seller.ShopPromotions)
                                .OrderByDescending(v => v.CreateDate)
                                .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                                .Where(x => x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true)
                                .Take(numberOfVoucher != 0 ? numberOfVoucher : 10)
                                .ToList()
                                .Where(x => x.stock > 0)
                                .ToList();

            return result;
        }

        public async Task<IList<GetVoucherDTO>> GetTopSaleVouchers(int numberOfVoucher)
        {
            IList<GetVoucherDTO> result;

            result = _voucherRepository.GetTable()
                        .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                        .Where(x => x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true)
                        .Take(numberOfVoucher != 0 ? numberOfVoucher : 10)
                        .ToList()
                        .Where(x => x.stock > 0)
                        .OrderByDescending(x => x.totalQuantitySold)
                        .ToList();

            return result;
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
                                                                    .ThenInclude(x => x.ShopPromotions)
                                                                .Include(x => x.Modals)
                                                                    .ThenInclude(x => x.VoucherCodes.Where(x => x.OrderId == null))
                                                                .Include(x => x.Medias));

            if (existedVoucher != null)
            {
                var voucher = _mapper.Map<GetDetailVoucherDTO>(existedVoucher);

                return new
                {
                    results = voucher
                };
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy voucher với id {id}");
            }
        }

        public async Task<ResponseMessage<bool>> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO, ThisUserObj thisUserObj)
        {
            List<Guid> imageId = [];

            var existedVoucher = await _voucherRepository.FindAsync(id, isTracking: true);
            
            if (existedVoucher == null)
            {
                throw new NotFoundException("Không tìm thấy voucher");
            }

            var existedBrand = await _brandReposiroty.FindAsync(updateVoucherDTO.brandId);
            if (existedBrand == null)
            {
                throw new NotFoundException($"Không tìm thấy brand với id {updateVoucherDTO.brandId}");
            }

            var existedSupplier = await _supplierRepository.FindAsync((updateVoucherDTO.supplierId));
            if (existedSupplier == null)
            {
                throw new NotFoundException($"Không tìm thấy supplier với id {updateVoucherDTO.supplierId}");
            }

            existedVoucher = _mapper.Map(updateVoucherDTO, existedVoucher);

            existedVoucher.Categories.Clear();

            foreach (var categoryId in updateVoucherDTO.categoryId)
            {
                var existedCategory = await _categoryRepository.FindAsync(categoryId, true);
                if (existedCategory == null)
                {
                    throw new NotFoundException($"Không thấy category với id {categoryId}");
                }
                existedVoucher.Categories.Add(existedCategory);
            }

            foreach (var media in existedVoucher.Medias.ToList())
            {
                await _mediaRepository.DeleteAsync(media);
            }

            if (updateVoucherDTO.images?.Count != 0)
            {
                int index = 0;
                foreach (var image in updateVoucherDTO.images)
                {
                    Media media = new()
                    {
                        Url = image,
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Index = index++,
                    };
                    existedVoucher.Medias.Add(media);
                }
            }

            await _voucherRepository.UpdateAsync(existedVoucher);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật dữ liệu thành công",
                result = true,
                value = true
            };
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
            var result = _voucherRepository.GetTable()
                        .ProjectTo<GetNearestVoucherDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetNearestVoucherDTO>(voucherFilter))
                        .Where(x => categoryIds == null || !categoryIds.Any() || x.categories.Any(c => categoryIds.Contains(c.id.Value)))
                        .Where(x => x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true)
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

            var pagedVouchers = result
                    .Where(x => x.stock > 0)
                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                    .Take(pagingRequest.pageSize)
                    .ToList();

            return new DynamicResponseModel<GetNearestVoucherDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Count() // Total vouchers count for metadata
                },
                results = pagedVouchers
            };
        }

        public async Task<IList<GetVoucherDTO>> GetSalestVouchers(int numberOfVoucher)
        {
            var result = _voucherRepository.GetTable()
                    .OrderByDescending(v => v.CreateDate)
                    .ProjectTo<GetVoucherDTO>(_mapper.ConfigurationProvider)
                    .Where(x => x.status == VoucherStatusEnum.NONE.ToString() && x.isActive == true && x.shopDiscount != null)
                    .OrderByDescending(x => x.shopDiscount)
                    .ToList()
                    .Where(x => x.stock > 0)
                    .ToList();

            return result;
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

            result = _voucherRepository.GetTable().Where(x => (categoryIds == null || !categoryIds.Any() || x.Categories.Any(c => categoryIds.Contains(c.Id)))
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

        public async Task<ResponseMessage<bool>> RemoveCategoryFromVoucherAsync(Guid categoryId, Guid voucherId, ThisUserObj thisUserObj)
        {
            var existedVoucher = await _voucherRepository.GetByIdAsync(voucherId, includeProperties: x => x.Include(x => x.Categories), isTracking: true);

            if (existedVoucher == null)
            {
                throw new NotFoundException("Không tìm thấy voucher này");
            }

            var existedCategory = existedVoucher.Categories.FirstOrDefault(x => x.Id == categoryId);

            if (existedCategory != null)
            {
                throw new ConflictException("Danh mục này đã tồn tại trong voucher");
            }

            existedCategory = await _categoryRepository.GetByIdAsync(categoryId, includeProperties: x => x.Include(x => x.Vouchers), isTracking: true);
            if (existedCategory == null)
            {
                throw new NotFoundException("Không tìm thấy category này");
            }

            existedVoucher.Categories.Add(existedCategory);
            existedVoucher.UpdateDate = DateTime.Now;
            existedVoucher.UpdateBy = thisUserObj.userId;

            await _voucherRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = true
            };
        }

        public async Task<IList<MiniVoucher>> GetMiniVoucherAsync(string title)
        {
            var result = _voucherRepository.GetTable()
                                            .Where(x => x.Title.ToLower().Contains(title.ToLower()))
                                            .Take(10)
                                            .ProjectTo<MiniVoucher>(_mapper.ConfigurationProvider);

            return await result.ToListAsync();
        }
    }
}
