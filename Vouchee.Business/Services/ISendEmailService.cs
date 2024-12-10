using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Services
{
    public interface ISendEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body);
    }
}
