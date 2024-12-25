using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IPartnerTransactionService
    {
        public Task<dynamic> CreatePartnerTransactionAsync(CreateSePayPartnerInTransactionDTO createWalletTransactionDTO);
        public Task<DynamicResponseModel<GetPartnerTransactionDTO>> GetPartnerTransactionAsync(PagingRequest pagingRequest, PartnerTransactionFilter partnerTransactionFilter);
    }
}
