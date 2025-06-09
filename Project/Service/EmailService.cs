// Services/EmailService.cs
using System.Net;
using System.Net.Mail;

namespace Project.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = _config["EmailSettings:From"];
            var smtpServer = _config["EmailSettings:Smtp"];
            var smtpPort = int.Parse(_config["EmailSettings:Port"]);
            var password = _config["EmailSettings:Password"];
            var message = new MailMessage(fromEmail, toEmail, subject, body);
            var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };
            client.Send(message);
        }
    }
}
