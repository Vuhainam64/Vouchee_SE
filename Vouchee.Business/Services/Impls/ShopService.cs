using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.Constants.String;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IMapper _mapper;

        public ShopService(IShopRepository shopRepository,
                            IMapper mapper)
        {
            _shopRepository = shopRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateShopAsync(CreateShopDTO createShopDTO)
        {
            try
            {
                Shop shop = _mapper.Map<Shop>(createShopDTO);

                shop.Status = ShopStatusEnum.ACTIVE.ToString();

                var shopId = await _shopRepository.AddAsync(shop);
                return shopId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo shop");
            }
        }

        public async Task<bool> DeleteShopAsync(Guid id)
        {
            try
            {
                bool result = false;
                var shop = await _shopRepository.GetByIdAsync(id);
                if (shop != null)
                {
                    result = await _shopRepository.DeleteAsync(shop);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy shop với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa shop");
            }
        }

        public async Task<GetShopDTO> GetShopByIdAsync(Guid id)
        {
            try
            {
                var shop = await _shopRepository.GetByIdAsync(id);
                if (shop != null)
                {
                    GetShopDTO ShopDTO = _mapper.Map<GetShopDTO>(shop);
                    return ShopDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy shop với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải shop");
            }
        }

        public async Task<DynamicResponseModel<GetShopDTO>> GetShopsAsync(PagingRequest pagingRequest,
                                                                            ShopFilter shopFilter,
                                                                            SortShopEnum sortShopEnum)
        {
            int total;
            (int, IQueryable<GetShopDTO>) result;
            try
            {
                result = _shopRepository.GetTable()
                            .ProjectTo<GetShopDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetShopDTO>(shopFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LimitPaging, PageConstant.DefaultPaging);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải shop");
            }
            return new DynamicResponseModel<GetShopDTO>()
            {
                Metadata = new PagingMetadata()
                {
                    Page = pagingRequest.page,
                    Size = pagingRequest.pageSize,
                    Total = result.Item1
                },
                Results = result.Item2.ToList()
            };
        }

        public async Task<bool> UpdateShopAsync(Guid id, UpdateShopDTO updateShopDTO)
        {
            try
            {
                var existedShop = await _shopRepository.GetByIdAsync(id);
                if (existedShop != null)
                {
                    var entity = _mapper.Map<Shop>(updateShopDTO);
                    return await _shopRepository.UpdateAsync(entity);
                }
                else
                {
                    throw new NotFoundException("Không tìm thấy Shop");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật Shop");
            }
        }
    }
}
