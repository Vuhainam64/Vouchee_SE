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
    public class CreateImageDTO
    {
        public Guid? id { get; set; }

        public string? imageUrl { get; set; }

        public ObjectStatusEnum status = ObjectStatusEnum.ACTIVE;
        public DateTime? CreateDate = DateTime.Now;
    }

    public class UpdateImageDTO
    {

    }

    public class GetImageDTO
    {
        public Guid id { get; set; }
        public string? imageUrl { get; set; }
        public MediaEnum? imageType { get; set; }
    }
}
