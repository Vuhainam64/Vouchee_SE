﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateRatingDTO
    {
        public CreateRatingDTO() 
        {
            medias = [];
        }

        [Required(ErrorMessage = "Mã đơn hàng không được để trống.")]
        public string? orderId { get; set; }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        public Guid? modalId { get; set; }

        public virtual ICollection<CreateMediaDTO> medias { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int qualityStar { get; set; }
        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int serviceStar { get; set; }
        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int sellerStar { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự.")]
        public string? comment { get; set; }

        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateRatingDTO
    {
        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int qualityStar { get; set; }
        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int serviceStar { get; set; }
        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int sellerStar { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự.")]
        public string? comment { get; set; }

        public DateTime? updateDate = DateTime.Now;
    }

    public class GetRatingDTO
    {
        public GetRatingDTO()
        {
            medias = [];
            reports = [];
        }

        public Guid? id { get; set; }
        public string? orderId { get; set; }
        public Guid? modalId { get; set; }
        public string? modalName { get; set; }
        public string? modalImage { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public Guid? sellerId { get; set; }
        public string? sellerName { get; set; }
        public string? sellerImage { get; set; }
        public Guid? buyerId { get; set; }
        public string? buyerName { get; set; }
        public string? buyerImage { get; set; }
        public int numberOfReport { get; set; }

        public decimal? totalStar =>
            qualityStar.HasValue && serviceStar.HasValue && sellerStar.HasValue
                ? Math.Round((decimal)(qualityStar + serviceStar + sellerStar) / 3, 1)
                : (decimal?)null;

        public int? qualityStar { get; set; }
        public int? serviceStar { get; set; }
        public int? sellerStar { get; set; }

        public string? comment { get; set; }
        public string? rep { get; set; }
        public string? Status { get; set; }
        public DateTime? createDate { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
        public virtual ICollection<GetReportDTO> reports { get; set; }
    }
}
