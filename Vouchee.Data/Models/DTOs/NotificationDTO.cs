using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class NotificationDTO
    {
        public Guid? receiverId { get; set; }

        public string? title { get; set; }
        public string? body { get; set; }
    }

    public class CreateNotificationDTO
    {
        public Guid receiverId { get; set; }

        public string? title { get; set; }
        public string? body { get; set; }
        public bool seen = false;

        public DateTime CreateDate = DateTime.Now;
    }

    public class UpdateNotificationDTO : NotificationDTO
    {
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
        public bool? seen { get; set; }
    }

    public class GetNotificationDTO : NotificationDTO
    {
        public Guid? id { get; set; }

        public bool? seen { get; set; }

        public string? receiverName { get; set; }
        public string? receiverImage { get; set; }

        public string? senderName { get; set; }
        public string? senderImage { get; set; }

        public DateTime? createDate { get; set; }
    }
}
