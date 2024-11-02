using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;

namespace Vouchee.Data.Models.Entities
{
    [Table("User")]
    [Index(nameof(RoleId), Name = "IX_User_RoleId")]
    public partial class User
    {
        public User()
        {
            Orders = [];
            Vouchers = [];
            Carts = [];
        }

        [InverseProperty(nameof(Order.User))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Voucher.Seller))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(Cart.Buyer))]
        public virtual ICollection<Cart> Carts { get; set; }

        public Guid? RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("Users")]
        public virtual Role? Role { get; set; }

        public virtual Supplier? Supplier { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        [StringLength(10)]
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? HashPassword { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public int ResponsibilityScore { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
