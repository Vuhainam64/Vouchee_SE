using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    public class AuthResponse
    {
        public string id { get; set; }
        public string? roleId { get; set; }
        public string? roleName { get; set; }
        public string? uid { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? image { get; set; }
        public string? token { get; set; }
        public string? buyerId { get; set; }
    }
}
