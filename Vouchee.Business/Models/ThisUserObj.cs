using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Models
{
    public class ThisUserObj
    {
        public Guid userId { get; set; }
        public string? email { get; set; }
        public Guid roleId { get; set; }
        public string? roleName { get; set; }
        public string? fullName { get; set; }
        public Guid adminRoleId { get; set; }
        public Guid? userRoleId { get; set; }
        public Guid? staffRoleId { get; set; }
        public CartDTO? cartDTO { get; set; }
    }
}
