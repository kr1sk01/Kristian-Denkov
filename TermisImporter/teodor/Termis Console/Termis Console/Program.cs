using MimeKit;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Termis_Console
{
    public class Program
    {
        private static ServiceDbContext _context = new ServiceDbContext();
        private static string csvDirectory = @"C:\TermisImporterFiles\CsvFiles";
        private static string processedDirectory = @"C:\TermisImporterFiles\ProcessedFiles";
        private static string errorDirectory = @"C:\TermisImporterFiles\ErrorFiles";
        private static string logDirectory = @"C:\TermisImporterFiles\Logs";
        private static string csvSeparator = " ";

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
                CreateDirectoryIfNotExists(logDirectory);

                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                LogError("Error while initializing service.", ex);
            }
        }

        private static void ProcessCsvFiles()
        {
            var csvFiles = Directory.GetFiles(csvDirectory, "*.csv");

            foreach (var csvFile in csvFiles)
            {
                int row = 0;
                bool isSuccess = true;
                string errorMessage = string.Empty;
                List<string> errorList = [];

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

                            var isDetailToUpdate = IsDetailToUpdate(detail!, out Detail? detailToUpdate);

                            if (isDetailToUpdate)
                            {
                                detailToUpdate!.Master = master;
                                detailToUpdate!.Temp = detail!.Temp;
                                detailToUpdate!.SoilTemp = detail!.SoilTemp;
                                _context.Entry(detailToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                detail!.Master = master;
                                _context.Details.Add(detail);
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorList.Add(parsingResponse.ErrorMessage!);
                        }
                    }

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                    isSuccess = false;
                }

                if (isSuccess)
                {
                    var processedFileName = Path.Combine(processedDirectory, Path.GetFileName(csvFile));
                    File.Move(csvFile, processedFileName);
                }
                else
                {
                    HandleCsvError(errorList, csvFile);
                }
            }
        }

        private static void HandleCsvError(List<string> errros, string csvFile)
        {
            try
            {
                var errorFileName = Path.Combine(errorDirectory, Path.GetFileNameWithoutExtension(csvFile) + ".err");
                File.WriteAllLines(errorFileName, errros);

                var unreadFileName = Path.Combine(errorDirectory, Path.GetFileName(csvFile));
                File.Move(csvFile, unreadFileName);

                SendErrorEmail(toEmail, unreadFileName, errros);
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
                if (month > 12 || month < 1)
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
                if (day > 31 || day < 1 || (month == 2 && day > 29))
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
                if (hour > 23 || hour < 0)
                {
                    string errorMessage = $"Invalid data format in row [{row}]. Hour value is not valid.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }

                //Parse Temperature value
                if (!double.TryParse(values[tempColumnIndex].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double temp))
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
                    if (!double.TryParse(values[soilTempColumnIndex].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out soilTemp))
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

        private static bool IsDetailToUpdate(Detail detail, out Detail? detailToUpdate)
        {
            detailToUpdate = _context.Details.FirstOrDefault(x =>
                    x.Month == detail.Month &&
                    x.Day == detail.Day &&
                    x.Hour == detail.Hour);

            return (detailToUpdate != null);
        }

        private static void SendErrorEmail(string toEmail, string csvFile, List<string> errorList)
        {
            try
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

                SendEmail(toEmail, "Error in NIMH .csv file", body);
            }
            catch (Exception ex)
            {
                LogError("Error sending email.", ex);
            }
        }

        private static void SendEmail(string toEmail, string subject, string body)
        {
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
                string logFile = GetLogFileToWrite();

                using (StreamWriter writer = new StreamWriter(logFile, true))
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

        private static string GetLogFileToWrite()
        {
            string[] logFiles = Directory.GetFiles(logDirectory, ".txt");
            var today = DateOnly.FromDateTime(DateTime.Now).ToString();

            string? todayLog = logFiles.FirstOrDefault(x => x.Contains(today));

            if (todayLog != null)
            {
                return todayLog;
            }
            else
            {
                return $"{logDirectory}\\{today}-TermisErrorLog.txt";
            }
        }
    }
}
