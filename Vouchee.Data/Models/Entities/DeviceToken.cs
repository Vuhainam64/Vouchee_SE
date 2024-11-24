using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(DeviceToken))]
    public class DeviceToken
    {
        public DeviceToken()
        {
            Users = [];
        }

        [InverseProperty(nameof(User.DeviceTokens))]
        public virtual ICollection<User> Users { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Token { get; set; }
        public string? Platform { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
