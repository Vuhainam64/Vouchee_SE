using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateDeviceTokenDTO
    {
        public Guid UserId { get; set; }
        public string? Token { get; set; }
        public DevicePlatformEnum? Platform { get; set; }
    }
    public class NotificationDeviceTokenDTO
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
