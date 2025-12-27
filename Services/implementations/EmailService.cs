using eShop.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace eShop.Services.implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string mailTo, string subject, string body)
        {
            var Settings = _config.GetSection("EmailSetting");
            var Email = new MimeMessage();
            Email.From.Add(new MailboxAddress(Settings["SenderName"], Settings["SenderEmail"]));
            Email.To.Add(MailboxAddress.Parse(mailTo));
            Email.Subject = subject;
            var builder = new BodyBuilder { HtmlBody = body };
            Email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(Settings["SmtpServer"], int.Parse(Settings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(Settings["UserName"], Settings["Password"]);
            await smtp.SendAsync(Email);
            await smtp.DisconnectAsync(true);
        }
    }
}
