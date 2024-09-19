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
            Orders = new HashSet<Order>();
        }

        [InverseProperty(nameof(Order.User))]
        public virtual ICollection<Order> Orders { get; set; }

        public Guid? RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("Users")]
        public virtual Role? Role { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Description { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        [StringLength(10)]
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? SecretKey { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
