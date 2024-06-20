using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public void SendErrorEmail(string csvFile, List<string> errorList)
        {
            string template;
            using (var reader = new StreamReader("ErrorTemplate.html"))
            {
                template = reader.ReadToEnd();
            }

            StringBuilder errors = new StringBuilder();
            foreach (string error in errorList)
            {
                errors.AppendLine($"<li>{error}</li>");
            }

            string body = template.Replace("[Csv File]", csvFile)
                .Replace("{{errors}}", errors.ToString());

            SendEmail(_emailSettings.ToEmail, "Error in NIMH .csv file", body);
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient();
            //Enable SSL/TLS for secure connection
            client.Connect(_emailSettings.Username, _emailSettings.Port, _emailSettings.EnableSsl);

            //Authenticate if using a password-protected email account
            client.Authenticate(_emailSettings.Username, _emailSettings.Password);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Username));
            message.To.Add(new MailboxAddress("Recipient", toEmail));
            message.Subject = subject;

            //Set the message body (can be plain text or HTML)
            message.Body = new TextPart("html") { Text = body };  //Assuming HTML template

            client.Send(message);
        }
    }
}
