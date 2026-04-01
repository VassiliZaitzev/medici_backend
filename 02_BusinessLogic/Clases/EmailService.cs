using _00_Entities;
using _02_BusinessLogic.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace _01_DataLogic.Clases
{
    public class EmailService : IEmailService
    {
        
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(config["SmtpSettings:FromName"], config["SmtpSettings:FromEmail"]));
            message.To.Add(MailboxAddress.Parse(to));

            //message.Cc.Add(MailboxAddress.Parse("contacto@medicy.cl"));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(config["SmtpSettings:Host"], Convert.ToInt32(config["SmtpSettings:Port"]),
                Convert.ToBoolean(config["SmtpSettings:EnableSsl"]) ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(config["SmtpSettings:User"], config["SmtpSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentName, string base64Attachment)
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            var message = new MimeMessage();
            var asd = config["SmtpSettings:FromName"];
            message.From.Add(new MailboxAddress(config["SmtpSettings:FromName"], config["SmtpSettings:FromEmail"]));
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
            await client.ConnectAsync(config["SmtpSettings:Host"], Convert.ToInt32(config["SmtpSettings:Port"]),
                Convert.ToBoolean(config["SmtpSettings:EnableSsl"]) ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(config["SmtpSettings:User"], config["SmtpSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
