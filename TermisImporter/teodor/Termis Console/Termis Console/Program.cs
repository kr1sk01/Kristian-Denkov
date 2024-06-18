using MimeKit;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Termis_Console
{
    public class Program
    {
        private static string csvDirectory = @"C:\TermisImporterFiles\CsvFiles";
        private static string processedDirectory = @"C:\TermisImporterFiles\ProcessedFiles";
        private static string errorDirectory = @"C:\TermisImporterFiles\ErrorFiles";

        private static char csvSeparator = ' ';

        private static string emailHost = "smtp.gmail.com";
        private static int emailPort = 465;
        private static string emailApplicationName = "ChampionshipMaster";
        private static string emailUsername = "championshipmaster.a1@gmail.com";
        private static string emailPassword = "fjen jevt wfgh miib";
        private static bool emailEnableSsl = true;

        private static string toEmail = "sitikom2007@gmail.com";

        static void Main(string[] args)
        {
            InitializeService(out ServiceDbContext context);
            ProcessCsvFiles(context);
        }

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void InitializeService(out ServiceDbContext context)
        {
            CreateDirectoryIfNotExists(csvDirectory);
            CreateDirectoryIfNotExists(processedDirectory);
            CreateDirectoryIfNotExists(errorDirectory);

            context = new ServiceDbContext();
            context.Database.EnsureCreated();
        }

        private static void ProcessCsvFiles(ServiceDbContext _context)
        {
            var csvFiles = Directory.GetFiles(csvDirectory, "*.csv");

            foreach (var csvFile in csvFiles)
            {
                int row = 0;
                bool isSuccess = true;
                string errorMessage = string.Empty;

                try
                {
                    var master = new Master
                    {
                        Date = DateTime.Now
                    };

                    _context.Masters.Add(master);
                    _context.SaveChanges();

                    var lines = File.ReadAllLines(csvFile);

                    foreach (var line in lines)
                    {
                        row++;
                        string[] values;
                        values = line.TrimStart().Split(csvSeparator, StringSplitOptions.RemoveEmptyEntries);

                        var parsingResponse = ParseDetail(values, row, out Detail? detail);

                        if (parsingResponse.IsSuccess)
                        {
                            if (master.Details.Count == 0)
                            {
                                master.ForecastDate = new DateTime(DateTime.Now.Year, detail!.Month, detail.Day, detail.Hour, 0, 0);
                            }

                            var isDetailToUpdate = IsDetailToUpdate(_context, detail!, out int detailId);

                            if (isDetailToUpdate)
                            {
                                Detail detailToUpdate = _context.Details.First(x => x.Id == detailId);
                                detailToUpdate.Master = master;
                                _context.Entry(detailToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _context.SaveChanges();
                            }
                            else
                            {
                                detail!.Master = master;
                                _context.Details.Add(detail);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = parsingResponse.ErrorMessage!;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    isSuccess = false;
                }

                if (isSuccess)
                {
                    var processedFileName = Path.Combine(processedDirectory, Path.GetFileName(csvFile));
                    File.Move(csvFile, processedFileName);
                }
                else
                {
                    HandleError(errorMessage, csvFile);
                }
            }
        }

        private static void HandleError(string message, string csvFile)
        {
            var errorFileName = Path.Combine(errorDirectory, Path.GetFileNameWithoutExtension(csvFile) + ".err");
            File.WriteAllText(errorFileName, message);

            var unreadFileName = Path.Combine(errorDirectory, Path.GetFileName(csvFile));
            File.Move(csvFile, unreadFileName);

            SendErrorEmail(toEmail, unreadFileName, message);
        }

        private static DetailParseResponse ParseDetail(string[] values, int row, out Detail? detail)
        {
            var response = new DetailParseResponse
            {
                IsSuccess = true
            };

            detail = null;

            if (values.Length != 4)
            {
                string errorMessage = $"Invalid data format in row [{row}]. This row does not contain the necessary amount of values.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }

            //Parse Month value
            if (!int.TryParse(values[0], out int month))
            {
                string errorMessage = $"Invalid data format in row [{row}]. Month value could not be parsed.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }
            if (month > 12)
            {
                string errorMessage = $"Invalid data format in row [{row}]. Month value is not valid.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }

            //Parse Day value
            if (!int.TryParse(values[1], out int day))
            {
                string errorMessage = $"Invalid data format in row [{row}]. Day value could not be parsed.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }
            if (day > 31)
            {
                string errorMessage = $"Invalid data format in row [{row}]. Day value is not valid.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }

            //Parse Hour value
            if (!int.TryParse(values[2], out int hour))
            {
                string errorMessage = $"Invalid data format in row [{row}]. Hour value could not be parsed.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }
            if (hour > 23)
            {
                string errorMessage = $"Invalid data format in row [{row}]. Hour value is not valid.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }

            //Parse Temperature value
            if (!double.TryParse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double temp))
            {
                string errorMessage = $"Invalid data format in row [{row}]. Temperature value is not valid.";
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                return response;
            }

            //Parse Temperature value
            //if (!double.TryParse(values[4], out double soilTemp))
            //{
            //    string errorMessage = $"Invalid data format in row [{row}]. Temperature value is not valid.";
            //    response.isSuccess = false;
            //    response.ErrorMessage = errorMessage;
            //    return response;
            //}

            detail = new Detail
            {
                Month = month,
                Day = day,
                Hour = hour,
                Temp = temp
            };

            return response;
        }

        private static bool IsDetailToUpdate(ServiceDbContext _context, Detail detail, out int detailId)
        {
            var detailToUpdate = _context.Details.FirstOrDefault(x =>
                    x.Month == detail.Month &&
                    x.Day == detail.Day &&
                    x.Hour == detail.Hour);

            if (detailToUpdate != null)
            {
                detailId = detailToUpdate.Id;
                return true;
            }
            else
            {
                detailId = 0;
                return false;
            }
        }

        private static void SendErrorEmail(string toEmail, string csvFile, string errorMessage)
        {
            string template;
            using (var reader = new StreamReader("ErrorTemplate.html"))
            {
                template = reader.ReadToEnd();
            }

            var body = template.Replace("[Csv File]", csvFile)
                .Replace("[Error Message]", errorMessage);

            SendEmail(toEmail, "Error in NIMH .csv file", body);
        }

        private static void SendEmail(string toEmail, string subject, string body)
        {
            /*using (var client = new SmtpClient())
            {
                 Enable SSL/TLS for secure connection
                client.Connect(emailHost, emailPort, emailEnableSsl);

                 Authenticate if using a password-protected email account
                client.Authenticate(emailUsername, emailPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailApplicationName, emailUsername));
                message.To.Add(new MailboxAddress("Recipient", toEmail));
                message.Subject = subject;

                 Set the message body (can be plain text or HTML)
                message.Body = new TextPart("html") { Text = body };  Assuming HTML template

                client.Send(message);
            }*/

            SmtpClient smtpClient = new SmtpClient(emailHost) // Replace with your SMTP server
            {
                Port = 587, // Common port for TLS
                Credentials = new NetworkCredential(emailUsername, emailPassword),
                EnableSsl = emailEnableSsl,
            };

            // Create the email message
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(emailUsername),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            // Send the email
            smtpClient.Send(mailMessage);
        }
    }
}
