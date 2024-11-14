using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class ModalPromotionService : IModalPromotionService
    {
        private readonly IBaseRepository<ModalPromotion> _modalPromotionRepository;
        private readonly IMapper _mapper;

        public ModalPromotionService(IBaseRepository<ModalPromotion> modalPromotionRepository, IMapper mapper)
        {
            _modalPromotionRepository = modalPromotionRepository;
            _mapper = mapper;
        }
    }
}
