using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MusicWebAppBackend.Infrastructure.Models;

namespace MusicWebAppBackend.Infrastructure.Helpers
{
    public class ConfigEmail
    {
        private readonly EmailSettings _emailSettings;

        public ConfigEmail(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SetContent(string email, string title, string body)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SmtpUsername, _emailSettings.SmtpUsername));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = title;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

    }
}
