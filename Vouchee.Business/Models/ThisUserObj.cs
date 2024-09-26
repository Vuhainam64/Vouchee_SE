using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    public class ThisUserObj
    {
        public string? userId { get; set; }
        public string? email { get; set; }
        public string? roleId { get; set; }
        public string? roleName { get; set; }
        public string? fullName { get; set; }
        public string? sellerId { get; set; }
        public string? buyerId { get; set; }
        public string? adminRoleId { get; set; }
        public string? sellerRoleId { get; set; }
        public string? buyerRoleId { get; set; }
        public string? staffRoleId { get; set; }
    }
}
