using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class RatingFilter
    {
        //[Required]
        public Guid? modalId { get; set; }
        public int? qualityStar { get; set; }
        public int? serviceStar { get; set; }
        public int? sellerStar { get; set; }
        public Guid? buyerId { get; set; }
        public string? buyerName { get; set; }
        public RatingStatusEnum? Status { get; set; }
        public int? minAverageStar { get; set; }
        public int? maxAverageStar { get; set; }
    }
}
