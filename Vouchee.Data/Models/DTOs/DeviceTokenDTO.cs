using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateDeviceTokenDTO
    {
        public string? token { get; set; }

        public DateTime createDate = DateTime.Now;
    }

    public class GetDeviceTokenDTO
    {
        public Guid id { get; set; }

        public string? token { get; set; }

        public DateTime? createDate { get; set; }
    }
}
