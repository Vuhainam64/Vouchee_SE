using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Services.Impls
{
    public class SendEmailService : ISendEmailService
    {
        private readonly SmtpClient _smtpClient;

        public SendEmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage("advouchee@gmail.com", to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
    }
}
