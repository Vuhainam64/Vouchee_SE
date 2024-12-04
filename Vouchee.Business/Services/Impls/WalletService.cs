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

        public async Task<dynamic> GetBuyerWalletAsync(ThisUserObj currentuser)
        {
            var user = await _userRepository.GetByIdAsync(currentuser.userId, includeProperties: x => x.Include(x => x.BuyerWallet.BuyerWalletTransactions));

            if (user.BuyerWallet == null)
            {
                throw new NotFoundException("Người dùng này chưa có ví buyer");
            }

            return new
            {
                totalBalance = user.BuyerWallet.Balance,
                bankAccount = user.BankAccount,
                bankName = user.BankName
            };
        }

        public async Task<dynamic> GetSellerWalletAsync(ThisUserObj currentUser)
        {
            var user = await _userRepository.GetByIdAsync(currentUser.userId, includeProperties: x => x.Include(x => x.SellerWallet.SellerWalletTransactions));
            
            if (user.SellerWallet == null)
            {
                throw new NotFoundException("Người dùng này chưa có ví seller");
            }

            var totalTransactions = user.SellerWallet.SellerWalletTransactions?.Count() ?? 0;
            var totalBalance = user.SellerWallet.Balance;
            var monthlyTransactions = user.SellerWallet.SellerWalletTransactions
                                        .GroupBy(t => new { Year = t.CreateDate?.Year, Month = t.CreateDate?.Month })
                                        .Select(g => new
                                        {
                                            Year = g.Key.Year,
                                            Month = g.Key.Month,
                                            TotalAmount = g.Sum(t => t.Amount), // Assuming Amount is a property in your transaction DTO
                                            TransactionCount = g.Count()
                                        })
                                        .OrderBy(g => g.Year).ThenBy(g => g.Month) // Order by Year and Month
                                        .ToList();

            return new
            {
                totalBalance = totalBalance,
                totalTransactions = totalTransactions,
                monthlyTransactions = monthlyTransactions
            };
        }
    }
}
