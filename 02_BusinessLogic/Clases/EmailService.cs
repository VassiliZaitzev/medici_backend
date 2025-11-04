using _00_Entities;
using _02_BusinessLogic.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace _01_DataLogic.Clases
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService()
        {
        }

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port,
                _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentName, string base64Attachment)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            // Convertir Base64 a bytes y agregar como adjunto
            var attachmentBytes = Convert.FromBase64String(base64Attachment);
            bodyBuilder.Attachments.Add(attachmentName, attachmentBytes);

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port,
                _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
