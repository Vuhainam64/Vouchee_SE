using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class ModalPromotionService : IModalPromotionService
    {
        //private readonly IBaseRepository<Modal> _modalRepository;
        //private readonly IBaseRepository<Promotion> _promotionRepository;
        //private readonly IMapper _mapper;

        //public ModalPromotionService(IBaseRepository<Modal> modalRepository,
        //                                IBaseRepository<Promotion> promotionRepository, 
        //                                IMapper mapper)
        //{
        //    _modalRepository = modalRepository;
        //    _promotionRepository = promotionRepository;
        //    _mapper = mapper;
        //}

        //public async Task<ResponseMessage<Guid>> CreateModalPromotionAsync(CreateModalPromotionDTO createModalPromotionDTO, ThisUserObj thisUserObj)
        //{
        //    try
        //    {
        //        Promotion modalPromotion = _mapper.Map<Promotion>(createModalPromotionDTO);

        //        foreach (var modalId in createModalPromotionDTO.modal_id)
        //        {
        //            var existedModal = await _modalRepository.GetByIdAsync(modalId, includeProperties: x => x.Include(x => x.Voucher), isTracking: true);
        //            if (existedModal == null)
        //            {
        //                throw new NotFoundException($"Không tìm thấy modal {modalId}");
        //            }
        //            if (thisUserObj.userId != existedModal.Voucher.SellerId)
        //            {
        //                throw new ConflictException($"Modal {existedModal.Id} không phải của bạn");
        //            }

        //            modalPromotion.Modals.Add(existedModal);
        //        }

        //        var modalPromotionId = await _promotionRepository.AddAsync(modalPromotion);

        //        return new ResponseMessage<Guid>()
        //        {
        //            message = "Tạo modal promotion thành công",
        //            result = true,
        //            value = modalPromotionId.Value
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public async Task<GetDetailModalPromotionDTO> GetModalPromotionById(Guid id)
        //{
        //    var existedModal = await _promotionRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Modals));

        //    if (existedModal == null)
        //    {
        //        throw new NotFoundException("Không tìm thấy modal này");
        //    }

        //    return _mapper.Map<GetDetailModalPromotionDTO>(existedModal);
        //}

        //public async Task<IList<GetDetailModalPromotionDTO>> GetModalPromotionBySeller(Guid sellerId)
        //{
        //    IQueryable<GetDetailModalPromotionDTO> result;
        //    try
        //    {
        //        DateTime currentDate = DateTime.Now;

        //        result = _promotionRepository.GetTable()
        //                                          .Where(mp => mp.Modals.Any(modal => modal.Voucher.SellerId == sellerId))
        //                                          .Where(x => x.StartDate <= currentDate && currentDate <= x.EndDate)
        //                                          .ProjectTo<GetDetailModalPromotionDTO>(_mapper.ConfigurationProvider);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new LoadException(ex.Message);
        //    }
        //    return await result.ToListAsync();
        //}
    }
}
