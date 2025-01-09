using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class ReportDTO
    {
        public string? reason { get; set; }
    }

    public class CreateReportDTO : ReportDTO
    {
        public CreateReportDTO()
        {
            imageUrl = [];
        }

        public string? status = "ACTIVE";
        public DateTime? createDate = DateTime.Now;
        public IList<string> imageUrl { get; set; }
    }

    public class UpdateReportDTO : ReportDTO
    {
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetReportDTO : ReportDTO
    {
        public GetReportDTO()
        {
            medias = [];
        }

        public Guid? id { get; set; }
        public Guid? userId { get; set; }
        public string? userName { get; set; }
        public Guid? ratingId { get; set; }
        public string? ratingContent { get; set; }
        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        // public virtual GetRatingDTO? rating { get; set; }
        // public virtual GetUserDTO? user { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
    }
}
