using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateModalDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Original price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be greater than zero")]
        public decimal originalPrice { get; set; }

        [Required(ErrorMessage = "Sell price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sell price must be greater than zero")]
        public decimal sellPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value")]
        public int? quantity { get; set; }

        public VoucherStatusEnum status { get; set; }

        //public IList<IFormFile>? productImages { get; set; }
        public IList<string>? productImagesUrl { get; set; }

        public DateTime createDate = DateTime.Now;
    }

    public class UpdateModalDTO : CreateModalDTO
    {

    }

    public class GetModalDTO
    {

        public GetModalDTO()
        {
            images = new HashSet<GetMediaDTO>();
        }

        public Guid id { get; set; }
        public Guid? voucherId { get; set; }

        public string? title { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public int? quantity { get; set; }
        public string? image { get; set; }

        public virtual ICollection<GetMediaDTO>? images { get; set; }
    }
}
