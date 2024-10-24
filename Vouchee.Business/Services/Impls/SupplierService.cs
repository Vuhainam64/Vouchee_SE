﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs.Dashboard;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(ISupplierRepository supplierRepository,
                            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateSupplierAsync(CreateSupplierDTO createSupplierDTO)
        {
            try
            {
                var supplier = _mapper.Map<Supplier>(createSupplierDTO);

                supplier.Status = ObjectStatusEnum.ACTIVE.ToString();

                var supplierId = await _supplierRepository.AddAsync(supplier);
                return supplierId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo supplier");
            }
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa supplier");
            }
        }

        public async Task<IList<BestSuppleriDTO>> GetBestSuppliers()
        {
            IList<Supplier> suppliers;
            try
            {
                // Fetch suppliers and include their vouchers and order details
                suppliers = _supplierRepository.GetTable()
                    .Include(s => s.Vouchers)
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
                throw new LoadException("Lỗi không xác định khi tải supplier");
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
                throw new LoadException("Lỗi không xác định khi tải supplier");
            }
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
                throw new LoadException("Lỗi không xác định khi tải supplier");
            }
            return result.ToList();
        }

        public async Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDTO updateSupplierDTO)
        {
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật supplier");
            }
        }
    }
}
