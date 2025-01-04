using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Modal))]
    public class Modal
    {
        public Modal()
        {
            Carts = [];
            VoucherCodes = [];
            OrderDetails = [];
            Ratings = [];
        }

        [InverseProperty(nameof(Cart.Modal))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(VoucherCode.Modal))]
        public virtual ICollection<VoucherCode> VoucherCodes { get; set; }
        [InverseProperty(nameof(OrderDetail.Modal))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(Rating.Modal))]
        public virtual ICollection<Rating> Ratings { get; set; }

        public Guid VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty(nameof(Voucher.Modals))]
        public required virtual Voucher Voucher { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; }
        public int OriginalPrice { get; set; }
        public int SellPrice { get; set; }
        public int Index { get; set; }
        public string? Image { get; set; }
        public int Stock => VoucherCodes.Count(x => x.OrderId == null 
                                                        && x.Status.Equals(VoucherCodeStatusEnum.NONE.ToString())
                                                            && x.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow)
                                                            && x.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow));
        public decimal AverageRating => Ratings.Any() ? Math.Round(
                                                        Ratings.Average(rating =>
                                                            (rating.QualityStar + rating.ServiceStar + rating.SellerStar) / 3m),
                                                        1)
                                                    : 0;

        public bool IsActive { get; set; }
        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
