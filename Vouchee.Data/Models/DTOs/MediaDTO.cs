using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateMediaDTO
    {
        public Guid? id { get; set; }

        public string? url { get; set; }

        public ObjectStatusEnum status = ObjectStatusEnum.ACTIVE;
        public DateTime? CreateDate = DateTime.Now;
    }

    public class UpdateMediaDTO
    {

    }

    public class GetMediaDTO
    {
        public Guid id { get; set; }
        public string? url { get; set; }
        public int? index { get; set; }
    }
}
