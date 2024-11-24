using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<DeviceToken> _devicetokenRepository;
        private readonly IMapper _mapper;

        public DeviceTokenService(IBaseRepository<User> userRepository, 
                                    IBaseRepository<DeviceToken> devicetokenRepository, 
                                    IMapper mapper)
        {
            _userRepository = userRepository;
            _devicetokenRepository = devicetokenRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateDeviceToken(Guid userId, CreateDeviceTokenDTO createDeviceTokenDTO, DevicePlatformEnum devicePlatformEnum)
        {
            var existedUser = await _userRepository.GetByIdAsync(userId, includeProperties: x => x.Include(x => x.DeviceTokens));

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user");
            }

            if (existedUser.DeviceTokens.FirstOrDefault(x => x.Token == createDeviceTokenDTO.token) != null)
            {
                throw new ConflictException("Người dùng này đã đăng ký token này");
            }

            var deviceToken = _mapper.Map<DeviceToken>(createDeviceTokenDTO);
            deviceToken.Platform = devicePlatformEnum.ToString();

            existedUser.DeviceTokens.Add(deviceToken);

            await _userRepository.SaveChanges();

            return new ResponseMessage<Guid>
            {
                message = "Tạo device token thành công",
                result = true,
                value = (Guid) deviceToken.Id
            };
        }

        public async Task<DynamicResponseModel<GetDeviceTokenDTO>> GetDeviceTokenAsync(PagingRequest pagingRequest, DeviceTokenFilter deviceTokenFilter, Guid userId)
        {
            (int, IQueryable<GetDeviceTokenDTO>) result;

            result = _userRepository.GetTable()
                                        .Include(x => x.DeviceTokens)
                                        .Where(x => x.Id == userId)
                        .ProjectTo<GetDeviceTokenDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetDeviceTokenDTO>(deviceTokenFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetDeviceTokenDTO>()
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
