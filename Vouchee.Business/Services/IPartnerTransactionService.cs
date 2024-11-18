using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IPartnerTransactionService
    {
        public Task<dynamic> CreatePartnerTransactionAsync(CreateSePayPartnerInTransactionDTO createWalletTransactionDTO);
    }
}
