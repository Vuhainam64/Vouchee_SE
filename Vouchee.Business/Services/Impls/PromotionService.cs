using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.DependencyInjection;
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
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class PromotionService : IPromotionService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(IFileUploadService fileUploadService,
                                    IPromotionRepository promotionRepository, 
                                    IMapper mapper)
        {
            _fileUploadService = fileUploadService;
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreatePromotionAsync(CreatePromotionDTO createPromotionDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var promotion = _mapper.Map<Promotion>(createPromotionDTO);

                promotion.CreateBy = Guid.Parse(thisUserObj.userId);

                var promotionId = _promotionRepository.AddAsync(promotion).Result;
                
                if (createPromotionDTO.image != null && promotionId != null)
                {
                    promotion.Image = await _fileUploadService.UploadImageToFirebase(createPromotionDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.PROMOTION);

                    await _promotionRepository.UpdateAsync(promotion);
                }

                return promotionId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception("Lỗi không xác định khi tạo promotion");
            }
        }

        public async Task<bool> DeletePromotionAsync(Guid id, ThisUserObj thisUserObj)
        {
            try
            {
                var result = false;
                var promotion = await _promotionRepository.GetByIdAsync(id);
                if (promotion != null)
                {
                    result = await _promotionRepository.DeleteAsync(promotion);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy promotion với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa promotion");
            }
        }

        public async Task<GetPromotionDTO> GetPromotionByIdAsync(Guid id)
        {
            try
            {
                var promotion = await _promotionRepository.GetByIdAsync(id);
                if (promotion != null)
                {
                    var promotioDTO = _mapper.Map<GetPromotionDTO>(promotion);
                    return promotioDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy promotion với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải promotion");
            }
        }

        public async Task<IList<GetPromotionDTO>> GetPromotionsAsync()
        {
            IQueryable<GetPromotionDTO> result;
            try
            {
                result = _promotionRepository.GetTable()
                            .ProjectTo<GetPromotionDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải promotion");
            }
            return result.ToList();
        }

        public async Task<bool> UpdatePromotionAsync(Guid id, UpdatePromotionDTO updatePromotionDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var existedPromotion = await _promotionRepository.GetByIdAsync(id);
                if (existedPromotion != null)
                {
                    var entity = _mapper.Map<Promotion>(updatePromotionDTO);
                    return await _promotionRepository.UpdateAsync(entity);
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
