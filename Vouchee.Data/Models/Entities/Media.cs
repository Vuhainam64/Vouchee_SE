﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Media))]
    public class Media
    {
        public Guid? VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty(nameof(Voucher.Medias))]
        public virtual Voucher? Voucher { get; set; }

        public Guid? RatingId { get; set; }
        [ForeignKey(nameof(RatingId))]
        [InverseProperty(nameof(Rating.Medias))]
        public virtual Rating? Rating { get; set; }

        public Guid? RefundRequestId { get; set; }
        [ForeignKey(nameof(RefundRequestId))]
        [InverseProperty(nameof(RefundRequest.Medias))]
        public virtual RefundRequest? RefundRequest { get; set; }

        public Guid? ReportId { get; set; }
        [ForeignKey(nameof(ReportId))]
        [InverseProperty(nameof(Report.Medias))]
        public virtual Report? Report { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Url { get; set; }
        public int Index { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
