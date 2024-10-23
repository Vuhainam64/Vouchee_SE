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
            sellers = [];
        }

        public int totalQuantity { get; set; }
        public decimal totalPrice { get; set; }
        public decimal discountPrice { get; set; }
        public decimal finalPrice { get; set; }

        public virtual ICollection<SellerCartDTO> sellers { get; set; }
    }

    public class SellerCartDTO
    {
        public SellerCartDTO()
        {
            // modals = [];
        }

        public Guid? id { get; set; }
        public string? name { get; set; }
        public string? image { get; set; }

        // public virtual ICollection<CartModalDTO> modals { get; set; }
    }
}
