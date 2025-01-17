using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface INotificationService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO, User trackingUser);

        // READ
        public Task<DynamicResponseModel<GetNotificationDTO>> GetNotificationsAsync(PagingRequest pagingRequest,
                                                                                        NotifcationFilter notifcationFilter);
        public Task<GetNotificationDTO> GetNotificationByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetNotificationDTO>> GetNotificationsByToUserIdAsync(PagingRequest pagingRequest,
                                                                                                NotifcationFilter notifcationFilter,
                                                                                                Guid userId);

        // UDPATE
        public Task<bool> UpdateNotificationsAsync(Guid id, 
                                                        UpdateNotificationDTO updateNotificationDTO, 
                                                        ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> MarkSeenAsync(Guid id, ThisUserObj thisUserObj);

        // DELETE
        public Task<bool> DeleteNotificationAsync(Guid id, ThisUserObj thisUserObj);
    }
}
