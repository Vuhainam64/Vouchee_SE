using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IDeviceTokenService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateDeviceToken(Guid userId, CreateDeviceTokenDTO createDeviceTokenDTO, DevicePlatformEnum devicePlatformEnum);

        // READ
        public Task<DynamicResponseModel<GetDeviceTokenDTO>> GetDeviceTokenAsync(PagingRequest pagingRequest, DeviceTokenFilter deviceTokenFilter, Guid userId);
    }
}
