using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IAddressService
    {
        // CREATE
        public Task<ResponseMessage<Guid>?> CreateAddressAsync(CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetDetailAddressDTO> GetAddressByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetDetailAddressDTO>> GetAddressesAsync(PagingRequest pagingRequest, AddressFilter addressFilter);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> UpdateAddressStateAsync(Guid id, bool isActive, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> UpdateAddressStatusAsync(Guid id, ObjectStatusEnum statusEnum, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> VerifyAddressAsync(Guid id, bool isVerify , ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteAddressAsync(Guid id);
    }
}
