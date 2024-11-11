using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class WalletService : IWalletService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IMapper _mapper;

        public WalletService(IBaseRepository<User> userRepository,
                             IBaseRepository<Wallet> walletRepository,
                             IMapper mapper)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<GetUserDTO>> CreateWalletAsync(ThisUserObj currenUser)
        {
            try
            {
                var existedUser = await _userRepository.GetByIdAsync(currenUser.userId,
                                                                        includeProperties: x => x.Include(x => x.BuyerWallet)
                                                                                                    .Include(x => x.SellerWallet),
                                                                        isTracking: true);

                if (existedUser == null)
                {
                    throw new NotFoundException("Không tìm thấy user này");
                }

                if (existedUser.BuyerWallet == null)
                {
                    existedUser.BuyerWallet = new()
                    {
                        CreateDate = DateTime.Now,
                        Status = ObjectStatusEnum.ACTIVE.ToString()
                    };
                }
                if (existedUser.SellerWallet == null)
                {
                    existedUser.SellerWallet = new()
                    {
                        CreateDate = DateTime.Now,
                        Status = ObjectStatusEnum.ACTIVE.ToString()
                    };
                }

                var result = await _userRepository.SaveChanges();

                if (result)
                {
                    return new ResponseMessage<GetUserDTO>()
                    {
                        message = "Tạo ví thành công",
                        result = true,
                        value = _mapper.Map<GetUserDTO>(existedUser)
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

        public async Task<GetBuyerWallet> GetBuyerWalletByIdAsync(Guid id)
        {
            var existedWallet = await _walletRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.BuyerWalletTransactions));
            if (existedWallet == null)
            {
                throw new NotFoundException("Không tìm thấy ví với id này");
            }
            return _mapper.Map<GetBuyerWallet>(existedWallet);
        }

        public async Task<GetSellerWallet> GetSellerWalletByIdAsync(Guid id)
        {
            var existedWallet = await _walletRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.SellerWalletTransactions));
            if (existedWallet == null)
            {
                throw new NotFoundException("Không tìm thấy ví với id này");
            }
            return _mapper.Map<GetSellerWallet>(existedWallet);
        }
    }
}
