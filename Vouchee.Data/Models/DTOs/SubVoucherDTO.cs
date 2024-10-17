using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateSubVoucherDTO
    {
        public string? title { get; set; }
        public decimal originalPrice { get; set; }
        public decimal sellPrice { get; set; }
        public int? quantity { get; set; }
    }

    public class UpdateSubVoucherDTO : CreateSubVoucherDTO
    {

    }

    public class GetSubVoucherDTO
    {

    }
}
