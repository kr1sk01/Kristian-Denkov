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
        private static int emailPort = 587;
        private static string emailApplicationName = "ChampionshipMaster";
        private static string emailUsername = "championshipmaster.a1@gmail.com";
        private static string emailPassword = "fjen jevt wfgh miib";
        private static bool emailEnableSsl = true;

        private static string toEmail = "sitikom2007@gmail.com";

        private static int monthColumnIndex = 0;
        private static int dayColumnIndex = 1;
        private static int hourhColumnIndex = 2;
        private static int tempColumnIndex = 3;
        private static int soilTempColumnIndex = 4;
        private static bool hasSoilTempColumn = true;

        private static string logFilePath = @"C:\TermisImporterFiles\termisErrorLog.txt";

        static void Main(string[] args)
        {
            try
            {
                InitializeService();
                ProcessCsvFiles();
            }
            catch (Exception ex)
            {
                LogError("General service error.", ex);
            }
        }

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void InitializeService()
        {
            try
            {
                CreateDirectoryIfNotExists(csvDirectory);
                CreateDirectoryIfNotExists(processedDirectory);
                CreateDirectoryIfNotExists(errorDirectory);
                CreateDirectoryIfNotExists(Path.GetDirectoryName(logFilePath));

                var context = new ServiceDbContext();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                LogError("Error while initializing service.", ex);
            }
        }

        private static void ProcessCsvFiles()
        {
            var _context = new ServiceDbContext();
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
            try
            {
                var errorFileName = Path.Combine(errorDirectory, Path.GetFileNameWithoutExtension(csvFile) + ".err");
                File.WriteAllText(errorFileName, message);

                var unreadFileName = Path.Combine(errorDirectory, Path.GetFileName(csvFile));
                File.Move(csvFile, unreadFileName);

                SendErrorEmail(toEmail, unreadFileName, message);
            }
            catch (Exception ex)
            {
                LogError("Error moving files.", ex);
            }
        }

        private static DetailParseResponse ParseDetail(string[] values, int row, out Detail? detail)
        {
            var response = new DetailParseResponse
            {
                IsSuccess = true
            };

            detail = null;

            try
            {
                if ((hasSoilTempColumn && values.Length != 5) || (!hasSoilTempColumn && values.Length != 4))
                {
                    string errorMessage = $"Invalid data format in row [{row}]. This row does not contain the necessary amount of columns.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }

                //Parse Month value
                if (!int.TryParse(values[monthColumnIndex], out int month))
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
                if (!int.TryParse(values[dayColumnIndex], out int day))
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
                if (!int.TryParse(values[hourhColumnIndex], out int hour))
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
                if (!double.TryParse(values[tempColumnIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out double temp))
                {
                    string errorMessage = $"Invalid data format in row [{row}]. Temperature value is not valid.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }

                //Parse Soil Temperature value
                double soilTemp = 0;
                if (hasSoilTempColumn)
                {
                    if (!double.TryParse(values[soilTempColumnIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out soilTemp))
                    {
                        string errorMessage = $"Invalid data format in row [{row}]. Soil temperature value is not valid.";
                        response.IsSuccess = false;
                        response.ErrorMessage = errorMessage;
                        return response;
                    } 
                }

                detail = new Detail
                {
                    Month = month,
                    Day = day,
                    Hour = hour,
                    Temp = temp,
                    SoilTemp = hasSoilTempColumn ? soilTemp : null
                };
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"Data parsing error in row [{row}]. {ex.Message}";
            }

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
            try
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
            catch (Exception ex)
            {
                LogError("Error sending email.", ex);
            }
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
                Port = emailPort, // Common port for TLS
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

        private static void LogError(string message, Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - ERROR: {message}");
                    writer.WriteLine($"Exception: {ex.Message}");
                    writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                    writer.WriteLine();
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Failed to log error: {logEx.Message}");
            }
        }
    }
}
