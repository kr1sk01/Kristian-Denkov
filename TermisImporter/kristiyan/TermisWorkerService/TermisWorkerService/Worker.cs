using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace TermisWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly AppSettings _appSettings;
        private readonly EmailSettings _emailSettings;
        private readonly ColumnIndexes _columnIndexes;
        private readonly IServiceScopeFactory _scopeFactory;
        private FileSystemWatcher watcher;
        private static readonly string logDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string logFileName;
        private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private static Master masterToAdd = new Master();
        private static bool first = true;
        private static List<CsvData> data = new();
        private readonly CsvContext _context;

        public Worker(IOptions<AppSettings> appSettings, IOptions<EmailSettings> emailSettings, IOptions<ColumnIndexes> columnIndexes, IServiceScopeFactory scopeFactory, CsvContext _context)
        {
            _appSettings = appSettings.Value;
            _emailSettings = emailSettings.Value;
            _columnIndexes = columnIndexes.Value;
            _scopeFactory = scopeFactory;
            this._context = _context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.Register(() => cancellationTokenSource.Cancel());

                EnsureCreatedDb();

                watcher = new FileSystemWatcher
                {
                    Path = _appSettings.FolderPath,
                    Filter = "*.csv",
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };
                watcher.Created += OnFileCreated;
                watcher.EnableRaisingEvents = true;

                WriteLog("Started service and watching directory: " + _appSettings.FolderPath);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                WriteLog($"" + ex);
            }
        }

        private void EnsureCreatedDb()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CsvContext>();
                context.Database.EnsureCreated();
                WriteLog("Database created successfully.");
            }
        }
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string filePath = e.FullPath;
            string? fileName = e.Name;
            WriteLog($"New .csv file created: {filePath}");

            bool isSuccess = false;
            using (var scope = _scopeFactory.CreateScope())
            {
                isSuccess = ReadCsvAndInsertToDatabase(filePath);
            }
            if (isSuccess)
            {
                SendEmail(filePath, true);
                MoveFileToFolder(filePath, _appSettings.SucceededFolderPath);
                first = true;
            }
            else
            {
                SendEmail(filePath, false);
                MoveFileToFolder(filePath, _appSettings.FailedFolderPath);
                first = true;
            }
            stopwatch.Stop();
        }
        private bool ReadCsvAndInsertToDatabase(string filePath)
        {
            bool isSuccess = true;
            try
            {
                List<int> indexes = new List<int>
                {
                    _columnIndexes.MonthColumnIndex,
                    _columnIndexes.DateColumnIndex,
                    _columnIndexes.HourColumnIndex,
                    _columnIndexes.EarthTempColumnIndex,
                    _columnIndexes.AirTempColumnIndex,
                };
                if (HasDuplicates(indexes))
                {
                    WriteLog("You have setup 2 different properies in 1 colums, check the config file!");
                    return false;
                }
                if (_columnIndexes.MonthColumnIndex <= -1 || _columnIndexes.DateColumnIndex <= -1 || _columnIndexes.HourColumnIndex <= -1)
                {
                    WriteLog("Error reading CSV file please check the config !");
                    return false;
                }
                data = _context.CsvData.ToList();
                logFileName = $"{DateTime.UtcNow.ToString("dd_MM_yyyy HH_mm")}.log";
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (columns.Length <= 3 ||
                            string.IsNullOrWhiteSpace(columns[0]) ||
                            string.IsNullOrWhiteSpace(columns[1]) ||
                            string.IsNullOrWhiteSpace(columns[2]) ||
                            string.IsNullOrWhiteSpace(columns[3]))
                        {
                            WriteLog($"Invalid line format or missing values: {line}");
                            isSuccess = false;
                            continue;
                        }

                        if (first)
                        {
                            masterToAdd = new Master
                            {
                                ImportDate = DateTime.Now,
                                ForecastDate = new DateTime(DateTime.UtcNow.Year, int.Parse(columns[0]), int.Parse(columns[1])),
                            };
                            _context.Masters.Add(masterToAdd);
                            first = false;
                        }
                        CsvData csvData = new CsvData
                        {
                            Month = columns[_columnIndexes.MonthColumnIndex],
                            Day = columns[_columnIndexes.DateColumnIndex],
                            Hour = columns[_columnIndexes.HourColumnIndex],
                            EarthTemperature = _columnIndexes.EarthTempColumnIndex > -1 ? double.Parse(columns[_columnIndexes.EarthTempColumnIndex], NumberStyles.Any, CultureInfo.InvariantCulture) : 0,
                            AirTemperature = _columnIndexes.AirTempColumnIndex > -1 ? double.Parse(columns[_columnIndexes.AirTempColumnIndex], NumberStyles.Any, CultureInfo.InvariantCulture) : 0,
                            Master = masterToAdd
                        };

                        InsertDataToDatabase(columns[_columnIndexes.MonthColumnIndex], columns[_columnIndexes.DateColumnIndex], columns[_columnIndexes.HourColumnIndex], csvData);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error reading CSV file: " + ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        private void SendEmail(string filePath, bool succeeded)
        {
            try
            {
                string subject;
                string body;
                if (succeeded)
                {
                    body = $"The CSV file '{Path.GetFileName(filePath)}' was successfully processed and moved to the succeeded_to_process folder.";
                    subject = "CSV File Processing Succeeded!";
                }
                else
                {
                    body = $"The CSV file '{Path.GetFileName(filePath)}' couldn't be inserted into database.";
                    subject = "CSV File Processing Failed!";
                }

                using var client = new SmtpClient();

                // Enable SSL/TLS for secure connection
                client.Connect(_emailSettings.Host, _emailSettings.Port, _emailSettings.EnableSsl);

                // Authenticate if using a password-protected email account
                if (!string.IsNullOrEmpty(_emailSettings.FromAddress))
                {
                    client.Authenticate(_emailSettings.FromAddress, _emailSettings.FromPassword);
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Termis", _emailSettings.FromAddress));
                message.To.Add(new MailboxAddress("Recipient", _emailSettings.ToAddress));
                message.Subject = subject;

                // Set the message body (can be plain text or HTML)
                message.Body = new TextPart("html") { Text = body }; // Assuming HTML template

                client.SendAsync(message);
                WriteLog("Email sent for file: " + filePath);
            }
            catch (Exception ex)
            {
                WriteLog("Error sending email: " + ex);
            }
        }
        private bool InsertDataToDatabase(string month, string day, string hour, CsvData csvData)
        {
            var test = data.FirstOrDefault(x => x.Day == csvData.Day && x.Month == csvData.Month && x.Hour == csvData.Hour);
            if (test != null)
            {
                test.EarthTemperature = csvData.EarthTemperature;
                test.AirTemperature = csvData.AirTemperature;
                test.Master = masterToAdd;
            }
            else
            {
                _context.CsvData.Add(csvData);
            }

            // Add the CsvData entry to the _context
            WriteLog($"Data inserted to database: {month}, {day}, {hour}, {csvData.EarthTemperature},{csvData.AirTemperature}");
            return true;
        }
        private void MoveFileToFolder(string filePath, string destinationFolderPath)
        {
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
            string destinationFilePath = Path.Combine(destinationFolderPath, Path.GetFileName(filePath));
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath); // Remove if already exists
            }
            File.Move(filePath, destinationFilePath);
            WriteLog($"File moved to {destinationFolderPath}");
        }
        private void WriteLog(string logMessage)
        {
            // Ensure the directory exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Write the log message to the file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {logMessage}");
            }

            // Additionally, write to the Event Viewer
            Console.WriteLine(logMessage);
        }
        public bool HasDuplicates(List<int> numbers)
        {
            HashSet<int> seenNumbers = new HashSet<int>();
            foreach (int number in numbers)
            {
                if (seenNumbers.Contains(number))
                {
                    return true; // Found a duplicate
                }
                seenNumbers.Add(number);
            }
            return false; // No duplicates found
        }
    }
}
