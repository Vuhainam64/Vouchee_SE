using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Services
{
    public interface IBrandService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateBrandAsync(CreateBrandDTO createBrandDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetDetalBrandDTO> GetBrandByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetBrandDTO>> GetBrandsAsync(PagingRequest pagingRequest, BrandFilter brandFilter);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateBrandAsync(Guid id, UpdateBrandDTO updateBrandDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> UpdateBrandStatusAsync(Guid id, ObjectStatusEnum status, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> UpdateBrandStateAsync(Guid id, bool isActive, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> VerifyBrand(Guid id, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> RemoveAddressFromBrandAsync(Guid addressId, Guid brandId);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteBrandAsync(Guid id);
    }
}
