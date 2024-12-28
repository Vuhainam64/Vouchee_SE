using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class VoucherCodeService : IVoucherCodeService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IMapper _mapper;

        public VoucherCodeService(IBaseRepository<User> userRepository,
                                    IBaseRepository<Modal> modalRepository,
                                    IBaseRepository<VoucherCode> voucherCodeRepository,
                                    IBaseRepository<Voucher> voucherRepository,
                                    IFileUploadService fileUploadService,
                                    IMapper mapper)
        {
            _userRepository = userRepository;
            _modalRepository = modalRepository;
            _fileUploadService = fileUploadService;
            _voucherCodeRepository = voucherCodeRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<GetDetailModalDTO>> CreateVoucherCodeAsync(Guid modalId, IList<CreateVoucherCodeDTO> createVoucherCodeDTOs, ThisUserObj thisUserObj)
        {
            try
            {
                IList<Guid> list = [];

                var exisedModal = await _modalRepository.GetByIdAsync(modalId,
                    includeProperties: x => x.Include(m => m.Voucher)
                                             .Include(m => m.Carts)
                                                 .ThenInclude(c => c.Buyer),
                    isTracking: true);

                if (exisedModal == null)
                {
                    throw new NotFoundException($"Không tìm thấy voucher với id {modalId}");
                }

                int count = 0;

                foreach (var createVoucherCode in createVoucherCodeDTOs)
                {
                    var voucherCode = _mapper.Map<VoucherCode>(createVoucherCode);
                    voucherCode.ModalId = exisedModal.Id;
                    voucherCode.Status = VoucherCodeStatusEnum.PENDING.ToString();
                    voucherCode.CreateBy = thisUserObj.userId;

                    exisedModal.VoucherCodes.Add(voucherCode);

                    count++;
                }

                // Update the stock of exisedModal
                //exisedModal.Stock += count;

                //// Update voucher stock as well
                //exisedModal.Voucher.Stock += count;

                var voucherUpdateSuccess = await _modalRepository.UpdateAsync(exisedModal);

                var voucherCodes = exisedModal.VoucherCodes
                                        .OrderByDescending(x => x.CreateDate)
                                        .Take(count)
                                        .ToList();

                exisedModal.VoucherCodes = voucherCodes;

                if (voucherUpdateSuccess)
                {
                    return new ResponseMessage<GetDetailModalDTO>()
                    {
                        message = "Thêm thành công",
                        result = true,
                        value = _mapper.Map<GetDetailModalDTO>(exisedModal)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException(ex.Message);
            }
        }


        public async Task<bool> DeleteVoucherCodeAsync(Guid id)
        {
            var existedVoucherCode = await _voucherCodeRepository.FindAsync(id, false);
            if (existedVoucherCode == null)
            {
                throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
            }

            var result = await _voucherCodeRepository.DeleteAsync(existedVoucherCode);

            return result;
        }

        public async Task<GetVoucherCodeDTO> GetVoucherCodeByIdAsync(Guid id)
        {
            try
            {
                var existedVoucherCode = await _voucherCodeRepository.GetByIdAsync(id);
                GetVoucherCodeDTO voucherCodeDTO = _mapper.Map<GetVoucherCodeDTO>(existedVoucherCode);

                if (existedVoucherCode == null)
                {
                    throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
                }

                return voucherCodeDTO;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<IList<GetVoucherCodeDTO>> GetVoucherCodesAsync(VoucherCodeFilter voucherCodeFilter)
        {
            try
            {
                IQueryable<GetVoucherCodeDTO> result;
                try
                {
                    result = _voucherCodeRepository.GetTable()
                                .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider)
                                .DynamicFilter(_mapper.Map<GetVoucherCodeDTO>(voucherCodeFilter));
                }
                catch (Exception ex)
                {
                    LoggerService.Logger(ex.Message);
                    throw new LoadException("Lỗi không xác định khi tải voucher code");
                }
                return result.ToList();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }


        public async Task<bool> UpdateVoucherCodeAsync(Guid id, UpdateVoucherCodeDTO updateVoucherCodeDTO)
        {
            var existedVoucherCode = await _voucherCodeRepository.FindAsync(id, false);

            if (existedVoucherCode == null)
            {
                throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
            }

            _mapper.Map(updateVoucherCodeDTO, existedVoucherCode);

            var result = await _voucherCodeRepository.UpdateAsync(existedVoucherCode);

            return result;
        }

        public async Task<ResponseMessage<GetVoucherCodeDTO>> UpdateStatusVoucherCodeAsync(Guid id, VoucherCodeStatusEnum voucherCodeStatus, ThisUserObj thisUserObj)
        {
            var existedVoucherCode = await _voucherCodeRepository.FindAsync(id, true);

            if (existedVoucherCode == null)
            {
                throw new NotFoundException($"Không tìm thấy voucher code với id {id}");
            }
            if (existedVoucherCode.Status == VoucherCodeStatusEnum.PENDING.ToString())
            {
                existedVoucherCode.IsActive = true;
            }
            if(existedVoucherCode.Status == VoucherCodeStatusEnum.CONVERTING.ToString() && voucherCodeStatus == VoucherCodeStatusEnum.UNUSED)
            {
                existedVoucherCode.IsVerified = true;
            }
            existedVoucherCode.Status = voucherCodeStatus.ToString();
            existedVoucherCode.UpdateDate = DateTime.Now;
            existedVoucherCode.UpdateBy = thisUserObj.userId;

            await _voucherCodeRepository.UpdateAsync(existedVoucherCode);

            return new ResponseMessage<GetVoucherCodeDTO>()
            {
                message = "Đổi Status thành công",
                result = true,
                value = _mapper.Map<GetVoucherCodeDTO>(existedVoucherCode)
            };
        }

        public async Task<DynamicResponseModel<GetVoucherCodeDTO>> GetOrderedVoucherCode(Guid modalId, ThisUserObj thisUserObj,
                                                                                    PagingRequest pagingRequest, VoucherCodeFilter voucherCodeFilter)
        {
            (int, IQueryable<GetVoucherCodeDTO>) result;

            if (modalId == Guid.Empty)
            {
                result = _voucherCodeRepository.GetTable()
                                                 .Where(x => x.Order.CreateBy == thisUserObj.userId)
                                                 .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider)
                                                 .DynamicFilter(_mapper.Map<GetVoucherCodeDTO>(voucherCodeFilter))
                                                 .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            else
            {
                result = _voucherCodeRepository.GetTable()
                                 .Where(x => x.ModalId == modalId && x.Order.CreateBy == thisUserObj.userId)
                                 .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider)
                                 .DynamicFilter(_mapper.Map<GetVoucherCodeDTO>(voucherCodeFilter))
                                 .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }

            return new DynamicResponseModel<GetVoucherCodeDTO>()
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

        public async Task<IList<GetVoucherCodeDTO>> UpdateCodeVoucherCodeAsync(IList<UpdateCodeVoucherCodeDTO> updateCodeVoucherCodeDTO, ThisUserObj thisUserObj)
        {
            var voucherCodes = _voucherCodeRepository.GetTable();
            IList<GetVoucherCodeDTO> list = new List<GetVoucherCodeDTO>();
            foreach (var code in updateCodeVoucherCodeDTO)
            {
                var updatecode = voucherCodes.Where(c => c.Id == code.id)
                    .FirstOrDefaultAsync();
                if (updatecode != null)
                {
                    var result = await updatecode;
                    result.Status = VoucherCodeStatusEnum.UNUSED.ToString();
                    result.UpdateDate = DateTime.Now;
                    result.NewCode = code.newcode;
                    result.Comment = code.Comment;
                    result.UpdateBy = thisUserObj.userId;
                    result.UpdateStatus = code.UpdateStatus.ToString();
                    if (code.UpdateStatus == UpdateStatusEnum.SUCCESS)
                    {
                        result.IsVerified = true;
                    }
                    else { 
                        result.IsVerified = false; 
                    }
                    await _voucherCodeRepository.UpdateAsync(result);
                    list.Add(_mapper.Map<GetVoucherCodeDTO>(result));
                }
                else
                {
                    throw new Exception("Khong tim thay code");
                }
            }
            return list;
        }

        public async Task<ResponseMessage<GetVoucherCodeDTO>> UpdatePosVoucherCodeAsync(string code, ThisUserObj thisUserObj)
        {
            var voucherCodes = _voucherCodeRepository.GetTable();
            var findcode = voucherCodes.Where(c => c.NewCode == code.ToString())
                .FirstOrDefaultAsync();
            var updatecode = await findcode;
            if (updatecode != null && updatecode.Status == VoucherCodeStatusEnum.UNUSED.ToString())
            {

                updatecode.Status = VoucherCodeStatusEnum.USED.ToString();
                updatecode.UpdateDate = DateTime.Now;
                updatecode.UpdateBy = thisUserObj.userId;
                _voucherCodeRepository.UpdateAsync(updatecode);

                return new ResponseMessage<GetVoucherCodeDTO>()
                {
                    message = "Sử dụng thành công",
                    result = true,
                    value = _mapper.Map<GetVoucherCodeDTO>(updatecode)
                };
            }
            else if (updatecode == null)
            {
                throw new Exception("Không tìm thấy code");
            }
            else if (updatecode != null && updatecode.Status != VoucherCodeStatusEnum.USED.ToString())
            {
                throw new Exception("Voucher Code bị " + updatecode.Status);
            }
            throw new Exception("loi khong xac dinh");
        }

        public async Task<ResponseMessage<IList<GetVoucherCodechangeStatusDTO>>> UpdateVoucherCodeStatusConvertingAsync(IList<Guid> id, ThisUserObj thisUserObj)
        {
            var gennerateid = Guid.NewGuid();
            var voucherCodes = _voucherCodeRepository.GetTable();
            IList<GetVoucherCodechangeStatusDTO> list = new List<GetVoucherCodechangeStatusDTO>();

            foreach (var code in id)
            {
                // Await directly when fetching the voucher code
                var updatecode = await voucherCodes.FirstOrDefaultAsync(c => c.Id == code);

                if (updatecode != null)
                {
                    // Update the status and update ID
                    updatecode.Status = VoucherCodeStatusEnum.CONVERTING.ToString();
                    updatecode.UpdateId = gennerateid;
                    updatecode.UpdateDate = DateTime.Now;
                    // Await the update operation
                    await _voucherCodeRepository.UpdateAsync(updatecode);

                    // Map the updated result and add it to the list
                    list.Add(_mapper.Map<GetVoucherCodechangeStatusDTO>(updatecode));
                }
                else
                {
                    throw new Exception("Không tìm thấy code");
                }
            }

            return new ResponseMessage<IList<GetVoucherCodechangeStatusDTO>>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = list
            };
        }


        public async Task<DynamicResponseModel<GetVoucherCodeDTO>> GetSupplierVoucherCodeAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest, VoucherCodeFilter voucherCodeFilter)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("Tài khoản này không phải tài khoản supplier");
            }

            (int, IQueryable<GetVoucherCodeDTO>) result;

            result = _voucherCodeRepository.GetTable()
                                            .Where(x => x.Modal.Voucher.SupplierId == existedUser.SupplierId)
                                            .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider)
                                            .DynamicFilter(_mapper.Map<GetVoucherCodeDTO>(voucherCodeFilter))
                                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetVoucherCodeDTO>()
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

        public async Task<DynamicResponseModel<GroupedVoucherCodeDTO>> GetSupplierVoucherCodeConvertingAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("Tài khoản này không phải tài khoản supplier");
            }

            (int, IQueryable<GroupedVoucherCodeDTO>) result;

            result = _voucherCodeRepository.GetTable()
                                .Where(x => x.Modal.Voucher.SupplierId == existedUser.SupplierId)
                                .Where(x => x.UpdateId != null)
                                .Where(x => x.Status == VoucherCodeStatusEnum.CONVERTING.ToString() || x.Status == VoucherCodeStatusEnum.UNUSED.ToString())
                                .GroupBy(x => x.UpdateId)
                                .Select(g => new GroupedVoucherCodeDTO
                                {
                                    UpdateId = g.Key,
                                    Count = g.Count(),
                                    UpdateTime = g.Max(x => x.UpdateDate),
                                    Status = g.Any(x => x.Status == VoucherCodeStatusEnum.UNUSED.ToString()) ? "Đã xử lý":"Chưa sử lý",
                                    FirstItem = _mapper.Map<GetVoucherCodeDTO>(g.FirstOrDefault())
                                })
                                .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);


            return new DynamicResponseModel<GroupedVoucherCodeDTO>()
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
        public async Task<DynamicResponseModel<GetVoucherCodeDTO>> GetVoucherCodeByUpdateIdAsync(ThisUserObj thisUserObj, Guid updateid, PagingRequest pagingRequest)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("Tài khoản này không phải tài khoản supplier");
            }

            (int, IQueryable<GetVoucherCodeDTO>) result;

            result = _voucherCodeRepository.GetTable()
                                            .Where(x => x.Modal.Voucher.SupplierId == existedUser.SupplierId)
                                            .Where(x => x.UpdateId == updateid)
                                            .ProjectTo<GetVoucherCodeDTO>(_mapper.ConfigurationProvider)
                                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetVoucherCodeDTO>()
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
    }
}