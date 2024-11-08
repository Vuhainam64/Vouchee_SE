using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IAccountTransactionService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateTopUpRequestAsync(ThisUserObj currentUser, int amount);

        // READ
        public Task<GetAccountTransactionDTO> GetAccountTransactionById(Guid id);

        // UPDATE

        // DELETE
    }
}
