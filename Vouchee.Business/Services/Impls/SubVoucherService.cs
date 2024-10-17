using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class SubVoucherService : ISubVoucherService
    {
        private readonly ISubVoucherRepository _subVoucherRepository;
        private readonly IMapper _mapper;
        public SubVoucherService(ISubVoucherRepository subVoucherRepository, 
                                    IMapper mapper)
        {
            _subVoucherRepository = subVoucherRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateSubVoucherAsync(CreateSubVoucherDTO createSubVoucherDTO, ThisUserObj thisUserObj)
        {
            SubVoucher subVoucher = _mapper.Map<SubVoucher>(createSubVoucherDTO);
            return await _subVoucherRepository.AddAsync(subVoucher);
        }

        public async Task<bool> DeleteSubVoucherAsync(Guid id)
        {
            SubVoucher subVoucher = await _subVoucherRepository.FindAsync(id);
            if (subVoucher != null)
            {
                return await _subVoucherRepository.DeleteAsync(subVoucher);
            }
            return false;
        }

        public async Task<GetSubVoucherDTO> GetSubVoucherByIdAsync(Guid id)
        {
            SubVoucher subVoucher = await _subVoucherRepository.GetByIdAsync(id);
            if (subVoucher != null)
            {
                return _mapper.Map<GetSubVoucherDTO>(subVoucher);
            }
            return null;
        }

        public Task<DynamicResponseModel<GetSubVoucherDTO>> GetSubVouchersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSubVoucherAsync(Guid id, UpdateSubVoucherDTO updateSubVoucherDTO)
        {
            throw new NotImplementedException();
        }
    }
}
