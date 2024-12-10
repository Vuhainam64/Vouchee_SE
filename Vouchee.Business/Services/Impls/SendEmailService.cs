using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Services.Impls
{
    public class SendEmailService : ISendEmailService
    {
        //private readonly SmtpClient _smtpClient;

        //public SendEmailService(SmtpClient smtpClient)
        //{
        //    _smtpClient = smtpClient;
        //}

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                //var mailMessage = new MailMessage("advouchee@gmail.com", to)
                //{
                //    Subject = subject,
                //    Body = body,
                //    IsBodyHtml = false
                //};

                //await _smtpClient.SendMailAsync(mailMessage);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("advouchee@gmail.com");
                    mail.To.Add("nguyenquyphat2711@gmail.com");
                    mail.Subject = "Hello World";
                    mail.Body = "<h1>Hello</h1>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("advouchee@gmail.com", "sfji znul dxwb jqfy");
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }  
        }
    }
}
