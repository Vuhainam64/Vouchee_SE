using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class AddressDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public string? name { get; set; }
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }
    }

    public class CreateAddressDTO : AddressDTO
    {
        public IFormFile? image { get; set; }
        public string status = ObjectStatusEnum.ACTIVE.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateAddressDTO : AddressDTO
    {

    }

    public class GetAddressDTO : AddressDTO
    {
        public Guid id { get; set; }

        public string? image { get; set; }

        public string? status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}