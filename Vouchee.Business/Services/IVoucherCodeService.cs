using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using Microsoft.AspNetCore.Mvc;
using Vouchee.Data.Models.DTOs;
using System.Collections;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Services
{
    public interface IVoucherCodeService
    {
        // CREATE
        public Task<ResponseMessage<GetDetailModalDTO>> CreateVoucherCodeAsync(Guid modalId, IList<CreateVoucherCodeDTO> createVoucherCodeDTOs, ThisUserObj thisUserObj);

        // READ
        public Task<GetVoucherCodeDTO> GetVoucherCodeByIdAsync(Guid id, bool isUsing, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetVoucherCodeDTO>> GetVoucherCodeByUpdateIdAsync(ThisUserObj thisUserObj ,Guid id, PagingRequest pagingRequest);
        public Task<IList<GetVoucherCodeDTO>> GetVoucherCodesAsync(VoucherCodeFilter voucherCodeFilter);
        public Task<DynamicResponseModel<GetVoucherCodeDTO>> GetOrderedVoucherCode(Guid modalId, ThisUserObj thisUserObj, PagingRequest pagingRequest, VoucherCodeFilter voucherCodeFilter);
        public Task<DynamicResponseModel<GetVoucherCodeDTO>> GetSupplierVoucherCodeAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest, VoucherCodeFilter voucherCodeFilter);
        public Task<DynamicResponseModel<GroupedVoucherCodeDTO>> GetSupplierVoucherCodeConvertingAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest);

        // UPDATE
        public Task<bool> UpdateVoucherCodeAsync(Guid id, UpdateVoucherCodeDTO updateVoucherCodeDTO);
        public Task<ResponseMessage<GetVoucherCodeDTO>> UpdateStatusVoucherCodeAsync(Guid id, VoucherCodeStatusEnum voucherCodeStatus, ThisUserObj thisUserObj);
        public Task<ResponseMessage< IList<GetVoucherCodechangeStatusDTO>>> UpdateVoucherCodeStatusConvertingAsync(IList<Guid> id, ThisUserObj thisUserObj);
        public Task<IList<GetVoucherCodeDTO>> UpdateCodeVoucherCodeAsync(IList<UpdateCodeVoucherCodeDTO> updateCodeVoucherCodeDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<GetVoucherCodeDTO>> UpdatePosVoucherCodeAsync(string code, ThisUserObj thisUserObj);
        
        // DELETE
        public Task<bool> DeleteVoucherCodeAsync(Guid id);
    }
}
