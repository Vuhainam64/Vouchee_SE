using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface ITopUpRequestService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateTopUpRequest(CreateTopUpRequestDTO createTopUpRequestDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetTopUpRequestDTO> GetTopUpRequestById(Guid id);
    }
}
