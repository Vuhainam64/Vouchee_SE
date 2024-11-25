using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class NotificationService : INotificationService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Data.Models.Entities.Notification> _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(IBaseRepository<User> userRepository,
                                   IBaseRepository<Data.Models.Entities.Notification> notificationRepository,
                                   IMapper mapper)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateNotificationAsync(Guid senderId, CreateNotificationDTO createNotificationDTO)
        {
            Data.Models.Entities.Notification notification = _mapper.Map<Data.Models.Entities.Notification>(createNotificationDTO);
            notification.ReceiverId = createNotificationDTO.receiverId;
            notification.SenderId = senderId;
            notification.CreateBy = senderId;

            var existedUser = await _userRepository.GetByIdAsync(notification.ReceiverId, includeProperties: x => x.Include(x => x.DeviceTokens));

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy người nhận");
            }


            if (existedUser.DeviceTokens.Count != 0)
            {
                foreach (var deviceToken in existedUser.DeviceTokens)
                {
                    var message = new Message
                    {
                        Token = deviceToken.Token,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = createNotificationDTO.title,
                            Body = createNotificationDTO.body,
                        }
                    };

                    await FirebaseMessaging.DefaultInstance.SendAsync(message);
                }

                var id = await _notificationRepository.AddAsync(notification);

                return new ResponseMessage<Guid>()
                {
                    message = "Tạo noti thành công",
                    result = true,
                    value = (Guid)id
                };
            }

            return null;
        }

        public Task<bool> DeleteNotificationAsync(Guid id, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public async Task<GetNotificationDTO> GetNotificationByIdAsync(Guid id)
        {
            try
            {
                Data.Models.Entities.Notification? existedNotification = await _notificationRepository.GetByIdAsync(id);
                if (existedNotification != null)
                {
                    throw new NotFoundException("Không tìm thấy notification này");
                }

                var result =  _mapper.Map<GetNotificationDTO>(existedNotification);
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<DynamicResponseModel<GetNotificationDTO>> GetNotificationsAsync(PagingRequest pagingRequest, NotifcationFilter notifcationFilter)
        {
            (int, IQueryable<GetNotificationDTO>) result;
            try
            {
                result = _notificationRepository.GetTable()
                                                    .ProjectTo<GetNotificationDTO>(_mapper.ConfigurationProvider)
                                                    .DynamicFilter(_mapper.Map<GetNotificationDTO>(notifcationFilter))
                                                    .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING); ;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetNotificationDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1
                },
                results = await result.Item2.ToListAsync()
            };
        }

        public async Task<DynamicResponseModel<GetNotificationDTO>> GetNotificationsByToUserIdAsync(PagingRequest pagingRequest, NotifcationFilter notifcationFilter, Guid userId)
        {
            (int, IQueryable<GetNotificationDTO>) result;
            try
            {
                result = _notificationRepository.GetTable()
                                                    .Where(x => x.ReceiverId == userId)
                                                    .ProjectTo<GetNotificationDTO>(_mapper.ConfigurationProvider)
                                                    .DynamicFilter(_mapper.Map<GetNotificationDTO>(notifcationFilter))
                                                    .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING); ;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetNotificationDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1
                },
                results = await result.Item2.ToListAsync()
            };
        }

        public async Task<ResponseMessage<bool>> MarkSeenAsync(Guid id, ThisUserObj thisUserObj)
        {
            var existedNotification = await _notificationRepository.GetByIdAsync(id, isTracking: true);
            if (existedNotification == null)
            {
                throw new NotFoundException("Không tìm thấy thông báo này");
            }

            existedNotification.Seen = true;
            existedNotification.UpdateDate = DateTime.Now;
            existedNotification.UpdateBy = thisUserObj.userId;

            await _notificationRepository.UpdateAsync(existedNotification);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thành công",
                result = true,
                value = true
            };
        }

        public Task<bool> UpdateNotificationsAsync(Guid id, UpdateNotificationDTO updateNotificationDTO, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }
    }
}
