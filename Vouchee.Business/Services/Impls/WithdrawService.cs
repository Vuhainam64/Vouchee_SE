using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Smtp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class WithdrawService : IWithdrawService
    {
        private readonly ISendEmailService _sendEmailService;

        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IMapper _mapper;

        public WithdrawService(ISendEmailService sendEmailService,
                               IBaseRepository<Wallet> walletRepository,
                               IBaseRepository<WalletTransaction> walletTransactionRepository,
                               IBaseRepository<User> userRepository,
                               IBaseRepository<MoneyRequest> moneyRequestRepository,
                               IMapper mapper)
        {
            _sendEmailService = sendEmailService;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _userRepository = userRepository;
            _moneyRequestRepository = moneyRequestRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<string>> CreateWithdrawRequestAsync(WalletTypeEnum walletTypeEnum, CreateWithdrawRequestDTO createWithdrawRequestDTO, ThisUserObj thisUserObj)
        {
            var generateId = Guid.NewGuid();
            var existedUser = await _userRepository.FindAsync(thisUserObj.userId, isTracking: true);

            MoneyRequest moneyRequest = new()
            {
                Status = MoneyRequestEnum.PENDING.ToString(),
                Amount = createWithdrawRequestDTO.amount,
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
                UserId = thisUserObj.userId,
                Type = MoneyRequestTypeEnum.WITHDRAW.ToString(),
            };

            WalletTransaction walletTransaction = new()
            {
                Status = WalletTransactionStatusEnum.PENDING.ToString(),
                Type = MoneyRequestTypeEnum.WITHDRAW.ToString(),
                CreateBy = thisUserObj.userId,
                CreateDate = DateTime.Now,
            };

            moneyRequest.WithdrawWalletTransaction = walletTransaction;

            if (walletTypeEnum == WalletTypeEnum.BUYER)
            {
                if (createWithdrawRequestDTO.amount > existedUser.BuyerWallet.Balance)
                {
                    throw new ConflictException($"Số tiền trong ví hiện tại không đủ, số dư hiện tại {existedUser.BuyerWallet.Balance}");
                }

                moneyRequest.WithdrawWalletTransaction.BuyerWalletId = existedUser.BuyerWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví mua";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.BuyerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.BuyerWallet.Balance - createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.UpdateId = generateId;

                existedUser.BuyerWallet.Balance -= createWithdrawRequestDTO.amount;
                existedUser.BuyerWallet.UpdateDate = DateTime.Now;
                existedUser.BuyerWallet.UpdateBy = thisUserObj.userId;
            }
            else if (walletTypeEnum == WalletTypeEnum.SELLER)
            {
                if (createWithdrawRequestDTO.amount > existedUser.BuyerWallet.Balance)
                {
                    throw new ConflictException($"Số tiền trong ví hiện tại không đủ, số dư hiện tại {existedUser.SellerWallet.Balance}");
                }

                moneyRequest.WithdrawWalletTransaction.SellerWalletId = existedUser.SellerWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví bán";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.SellerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.SellerWallet.Balance - createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.UpdateId = generateId;

                existedUser.SellerWallet.Balance -= createWithdrawRequestDTO.amount;
                existedUser.SellerWallet.UpdateDate = DateTime.Now;
                existedUser.SellerWallet.UpdateBy = thisUserObj.userId;
            }
            else if (walletTypeEnum == WalletTypeEnum.SUPPLIER)
            {
                if (createWithdrawRequestDTO.amount > existedUser.BuyerWallet.Balance)
                {
                    throw new ConflictException($"Số tiền trong ví hiện tại không đủ, số dư hiện tại {existedUser.Supplier.SupplierWallet.Balance}");
                }

                moneyRequest.WithdrawWalletTransaction.SupplierWalletId = existedUser.Supplier.SupplierWallet.Id;
                moneyRequest.WithdrawWalletTransaction.Note = "Rút tiền từ ví supplier";
                moneyRequest.WithdrawWalletTransaction.BeforeBalance = existedUser.SellerWallet.Balance;
                moneyRequest.WithdrawWalletTransaction.Amount = createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.AfterBalance = existedUser.SellerWallet.Balance - createWithdrawRequestDTO.amount;
                moneyRequest.WithdrawWalletTransaction.UpdateId = generateId;

                existedUser.Supplier.SupplierWallet.Balance -= createWithdrawRequestDTO.amount;
                existedUser.Supplier.SupplierWallet.UpdateDate = DateTime.Now;
                existedUser.Supplier.SupplierWallet.UpdateBy = thisUserObj.userId;
            }

            await _userRepository.SaveChanges();

            string? result = await _moneyRequestRepository.AddReturnString(moneyRequest);

            return new ResponseMessage<string>()
            {
                message = "Tạo yêu cầu rút tiền thành công",
                result = true,
                value = result
            };
        }

        public async Task<ResponseMessage<bool>> CreateWithdrawRequestInAllWalletAsync()
        {
            var generateId = Guid.NewGuid();

            var wallets = await _walletRepository.GetTable().AsTracking().ToListAsync();

            foreach (var wallet in wallets.ToList())
            {
                if (wallet.Balance > 0)
                {
                    string note = string.Empty;

                    if (wallet.BuyerId != null)
                    {
                        note = $"Tự động rút {wallet.Balance} từ ví mua, {wallet.Buyer}";

                        await _sendEmailService.SendEmailAsync(wallet.Buyer.Email, "Tự động rút tiền", note);
                    }
                    else if (wallet.SellerId != null)
                    {
                        note = $"Tự động rút  {wallet.Balance} từ ví bán, {wallet.Balance}";

                        await _sendEmailService.SendEmailAsync(wallet.Seller.Email, "Tự động rút tiền", note);
                    }
                    else if (wallet.SupplierId != null)
                    {
                        note = $"Tự động rút  {wallet.Balance} từ ví supplier, {wallet.Balance}";

                        foreach (var supplierAccount in wallet.Supplier.Users)
                        {
                            await _sendEmailService.SendEmailAsync(supplierAccount.Email, "Tự động rút tiền", note);
                        }
                    }

                    MoneyRequest moneyRequest = new MoneyRequest()
                    {
                        Status = "PENDING",
                        Amount = wallet.Balance,
                        CreateDate = DateTime.Now,
                        Type = "WITHDRAW",
                        WithdrawWalletTransaction = new()
                        {
                            Status = "PENDING",
                            Amount = wallet.Balance,
                            CreateDate = DateTime.Now,
                            Type = "WITHDRAW",
                            BeforeBalance = wallet.Balance,
                            AfterBalance = 0,
                            Note = note,
                            BuyerWalletId = wallet?.Buyer?.BuyerWallet?.Id,
                            SellerWalletId = wallet?.Seller?.SellerWallet?.Id,
                            SupplierWalletId = wallet?.Supplier?.SupplierWallet?.Id,
                            UpdateId = generateId
                        }
                    };

                    await _moneyRequestRepository.Add(moneyRequest);

                    wallet.Balance = 0;
                }
            }

            await _walletRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Đã tạo rút tiền thành công",
                result = true,
                value = true
            };
        }

        public async Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(
                PagingRequest pagingRequest,
                WithdrawRequestFilter withdrawRequestFilter,
                ThisUserObj thisUserObj)
        {
            // Generate a unique ID for the update
            var generateId = Guid.NewGuid();

            // Fetch and update all entities with the new UpdateId
            var query = _moneyRequestRepository.GetTable().AsTracking()
                .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()) && x.UserId == thisUserObj.userId);

            foreach (var entity in query)
            {
                entity.UpdateId = generateId;
            }

            await _moneyRequestRepository.SaveChanges();

            // Fetch the filtered and paginated data from the database
            var filteredQuery = query
                .ProjectTo<GetWithdrawRequestDTO>(_mapper.ConfigurationProvider)
                .DynamicFilter(_mapper.Map<GetWithdrawRequestDTO>(withdrawRequestFilter));

            // Execute the query to retrieve data as a list
            var filteredList = await filteredQuery.ToListAsync();

            // Perform grouping in memory
            var groupedResult = filteredList
                .GroupBy(x => x.updateId)
                .Select(group => new
                {
                    UpdateId = group.Key,
                    Items = group.ToList()
                })
                .ToList();

            // Flatten the grouped result if needed for the response
            var flattenedResult = groupedResult
                .SelectMany(g => g.Items)
                .ToList();

            // Calculate pagination metadata
            var totalItems = flattenedResult.Count;
            var pagedResults = flattenedResult
                .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                .Take(pagingRequest.pageSize)
                .ToList();

            return new DynamicResponseModel<GetWithdrawRequestDTO>
            {
                metaData = new MetaData
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = totalItems
                },
                results = pagedResults
            };
        }

        public async Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter)
        {
            // Fetch the base query
            var query = _moneyRequestRepository.GetTable().AsTracking()
                .Where(x => x.Type == MoneyRequestTypeEnum.WITHDRAW.ToString());

            // Apply dynamic filtering
            var filteredQuery = query
                .ProjectTo<GetWithdrawRequestDTO>(_mapper.ConfigurationProvider)
                .DynamicFilter(_mapper.Map<GetWithdrawRequestDTO>(withdrawRequestFilter));

            // Fetch the filtered data
            var filteredList = await filteredQuery.ToListAsync();

            // Assign WalletType and group results by UpdateId
            var processedList = filteredList.Select(item =>
            {
                item.WalletType = item.withdrawWalletTransaction?.sellerWalletId != null ? "Seller" :
                                  item.withdrawWalletTransaction?.buyerWalletId != null ? "Buyer" :
                                  item.withdrawWalletTransaction?.supplierWalletId != null ? "Supplier" : "Unknown";
                return item;
            }).ToList();

            // Group by UpdateId
            var groupedResult = processedList
                .GroupBy(x => x.updateId)
                .Select(group => new
                {
                    UpdateId = group.Key,
                    Items = group.ToList()
                })
                .ToList();

            // Flatten the grouped result if needed for pagination
            var flattenedResult = groupedResult
                .SelectMany(g => g.Items)
                .ToList();

            // Calculate pagination metadata
            var totalItems = flattenedResult.Count;
            var pagedResults = flattenedResult
                .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                .Take(pagingRequest.pageSize)
                .ToList();

            // Return the paginated response
            return new DynamicResponseModel<GetWithdrawRequestDTO>
            {
                metaData = new MetaData
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = totalItems
                },
                results = pagedResults
            };
        }
        public async Task<dynamic> GetWithdrawRequestbyMonthAsync(WithdrawRequestFilter withdrawRequestFilter)
        {
            var year = DateTime.Now.Year;  // Use the current year for grouping

            // Fetch the base query
            var query = _moneyRequestRepository.GetTable().AsTracking()
                .Where(x => x.Type == MoneyRequestTypeEnum.WITHDRAW.ToString());

            // Apply dynamic filtering based on the provided filter
            var filteredQuery = query
                .ProjectTo<GetWithdrawRequestDTO>(_mapper.ConfigurationProvider)
                .DynamicFilter(_mapper.Map<GetWithdrawRequestDTO>(withdrawRequestFilter));

            // Fetch the filtered data
            var filteredList = await filteredQuery.ToListAsync();
            var sumAmount = filteredList.Sum(x => x.amount);
            var countItem = filteredList.Count();
            var newestItem = DateTime.Now;
            // Group by Month and Year, and return the monthly totals
            var groupedResult = filteredList
                .Where(x => x.createDate?.Year == year)  // Filter by the current year
                .GroupBy(x => new { x.createDate?.Month, x.createDate?.Year })
                .Select(group => new MonthlyTotalDTO
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    TotalAmount = group.Sum(x => x.amount)  // Calculate total amount for the month
                })
                .ToList();

            // Ensure all 12 months are included, even if there is no data for a particular month
            var allMonths = Enumerable.Range(1, 12)  // Generate months 1 through 12
                .Select(month => new MonthlyTotalDTO
                {
                    Year = year,
                    Month = month,
                    TotalAmount = groupedResult.FirstOrDefault(g => g.Month == month)?.TotalAmount ?? 0  // Set TotalAmount to 0 if no data
                })
                .ToList();

            return new {
                allMonths,
                sumAmount,
                countItem,
                newestItem
                    };  // Return the list of MonthlyTotalDTO for all months
        }


        public async Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id)
        {
            var existedWithdrawRequest = await _moneyRequestRepository.GetByIdAsync(id);
            if (existedWithdrawRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu rút tiền này");
            }

            return _mapper.Map<GetWithdrawRequestDTO>(existedWithdrawRequest);
        }

        public async Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWithdrawWalletTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable()
                        .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()) && x.BuyerWallet.BuyerId == thisUserObj.userId)
                        .ProjectTo<GetWalletTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetWalletTransactionDTO>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWithdrawWalletTransactionByUpdateId(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, Guid updateId)
        {
            (int, IQueryable<GetWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable()
                        .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()))
                        .ProjectTo<GetWalletTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetWalletTransactionDTO>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<ResponseMessage<Guid>> UpdateWithdrawRequest(List<UpdateWithDrawRequestDTO> updateWithDrawRequestDTOs, ThisUserObj thisUserObj)
        {
            var generateId = Guid.NewGuid();
            foreach (var item in updateWithDrawRequestDTOs)
            {
                var withdrawRequest = await _moneyRequestRepository.GetByIdAsync(item.id, isTracking: true);
                // Generate a unique ID for the update
                
                if (withdrawRequest == null)
                {
                    throw new NotFoundException("Không tìm thấy request với id này");
                }

                withdrawRequest.UpdateId = generateId;

                withdrawRequest.Status = item.statusEnum.ToString();
                withdrawRequest.UpdateDate = DateTime.Now;
                withdrawRequest.UpdateBy = thisUserObj.userId;

                await _moneyRequestRepository.SaveChanges();
            }
            return new ResponseMessage<Guid>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = generateId
            };
        }
    }
}
