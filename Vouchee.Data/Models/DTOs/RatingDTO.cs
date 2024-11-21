using System;
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
        public int star { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự.")]
        public string? comment { get; set; }

        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateRatingDTO
    {
        [Range(1, 5, ErrorMessage = "Số sao phải nằm trong khoảng từ 1 đến 5.")]
        public int star { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự.")]
        public string? comment { get; set; }

        public DateTime? updateDate = DateTime.Now;
    }

    public class GetRatingDTO
    {
        public GetRatingDTO()
        {
            medias = [];
        }

        public string? orderId { get; set; }
        public Guid? modalId { get; set; }  

        public int? star { get; set; }
        public string? comment { get; set; }
        public string? rep { get; set; }

        public DateTime? createDate { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
    }
}
