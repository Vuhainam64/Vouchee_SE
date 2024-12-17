﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.DTOs.Dashboard;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class SupplierService : ISupplierService
    {
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(IBaseRepository<VoucherCode> voucherCodeRepository,
                               IBaseRepository<WalletTransaction> walletTransactionRepository,
                               IBaseRepository<User> userRepository,
                               IBaseRepository<Wallet> walletRepository,
                               IBaseRepository<Supplier> supplierRepository,
                               IMapper mapper)
        {
            _voucherCodeRepository = voucherCodeRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO)
        {
            var supplier = _mapper.Map<Supplier>(createSupplierDTO);

            supplier.Status = ObjectStatusEnum.ACTIVE.ToString();

            var supplierId = await _supplierRepository.AddAsync(supplier);
            return supplierId;
        }

        public async Task<ResponseMessage<bool>> CreateSupplierWalletAsync(Guid supplierId)
        {
            var existedSupplier = await _supplierRepository.GetByIdAsync(supplierId, includeProperties: x => x.Include(x => x.SupplierWallet), isTracking: true);
            if (existedSupplier == null)
            {
                throw new NotFoundException("Không tìm thấy ví của supplier này");
            }
            if (existedSupplier.SupplierWallet != null)
            {
                throw new ConflictException("Supplier này đã có ví rồi");
            }

            existedSupplier.SupplierWallet = new()
            {
                Status = "ACTIVE",
                Balance = 0,
                CreateDate = DateTime.Now,
                IsActive = true,
            };

            await _supplierRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Đã tạo ví tiền cho supplier thành công",
                result = true,
                value = true
            };
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            bool result = false;
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier != null)
            {
                result = await _supplierRepository.DeleteAsync(supplier);
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy supplier với id {id}");
            }
            return result;
        }

        public async Task<IList<BestSuppleriDTO>> GetBestSuppliers()
        {
            IList<Supplier> suppliers;
            try
            {
                // Fetch suppliers and include their vouchers and order details
                suppliers = _supplierRepository.GetTable()
                    //.ThenInclude(v => v.OrderDetails) // Include order details to calculate sold quantities
                    .ToList();

                // Calculate the total sold vouchers for each supplier and map it to BestSuppleriDTO
                var bestSuppleriDTOs = suppliers
                    .Select(supplier => new BestSuppleriDTO
                    {
                        id = supplier.Id,
                        name = supplier.Name,
                        image = supplier.Image,
                        //soldVoucher = supplier.Vouchers
                        //.SelectMany(v => v.OrderDetails)  // Get all order details across all vouchers
                        //.Sum(od => od.Quantity)           // Sum up the sold quantities (assuming Quantity field exists)
                    })
                    .OrderByDescending(s => s.soldVoucher) // Order by sold voucher quantity in descending order
                    .ToList();

                return bestSuppleriDTOs;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<GetSupplierDTO> GetSupplierByIdAsync(Guid id)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier != null)
                {
                    GetSupplierDTO supplierDTO = _mapper.Map<GetSupplierDTO>(supplier);
                    return supplierDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy supplier với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<dynamic> GetSupplierDashboard(ThisUserObj currentUser)
        {
            // Fetch the current user along with their Supplier data
            var existedUser = await _userRepository.GetByIdAsync(
                currentUser.userId,
                includeProperties: x => x.Include(x => x.Supplier).ThenInclude(s => s.SupplierWallet)
            );

            if (existedUser?.Supplier == null)
                throw new Exception("Supplier not found for the current user.");

            // Query for voucher data linked to the supplier
            var voucherCodes = _voucherCodeRepository.GetTable()
                .Where(x => x.Modal.Voucher.Supplier.Id == existedUser.Supplier.Id);

            // Query for wallet transactions linked to the supplier's wallet
            var walletTransactions = _walletTransactionRepository.GetTable()
                .Where(x => x.SupplierWalletId == existedUser.Supplier.SupplierWallet.Id);

            // Calculate statistics
            var pendingVouchers = await voucherCodes
                .CountAsync(x => x.Status == VoucherCodeStatusEnum.PENDING.ToString());

            var approvedVouchers = await voucherCodes
                .CountAsync(x => x.Status == VoucherCodeStatusEnum.UNUSED.ToString());

            // Group transactions by month
            var monthDashboard = await walletTransactions
                .GroupBy(x => new { x.CreateDate.Value.Year, x.CreateDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount) // Assuming there's an Amount property
                })
                .ToListAsync();

            // Return the dashboard data
            return new
            {
                pendingVouchers,
                approvedVouchers,
                monthDashboard
            };
        }

        public async Task<IList<GetSupplierDTO>> GetSuppliersAsync()
        {
            IQueryable<GetSupplierDTO> result;
            try
            {
                result = _supplierRepository.GetTable()
                            .ProjectTo<GetSupplierDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return result.ToList();
        }

        public async Task<dynamic> GetSupplierWalletTransactionAsync(ThisUserObj currentUser, PagingRequest pagingRequest, SupplierWalletTransactionFilter supplierWalletTransactionFilter)
        {
            var existedUser = await _userRepository.GetByIdAsync(currentUser.userId, includeProperties: x => x.Include(x => x.Supplier)
                                                                                                                .ThenInclude(x => x.SupplierWallet));

            (int, IQueryable<GetSupplierWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable()
                         .Where(x => x.SupplierWalletId == existedUser.Supplier.SupplierWallet.Id)
                         .ProjectTo<GetSupplierWalletTransactionDTO>(_mapper.ConfigurationProvider)
                         .DynamicFilter(_mapper.Map<GetSupplierWalletTransactionDTO>(supplierWalletTransactionFilter))
                         .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new
            {
                balance = existedUser.Supplier.SupplierWallet.Balance,
                supplierWalletId = existedUser.Supplier.SupplierWallet.Id,
                transasctions = new DynamicResponseModel<GetSupplierWalletTransactionDTO>()
                {
                    metaData = new MetaData()
                    {
                        page = pagingRequest.page,
                        size = pagingRequest.pageSize,
                        total = result.Item1 // Total vouchers count for metadata
                    },
                    results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
                }
            };
        }

        public async Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDTO updateSupplierDTO)
        {
            var existedSupplier = await _supplierRepository.GetByIdAsync(id);
            if (existedSupplier != null)
            {
                var entity = _mapper.Map<Supplier>(updateSupplierDTO);
                return await _supplierRepository.UpdateAsync(entity);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy supplier");
            }
        }

        public async Task<ResponseMessage<bool>> UpdateSupplierBankAsync(UpdateBankSupplierDTO updateBankSupplierDTO, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Supplier), isTracking: true);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }
            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("Tài khoản này không thuộc supplier nào");
            }

            existedUser.Supplier = _mapper.Map(updateBankSupplierDTO, existedUser.Supplier);

            await _userRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật supplier thành công",
                result = true,
                value = true
            };
        }
    }
}
