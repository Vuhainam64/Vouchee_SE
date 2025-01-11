using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Rating))]
    public class Rating
    {
        public Rating()
        {
            Medias = [];
            Reports = [];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.Rating))]
        public virtual Order? Order { get; set; }

        public Guid? ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.Ratings))]
        public virtual Modal? Modal { get; set; }

        [InverseProperty(nameof(Media.Rating))]
        public virtual ICollection<Media> Medias { get; set; }
        [InverseProperty(nameof(Report.Rating))]
        public virtual ICollection<Report> Reports { get; set; }

        public int QualityStar { get; set; }
        public int ServiceStar { get; set; }
        public int SellerStar { get; set; }
        public string? Comment { get; set; }
        public string? Reply { get; set; }
        public string? Reason { get; set; }
        public int NumberOfReport => Reports.Count;

        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? ReplyDate { get; set; }
        public Guid? ReplyBy { get; set; }
    }
}
