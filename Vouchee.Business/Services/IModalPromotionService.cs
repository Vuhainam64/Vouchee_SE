using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IModalPromotionService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateModalPromotionAsync(CreateModalPromotionDTO createModalPromotionDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetDetailModalPromotionDTO> GetModalPromotionById(Guid id);
        public Task<IList<GetDetailModalPromotionDTO>> GetModalPromotionBySeller(Guid sellerId);

        // UPDATE
        
        // DELETE
    }
}
