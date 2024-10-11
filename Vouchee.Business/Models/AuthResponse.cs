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
        public string? name { get; set; }
        public string? email { get; set; }
        public string? image { get; set; }
        public string? phoneNumber { get; set; }

        // Tokens
        public string? accessToken { get; set; }
        //public string? refreshToken { get; set; }

        // Token Expiry Times (in seconds or a DateTime)
        //public DateTime accessTokenExpiresAt { get; set; } // When the access token expires
        //public DateTime refreshTokenExpiresAt { get; set; } // When the refresh token expires
    }
}
