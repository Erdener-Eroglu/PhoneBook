using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PhoneBookBusinessLayer.EmailSenderBusiness
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SenderMail => _configuration.GetSection("EmailOptions:SenderMail").Value;
        public string Password => _configuration.GetSection("EmailOptions:Password").Value;
        public string Smtp => _configuration.GetSection("EmailOptions:Smtp").Value;
        public int SmptPort => Convert.ToInt32(_configuration.GetSection("EmailOptions:SmptPort").Value);

        private void MailInfoSet(EmailMessage message, out MailMessage mail, out SmtpClient client)
        {
            try
            {
                mail = new MailMessage()
                {
                    From = new MailAddress(SenderMail) //sınıfın/projenin maili
                };
                //to'yu ekleyelim
                foreach (var item in message.To)
                {
                    mail.To.Add(item);
                }
                //CC ve BCC sonra
                mail.Subject = message.Subject; 
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                client = new SmtpClient(Smtp, SmptPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(SenderMail,Password) //emaile girebilmek için kulllanıcı adı ve parolası gereklidir.
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool SendEmail(EmailMessage message)
        {
            try
            {
                MailInfoSet(message,out MailMessage mail,out SmtpClient client);
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            try
            {
                MailInfoSet(message, out MailMessage mail, out SmtpClient client);
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
