using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class RatingFilter
    {
        [Required]
        public Guid modalId { get; set; }
        public int? qualityStar { get; set; }
        public int? serviceStar { get; set; }
        public int? sellerStar { get; set; }
    }
}
