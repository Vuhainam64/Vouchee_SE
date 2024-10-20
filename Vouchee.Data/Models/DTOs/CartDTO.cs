using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    //public class CartDTO
    //{
    //    [Required(ErrorMessage = "Quantity is required.")]
    //    [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    //    public decimal? Quantity { get; set; }
    //}
    //public class GetCartDTO : CartDTO
    //{
    //    public virtual GetUserDTO? User { get; set; }
    //    public virtual ICollection<GetAllVoucherDTO> Vouchers { get; set; }
    //}
    //public class UpdateCartDTO : CartDTO
    //{
    //    public virtual GetUserDTO? User { get; set; }
    //    public virtual ICollection<GetAllVoucherDTO> Vouchers { get; set; }
    //}
    //public class CreateCartDTO : CartDTO
    //{
    //    public virtual GetUserDTO? User { get; set; }
    //    public virtual ICollection<GetAllVoucherDTO> Vouchers { get; set; }
    //}

    public class CartDTO
    {
        public CartDTO()
        {
            vouchers = new HashSet<CartVoucherDTO>();
        }

        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal FinalPrice { get; set; }

        public virtual ICollection<CartVoucherDTO> vouchers { get; set; }
    }
}
