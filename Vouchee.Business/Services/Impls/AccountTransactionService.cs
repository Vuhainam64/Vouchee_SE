using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class AccountTransactionService : IAccountTransactionService
    {
        private readonly IBaseRepository<AccountTransaction> _accountTransactionRepository;
        private readonly IMapper _mapper;

        public AccountTransactionService(IBaseRepository<AccountTransaction> accountTransactionRepository,
                                            IMapper mapper)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateTopUpRequestAsync(ThisUserObj currentUser, int amount)
        {
            try
            {
                AccountTransaction accountTransaction = new()
                {
                    CreateBy = currentUser.userId,
                    CreateDate = DateTime.Now,
                    Status = AccountTransactionStatusEnum.PENDING.ToString(),
                    FromUserId = currentUser.userId,
                    Type = AccountTransactionTypeEnum.TOP_UP.ToString(),
                    Amount = amount
                };

                var result = await _accountTransactionRepository.AddAsync(accountTransaction);
                if (result != Guid.Empty)
                {
                    return new ResponseMessage<Guid>()
                    {
                        message = "Tạo request thành công",
                        result = true,
                        value = (Guid)result
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetAccountTransactionDTO> GetAccountTransactionById(Guid id)
        {
            try
            {
                var existedAccountTransaction = await _accountTransactionRepository.GetByIdAsync(id);
                if (existedAccountTransaction == null)
                {
                    throw new NotFoundException("Không tìm thấy account transaction này");
                }

                return _mapper.Map<GetAccountTransactionDTO>(existedAccountTransaction);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
