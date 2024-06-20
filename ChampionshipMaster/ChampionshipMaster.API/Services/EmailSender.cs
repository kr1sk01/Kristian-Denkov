using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChampionshipMaster.API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAccountConfirmationEmail(string userEmail, string userName, string confirmationLink)
        {
            var applicationName = _configuration["Email:ApplicationName"]!;
            var templatePath = "Services/EmailTemplates/ConfirmationTemplate.html";
            var template = GetEmailTemplate(templatePath);

            var body = template.Replace("[Your Application Name]", applicationName)
                                .Replace("[User Name]", userName)
                                .Replace("[Confirmation Link]", confirmationLink);

            await SendEmailAsync(userEmail, $"Confirm Your Account on {applicationName}", body);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _configuration["Email:Host"]!;
            if (!int.TryParse(_configuration["Email:Port"], out int port))
            {
                throw new Exception("Invalid port configuration for email server!");
            }
            if (!bool.TryParse(_configuration["Email:EnableSsl"], out bool enableSsl))
            {
                throw new Exception("Invalid Ssl configuration for email server!");
            }
            var senderAddress = _configuration["Email:Username"]!;
            var password = _configuration["Email_Password"]!;

            using var client = new SmtpClient();

            // Enable SSL/TLS for secure connection
            client.Connect(host, port, enableSsl);

            // Authenticate if using a password-protected email account
            if (!string.IsNullOrEmpty(senderAddress))
            {
                client.Authenticate(senderAddress, password);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ChampionshipMaster", senderAddress));
            message.To.Add(new MailboxAddress("Recipient", toEmail));
            message.Subject = subject;

            // Set the message body (can be plain text or HTML)
            message.Body = new TextPart("html") { Text = body }; // Assuming HTML template

            await client.SendAsync(message);
        }

        private static string GetEmailTemplate(string templatePath)
        {
            using (var reader = new StreamReader(templatePath))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task SendGameScheduledEmail(string userEmail, string userName, string gameName, string userTeam, string opponentTeam, DateTime date)
        {
            var templatePath = "Services/EmailTemplates/GameScheduledTemplate.html";
            var template = GetEmailTemplate(templatePath);

            DateTimeOffset dateTimeOffset = new DateTimeOffset(date);
            string formattedDate = dateTimeOffset.ToString("dd/MM/yyyy HH:mm zz");

            var body = template.Replace("[User Name]", userName)
                .Replace("[Game Name]", gameName)
                .Replace("[User Team]", userTeam)
                .Replace("[Opponent Team]", opponentTeam)
                .Replace("[Game Date]", formattedDate);

            await SendEmailAsync(userEmail, $"Game scheduled", body);
        }

        public async Task SendGameFinishedEmail(string userEmail, string gameName, string blueTeam, int bluePoints, string redTeam, int redPoints)
        {
            var templatePath = "Services/EmailTemplates/GameFinishedTemplate.html";
            var template = GetEmailTemplate(templatePath);

            var body = template.Replace("[Game Name]", gameName)
                .Replace("[Blue Team]", blueTeam)
                .Replace("[Blue Points]", bluePoints.ToString())
                .Replace("[Red Team]", redTeam)
                .Replace("[Red Points]", redPoints.ToString());

            await SendEmailAsync(userEmail, $"Game finished", body);
        }

        public async Task SendAddedToTeamEmail(string userEmail, string userName, string teamName, string createdBy, string teamType, List<string> players)
        {
            var templatePath = "Services/EmailTemplates/AddedToTeamTemplate.html";
            var template = GetEmailTemplate(templatePath);

            StringBuilder playersList = new StringBuilder();
            foreach (string player in players)
            {
                playersList.AppendLine($"<li>{player}</li>");
            }

            string body = template.Replace("{{players}}", playersList.ToString())
                .Replace("[User Name]", userName)
                .Replace("[Team Name]", teamName)
                .Replace("[CreatedBy Name]", createdBy)
                .Replace("[Team Type]", teamType);

            await SendEmailAsync(userEmail, "Added to Team", body);
        }

        public async Task SendChampionshipLotEmail(string userEmail, string championshipName, int championshipId, string userName, string userTeam, string opponentTeam, DateTime? date)
        {
            var templatePath = "Services/EmailTemplates/ChampionshipLotTemplate.html";
            var template = GetEmailTemplate(templatePath);

            var formattedDate = "TBD";
            if (date != null)
            {
                DateTimeOffset dateTimeOffset = new DateTimeOffset(date.Value);
                formattedDate = dateTimeOffset.ToString("dd/MM/yyyy HH:mm zz");
            }

            string clientAddress = _configuration["CustomEnvironment"] == "Development" ? "https://localhost:50399" : "http://10.244.44.38:8090";
            string lotLink = $"{clientAddress}/championshipDetails/{championshipId}";

            string body = template.Replace("[User Name]", userName)
                .Replace("[Championship Name]", championshipName)
                .Replace("[User Team]", userTeam)
                .Replace("[Opponent Team]", opponentTeam)
                .Replace("[Date]", formattedDate)
                .Replace("[Lot Link]", lotLink);

            await SendEmailAsync(userEmail, "Championship Lot Drawn", body);
        }

        public async Task SendChampionshipFinishedEmail(string userEmail, string userName, string championshipName, string winnerName)
        {
            var templatePath = "Services/EmailTemplates/ChampionshipFinishedTemplate.html";
            var template = GetEmailTemplate(templatePath);

            string body = template.Replace("[User Name]", userName)
                .Replace("[Championship Name]", championshipName)
                .Replace("[Winner Name]", winnerName);

            await SendEmailAsync(userEmail, "Championship has finished", body);
        }
    }
}
