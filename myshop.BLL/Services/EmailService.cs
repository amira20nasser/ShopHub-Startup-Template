using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using myshop.BLL.Abstraction;

namespace myshop.BLL.Services
{
    /// <summary>
    /// Sends emails via SMTP using MailKit. Configure "Email:Smtp" (ideally via
    /// .NET user-secrets or environment variables, not the repository) to enable.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var host = _configuration["Email:Smtp:Host"];
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException(
                    "SMTP is not configured. Set Email:Smtp:Host (and Username/Password) to send email.");
            }

            var port = int.TryParse(_configuration["Email:Smtp:Port"], out var parsedPort) ? parsedPort : 587;
            var useSsl = bool.TryParse(_configuration["Email:Smtp:UseSsl"], out var parsedSsl) && parsedSsl;
            var username = _configuration["Email:Smtp:Username"];
            var password = _configuration["Email:Smtp:Password"];
            var fromEmail = _configuration["Email:Smtp:FromEmail"] ?? _configuration["Email:Smtp:From"] ?? "no-reply@myshop.com";
            var fromName = _configuration["Email:Smtp:FromName"];

            var message = new MimeMessage();
            message.From.Add(string.IsNullOrWhiteSpace(fromName)
                ? MailboxAddress.Parse(fromEmail)
                : MailboxAddress.Parse($"{fromName} <{fromEmail}>"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, useSsl ? MailKit.Security.SecureSocketOptions.SslOnConnect : MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);

            if (!string.IsNullOrWhiteSpace(username))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
