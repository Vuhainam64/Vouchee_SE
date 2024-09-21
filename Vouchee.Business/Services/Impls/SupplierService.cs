using AutoMapper;
using AutoMapper.QueryableExtensions;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
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

        public async Task<DynamicResponseModel<GetSupplierDTO>> GetSuppliersAsync(PagingRequest pagingRequest,
                                                                            SupplierFilter supplierFilter,
                                                                            SortSupplierEnum sortSupplierEnum)
        {
            (int, IQueryable<GetSupplierDTO>) result;
            try
            {
                result = _supplierRepository.GetTable()
                            .ProjectTo<GetSupplierDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetSupplierDTO>(supplierFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LimitPaging, PageConstant.DefaultPaging);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải supplier");
            }
            return new DynamicResponseModel<GetSupplierDTO>()
            {
                PagingMetaData = new PagingMetaData()
                {
                    Page = pagingRequest.page,
                    Size = pagingRequest.pageSize,
                    Total = result.Item1
                },
                Results = result.Item2.ToList()
            };
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
