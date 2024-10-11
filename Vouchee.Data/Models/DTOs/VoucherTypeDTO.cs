using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Vouchee.Business.Models.DTOs
{
    public class CreateVoucherTypeDTO
    {
        [Required(ErrorMessage = "Cần title")]
        public string? title { get; set; }
        public IFormFile? image { get; set; }
        public DateTime? createDate = DateTime.Now;
        public string? status = ObjectStatusEnum.ACTIVE.ToString();
    }

    public class UpdateVoucherTypeDTO
    {
        [Required(ErrorMessage = "Cần title")]
        public string? title { get; set; }
        public ObjectStatusEnum status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetVoucherTypeDTO
    {
        public GetVoucherTypeDTO()
        {
            vouchers = new HashSet<GetVoucherDTO>();
            categories = new HashSet<GetCategoryDTO>();
        }

        public Guid? id { get; set; }
        public string? image { get; set; }
        public string? title { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetVoucherDTO>? vouchers { get; set; }
        public virtual ICollection<GetCategoryDTO>? categories { get; set; }
    }
}
