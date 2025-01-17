﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Report))]
    public class Report
    {
        public Report()
        {
            Medias = [];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Reports))]
        public virtual User? User { get; set; }

        public Guid? RatingId { get; set; }
        [ForeignKey(nameof(RatingId))]
        [InverseProperty(nameof(Rating.Reports))]
        public virtual Rating? Rating { get; set; }

        [InverseProperty(nameof(Media.Report))]
        public virtual ICollection<Media> Medias { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
