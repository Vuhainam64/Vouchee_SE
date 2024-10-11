using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models.ViewModels
{
    public class RefreshTokenRequest
    {
        public Guid userId { get; set; }
        public string refreshToken { get; set; }
    }
}
