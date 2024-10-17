using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.DTOs
{
    public class CartDTO
    {
        [Required ]
        public decimal? Quantity { get; set; }
    }
}
