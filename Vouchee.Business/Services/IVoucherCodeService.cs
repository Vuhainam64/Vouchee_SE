using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Vouchee.Business.Services
{
    public interface IVoucherCodeService
    {
        // CREATE
        public Task<Guid?> CreateVoucherCodeAsync(Guid modalId, [FromBody] CreateVoucherCodeDTO createVoucherCodeDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetVoucherCodeDTO> GetVoucherCodeByIdAsync(Guid id);
        public Task<IList<GetVoucherCodeDTO>> GetVoucherCodesAsync();

        // UPDATE
        public Task<bool> UpdateVoucherCodeAsync(Guid id, UpdateVoucherCodeDTO updateVoucherCodeDTO);

        // DELETE
        public Task<bool> DeleteVoucherCodeAsync(Guid id);
    }
}
