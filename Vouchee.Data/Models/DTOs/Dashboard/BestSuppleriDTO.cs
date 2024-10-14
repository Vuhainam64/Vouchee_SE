using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.DTOs.Dashboard
{
    public class BestSuppleriDTO
    {
        public Guid id { get; set; }

        public string? name { get; set; }
        public string? image { get; set; }

        public int? soldVoucher { get; set; }
    }
}
