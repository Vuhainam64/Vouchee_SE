using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class PartnerTransactionService : IPartnerTransactionService
    {
        private readonly IBaseRepository<PartnerTransaction> _partnerTransactionRepository;
        private readonly IMapper _mapper;

        public PartnerTransactionService(IBaseRepository<PartnerTransaction> partnerTransactionRepository, 
                                            IMapper mapper)
        {
            _partnerTransactionRepository = partnerTransactionRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreatePartnerTransactionAsync(CreateSePayPartnerInTransactionDTO createPartnerTransaction, ThisUserObj thisUserObj)
        {
            PartnerTransaction partnerTransaction = _mapper.Map<PartnerTransaction>(createPartnerTransaction);
            partnerTransaction.PartnerTransactionId = createPartnerTransaction.id;
            partnerTransaction.PartnerName = "SePAY";
            if (createPartnerTransaction.transferType == PartnerTransactionTypeEnum.@in.ToString())
            {
                partnerTransaction.AmountIn = createPartnerTransaction.transferAmount;
            }

            var result = await _partnerTransactionRepository.AddAsync(partnerTransaction);

            return new ResponseMessage<Guid>()
            {
                message = "Tạo transaction thành công",
                result = true,
                value = (Guid) result
            };
        }
    }
}
