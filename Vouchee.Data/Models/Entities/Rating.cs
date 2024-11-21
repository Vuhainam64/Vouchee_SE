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
        }

        [Key]
        public string? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.Rating))]
        public required virtual Order? Order { get; set; }

        [Key]
        public Guid? ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.Ratings))]
        public required virtual Modal? Modal { get; set; }

        [InverseProperty(nameof(Media.Rating))]
        public virtual ICollection<Media> Medias { get; set; }

        public int Star { get; set; }
        public string? Comment { get; set; }
        public string? Reply { get; set; }

        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime ReplyDate { get; set; }
        public Guid? ReplyBy { get; set; }
    }
}
