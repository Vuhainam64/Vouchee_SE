using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Notification))]
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid? SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        [InverseProperty(nameof(User.SenderNotifications))]
        public virtual User? Sender { get; set; }

        public Guid? ReceiverId { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        [InverseProperty(nameof(User.ReceiverNotifications))]
        public virtual User? Receiver { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Seen { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
