using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (walletTypeEnum.Equals(WalletTypeEnum.BUYER))
            {
                if (string.IsNullOrEmpty(existedUser.BuyerWallet.BankAccount) 
                        || string.IsNullOrEmpty(existedUser.BuyerWallet.BankName) 
                            || string.IsNullOrEmpty(existedUser.BuyerWallet.BankNumber))
                {
                    throw new ConflictException("Bạn càn cập nhật đầy đủ tài khoản ngân hàng mới có thể rút tiền");
                }
            }
            else if (walletTypeEnum.Equals(WalletTypeEnum.SELLER))
            {
                if (string.IsNullOrEmpty(existedUser.SellerWallet.BankAccount) 
                        || string.IsNullOrEmpty(existedUser.SellerWallet.BankName) 
                            || string.IsNullOrEmpty(existedUser.SellerWallet.BankNumber))
                {
                    throw new ConflictException("Bạn càn cập nhật đầy đủ tài khoản ngân hàng mới có thể rút tiền");
                }
            }
            else if (walletTypeEnum.Equals(WalletTypeEnum.SUPPLIER))
            {
                if (string.IsNullOrEmpty(existedUser.Supplier.SupplierWallet.BankAccount) 
                        || string.IsNullOrEmpty(existedUser.Supplier.SupplierWallet.BankName) 
                            || string.IsNullOrEmpty(existedUser.Supplier.SupplierWallet.BankNumber))
                {
                    throw new ConflictException("Bạn càn cập nhật đầy đủ tài khoản ngân hàng mới có thể rút tiền");
                }
            }

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
                moneyRequest.WithdrawWalletTransaction.Note = $"Rút {createWithdrawRequestDTO.amount} từ ví mua";
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
                moneyRequest.WithdrawWalletTransaction.Note = $"Rút {createWithdrawRequestDTO.amount} từ ví bán";
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
                moneyRequest.WithdrawWalletTransaction.Note = $"Rút {createWithdrawRequestDTO.amount} từ ví supplier";
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

            foreach (var wallet in wallets.ToList().Where(x => (x.SupplierId != null 
                                                                    && !string.IsNullOrEmpty(x.Supplier.SupplierWallet.BankAccount)
                                                                        && !string.IsNullOrEmpty(x.Supplier.SupplierWallet.BankName)
                                                                            && !string.IsNullOrEmpty(x.Supplier.SupplierWallet.BankNumber)) 
                                                                        || (x.SellerId != null
                                                                                && !string.IsNullOrEmpty(x.Seller.SellerWallet.BankAccount)
                                                                                    && !string.IsNullOrEmpty(x.Seller.SellerWallet.BankName)
                                                                                        && !string.IsNullOrEmpty(x.Seller.SellerWallet.BankNumber))))
            {
                if (wallet.Balance > 0)
                {
                    string note = string.Empty;

                    //if (wallet.BuyerId != null)
                    //{
                    //    note = $"Tự động rút {wallet.Balance} từ ví mua";

                    //    await _sendEmailService.SendEmailAsync(wallet.Buyer.Email, "Tự động rút tiền", note);
                    //}

                    if (wallet.SellerId != null)
                    {
                        note = $"Tự động rút {wallet.Balance} từ ví bán";

                        await _sendEmailService.SendEmailAsync(wallet.Seller.Email, "Tự động rút tiền", note);
                    }
                    else if (wallet.SupplierId != null)
                    {
                        note = $"Tự động rút {wallet.Balance} từ ví supplier";

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
                        Type = MoneyRequestTypeEnum.AUTO_WITHDRAW.ToString(),
                        WithdrawWalletTransaction = new()
                        {
                            Status = "PENDING",
                            Amount = wallet.Balance,
                            CreateDate = DateTime.Now,
                            Type = WalletTransactionTypeEnum.AUTO_WITHDRAW.ToString(),
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
                .Where(x => (x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()) || x.Type.Equals(MoneyRequestTypeEnum.AUTO_WITHDRAW.ToString()) 
                            && x.UserId == thisUserObj.userId));

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
                //.Where(x => x.UpdateId != null)
                .Where(x => x.Type == MoneyRequestTypeEnum.WITHDRAW.ToString() || x.Type == MoneyRequestTypeEnum.AUTO_WITHDRAW.ToString());

            if (withdrawRequestFilter.StartDate.HasValue && withdrawRequestFilter.EndDate.HasValue)
            {
                query = query.Where(x => x.CreateDate >= withdrawRequestFilter.StartDate.Value &&
                                         x.CreateDate <= withdrawRequestFilter.EndDate.Value);
            }

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
                .Where(x => x.Type == MoneyRequestTypeEnum.WITHDRAW.ToString() || x.Type == MoneyRequestTypeEnum.AUTO_WITHDRAW.ToString());

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

        public async Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWithdrawWalletTransactionAsync(
     PagingRequest pagingRequest,
     WalletTransactionFilter walletTransactionFilter,
     ThisUserObj thisUserObj)
        {
            // Fetch and filter the data
            var query = _walletTransactionRepository.GetTable()
                .Where(x => (x.Type.Equals(WalletTransactionTypeEnum.WITHDRAW.ToString())
                             || x.Type.Equals(WalletTransactionTypeEnum.AUTO_WITHDRAW.ToString()))
                            && ((x.BuyerWallet != null && x.BuyerWallet.BuyerId == thisUserObj.userId)
                                || (x.SellerWallet != null && x.SellerWallet.SellerId == thisUserObj.userId)
                                || (x.SupplierWallet != null && x.SupplierWallet.SupplierId == thisUserObj.userId)));

            // Apply dynamic filters
            var filteredQuery = query
                .ProjectTo<GetWalletTransactionDTO>(_mapper.ConfigurationProvider)
                .DynamicFilter(_mapper.Map<GetWalletTransactionDTO>(walletTransactionFilter));

            // Sort by note
            var sortedQuery = filteredQuery.Where(x => x.note.Equals(walletTransactionFilter.note));

            // Apply pagination
            var result = sortedQuery.PagingIQueryable(
                pagingRequest.page,
                pagingRequest.pageSize,
                PageConstant.LIMIT_PAGING,
                PageConstant.DEFAULT_PAPING);

            // Return the paginated response
            return new DynamicResponseModel<GetWalletTransactionDTO>
            {
                metaData = new MetaData
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total count
                },
                results = await result.Item2.ToListAsync() // Return paginated results
            };
        }


        public async Task<DynamicResponseModel<dynamic>> GetWithdrawWalletTransactionByUpdateId(
    PagingRequest pagingRequest,
    WithdrawRequestFilter walletTransactionFilter)
        {
            // Filter and group by updateId
            var filteredQuery = _moneyRequestRepository.GetTable()
                .Where(x => x.UpdateId != null)
                .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString())
                         || x.Type.Equals(MoneyRequestTypeEnum.AUTO_WITHDRAW.ToString()))
                .ProjectTo<GetWithdrawRequestDTO>(_mapper.ConfigurationProvider)
                .DynamicFilter(_mapper.Map<GetWithdrawRequestDTO>(walletTransactionFilter));

            if (!string.IsNullOrEmpty(walletTransactionFilter.updateId.ToString()))
            {
                filteredQuery = filteredQuery.Where(x => x.updateId.Equals(walletTransactionFilter.updateId));
            }

            var groupedQuery = filteredQuery
                .GroupBy(x => x.updateId)
                .Select(group => new
                {
                    UpdateId = group.Key,
                    Count = group.Count(),
                    UpdateDate = group.Max(x => x.updateDate),
                    Transactions = group.ToList(),
                    Status = group.All(x => x.status == "PAID")
                             ? "Hoàn thành"
                             : group.Any(x => x.status == "FAIL")
                             ? "Chưa hoàn thành"
                             : "Đang xử lý"
                });

            // Apply paging to the grouped results
            var pagedGroups = groupedQuery
                .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                .Take(pagingRequest.pageSize)
                .ToList();

            // Total count of unique groups
            var totalGroups = groupedQuery.Count();

            // Prepare the response
            var results = pagedGroups.Select(group => new
            {
                UpdateId = group.UpdateId,
                Count = group.Count,
                UpdateDate = group.UpdateDate,
                Status = group.Status,
                Transactions = group.Transactions
            }).ToList<dynamic>();

            return new DynamicResponseModel<dynamic>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = totalGroups
                },
                results = results
            };
        }



        public async Task<ResponseMessage<Guid>> UpdateWithdrawRequest(List<UpdateWithDrawRequestDTO> updateWithDrawRequestDTOs, ThisUserObj thisUserObj)
        {
            var generateId = Guid.NewGuid();
            List<Guid> updateId = new List<Guid>() ;
            foreach (var item in updateWithDrawRequestDTOs)
            {
                var withdrawRequest = await _moneyRequestRepository.GetByIdAsync(item.id, isTracking: true);
                // Generate a unique ID for the update
                
                if (withdrawRequest == null)
                {
                    throw new NotFoundException("Không tìm thấy request với id này");
                }
                if (withdrawRequest.UpdateId.HasValue) { 
                    updateId.Add((Guid)withdrawRequest.UpdateId); 
                }
                else {
                    updateId.Add(generateId);
                    withdrawRequest.UpdateId = generateId;
                }
                /*withdrawRequest.UpdateId = generateId;*/

                withdrawRequest.Note = item.note;
                withdrawRequest.Status = item.statusEnum.ToString();
                withdrawRequest.UpdateDate = DateTime.Now;
                withdrawRequest.UpdateBy = thisUserObj.userId;

                await _moneyRequestRepository.SaveChanges();
            }
            return new ResponseMessage<Guid>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = updateId.FirstOrDefault(),
            };
        }

        public Task<ResponseMessage<bool>> UpdateWithdrawRequest(string id, int amount, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMessage<bool>> DeleteWithdrawRequest(string id, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }
    }
}
