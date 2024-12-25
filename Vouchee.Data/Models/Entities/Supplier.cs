using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Supplier))]
    public partial class Supplier
    {
        public Supplier()
        {
            Vouchers = [];
            Users = [];
        }

        [InverseProperty(nameof(Wallet.Supplier))]
        public virtual Wallet? SupplierWallet { get; set; }

        [InverseProperty(nameof(Voucher.Supplier))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(User.Supplier))]
        public virtual ICollection<User> Users { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Contact { get; set; }
        public bool IsVerified { get; set; }
        public string? Image { get; set; }
        //public string? BankName { get; set; }
        //public string? BankNumber { get; set; }
        //public string? BankAccount { get; set; }
        public int? TotalQuantitySold => Vouchers.Sum(x => x.Modals.Sum(x => x.OrderDetails.Sum(x => x.Quantity)));

        public bool IsActive { get; set; }
        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public Guid? VerifiedBy { get; set; }
    }
}
