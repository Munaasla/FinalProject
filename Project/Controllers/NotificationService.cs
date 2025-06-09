using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Project.Services
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class NotificationService
    {
        private readonly EmailSettings _emailSettings;

        public NotificationService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string body)
        {
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentException("כתובת המייל ריקה או לא תקינה.");
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.Username),
                Subject = "תזכורת לחיסון",
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);
            using (var smtpClient = new SmtpClient(_emailSettings.Host))
            {
                smtpClient.Port = _emailSettings.Port;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                smtpClient.EnableSsl = _emailSettings.EnableSSL;
                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("שגיאה בשליחת מייל: " + ex.Message);
                    throw;
                }
            }
        }
    }
}
