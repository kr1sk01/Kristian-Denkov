using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Termis_Service.Models;

namespace Termis_Service
{
    public partial class TermisImporter : ServiceBase
    {
        public TermisImporter()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                InitializeService();

                watcher = new FileSystemWatcher
                {
                    Path = csvDirectory,
                    Filter = "*.csv"
                };
                watcher.Created += OnCsvAdded;

                watcher.EnableRaisingEvents = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void InitializeService()
        {
            try
            {
                GetConfigSettings();

                CreateDirectoryIfNotExists(csvDirectory);
                CreateDirectoryIfNotExists(processedDirectory);
                CreateDirectoryIfNotExists(errorDirectory);

                EnsureDatabaseExists();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void EnsureDatabaseExists()
        {
            var db = new ServiceDbContext();
            db.Database.EnsureCreated();
        }

        void OnCsvAdded(object sender, FileSystemEventArgs e)
        {
            var _context = new ServiceDbContext();
            string csvFile = e.FullPath;
            string[] lines = File.ReadAllLines(csvFile);

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

                foreach (var line in lines)
                {
                    row++;
                    string[] values;
                    values = line.TrimStart().Split(new char[] { csvSeparator }, StringSplitOptions.RemoveEmptyEntries);

                    var parsingResponse = ParseDetail(values, row, out Detail detail);

                    if (parsingResponse.IsSuccess)
                    {
                        if (master.Details.Count == 0)
                        {
                            master.ForecastDate = new DateTime(DateTime.Now.Year, detail.Month, detail.Day, detail.Hour, 0, 0);
                        }

                        var isDetailToUpdate = IsDetailToUpdate(_context, detail, out int detailId);

                        if (isDetailToUpdate)
                        {
                            Detail detailToUpdate = _context.Details.First(x => x.Id == detailId);
                            detailToUpdate.Master = master;
                            _context.Entry(detailToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _context.SaveChanges();
                        }
                        else
                        {
                            detail.Master = master;
                            _context.Details.Add(detail);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = parsingResponse.ErrorMessage;
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
                HandleCsvError(errorMessage, csvFile);
            }
        }

        void HandleCsvError(string message, string csvFile)
        {
            var errorFileName = Path.Combine(errorDirectory, Path.GetFileNameWithoutExtension(csvFile) + ".err");
            File.WriteAllText(errorFileName, message);

            var unreadFileName = Path.Combine(errorDirectory, Path.GetFileName(csvFile));
            File.Move(csvFile, unreadFileName);

            SendErrorEmail(toEmail, unreadFileName, message);
        }

        DetailParseResponse ParseDetail(string[] values, int row, out Detail detail)
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

        bool IsDetailToUpdate(ServiceDbContext _context, Detail detail, out int detailId)
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

        void SendErrorEmail(string toEmail, string csvFile, string errorMessage)
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

        void SendEmail(string toEmail, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient(host) // Replace with your SMTP server
            {
                Port = port, // Common port for TLS
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
            };

            // Create the email message
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            // Send the email
            smtpClient.Send(mailMessage);
        }

        void GetConfigSettings()
        {
            try
            {
                ServiceSettings serviceSettings = (ServiceSettings)ConfigurationManager.GetSection("serviceSettings");
                if (serviceSettings == null)
                {
                    throw new ConfigurationErrorsException("'serviceSettings' section is missing from config file");
                }

                csvDirectory = serviceSettings.CsvDirectory;
                processedDirectory = serviceSettings.ProcessedDirectory;
                errorDirectory = serviceSettings.ErrorDirectory;
                csvSeparator = serviceSettings.CsvSeparator;

                EmailSettings emailSettings = (EmailSettings)ConfigurationManager.GetSection("emailSettings");
                if (emailSettings == null)
                {
                    throw new ConfigurationErrorsException("'emailSettings' section is missing from config file");
                }

                host = emailSettings.Host;
                port = emailSettings.Port;
                username = emailSettings.Username;
                password = emailSettings.Password;
                enableSsl = emailSettings.EnableSsl;
                toEmail = emailSettings.ToEmail;
                displayName = emailSettings.DisplayName;
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine("Configuration error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);
                throw;
            }
        }

        private string csvDirectory;
        private string processedDirectory;
        private string errorDirectory;
        private char csvSeparator;

        private string host;
        private int port;
        private string username;
        private string password;
        private bool enableSsl;
        private string toEmail;
        private string displayName;

        private FileSystemWatcher watcher;
    }
}
