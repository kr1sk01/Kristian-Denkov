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
using static System.Formats.Asn1.AsnWriter;
using System.Buffers;
using System.Net.NetworkInformation;

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
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Master masterToAdd = new Master();
        private static bool first = true;
        private static List<CsvData> data = new();
        private CsvContext _context;
        private string importFileName = "";
        int date, month;
        private enum Status
        {
            Partial,
            Failed,
            Success
        }

        private static Status operationStatus;

        public Worker(IOptions<AppSettings> appSettings, IOptions<EmailSettings> emailSettings, IOptions<ColumnIndexes> columnIndexes, IServiceScopeFactory scopeFactory)
        {
            _appSettings = appSettings.Value;
            _emailSettings = emailSettings.Value;
            _columnIndexes = columnIndexes.Value;
            _scopeFactory = scopeFactory;
            _context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<CsvContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

            WriteLog("Started service and watching directory: " + _appSettings.FolderPath, Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private void EnsureCreatedDb()
        {
            _context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully.");
        }
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string filePath = e.FullPath;
                importFileName = e.Name ?? "error_reading_imput_file_name";
                importFileName = importFileName.Replace(".csv", ".err");
                Console.WriteLine($"File name is {importFileName}");
                WriteLog($"New .csv file created: {filePath}", Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
                bool isSuccess = false;

                isSuccess = ReadCsvAndInsertToDatabase(filePath);

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
                WriteLog($"Elapsed time for processing file '{e.FullPath}': {stopwatch.ElapsedMilliseconds} ms", Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
            }
            catch (Exception ex)
            {
                masterToAdd = new Master
                {
                    ImportDate = DateTime.Now,
                    Status = "Failed"
                };
                _context.Masters.Add(masterToAdd);
                _context.SaveChanges();
                WriteLog($"" + ex, Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
            }
        }
        private bool ReadCsvAndInsertToDatabase(string filePath)
        {
            operationStatus = Status.Success;
            int lineCounter = 0;

            bool isSuccess = true;

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
                WriteLog("You have setup 2 different properies in 1 colums, check the config file!", Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
                return false;
            }
            if (_columnIndexes.MonthColumnIndex <= -1 || _columnIndexes.DateColumnIndex <= -1 || _columnIndexes.HourColumnIndex <= -1)
            {
                //WriteLog("Error reading CSV file please check the config !", Path.Combine(logDirectory, logFileName));
                return false;
            }
            data = _context.CsvData.ToList();
            using (StreamReader reader = new StreamReader(filePath))
            {
                operationStatus = Status.Success;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    lineCounter++;
                    if (columns.Length <= 3 ||
                        string.IsNullOrWhiteSpace(columns[0]) ||
                        string.IsNullOrWhiteSpace(columns[1]) ||
                        string.IsNullOrWhiteSpace(columns[2]) ||
                        string.IsNullOrWhiteSpace(columns[3]))
                    {

                        operationStatus = Status.Failed;
                        WriteLog($"Invalid line format or missing values on line {lineCounter}: {line}", Path.Combine(logDirectory, importFileName));
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
                        EarthTemperature = _columnIndexes.EarthTempColumnIndex > -1 ? double.Parse(columns[_columnIndexes.EarthTempColumnIndex], NumberStyles.Any, CultureInfo.InvariantCulture):0,
                        AirTemperature = _columnIndexes.AirTempColumnIndex > -1 ? double.Parse(columns[_columnIndexes.AirTempColumnIndex], NumberStyles.Any, CultureInfo.InvariantCulture):0,
                        Master = masterToAdd
                    };
                    InsertDataToDatabase(columns[_columnIndexes.MonthColumnIndex], columns[_columnIndexes.DateColumnIndex], columns[_columnIndexes.HourColumnIndex], csvData);
                    if (operationStatus == Status.Failed)
                    {
                        operationStatus = Status.Partial;
                    }
                }
                switch (operationStatus)
                {
                    case Status.Partial:
                        masterToAdd.Status = "Partial";
                        break;
                    case Status.Failed:
                        masterToAdd = new Master
                        {
                            ImportDate = DateTime.Now,                          
                        };
                        _context.Masters.Add(masterToAdd);
                        masterToAdd.Status = "Failed";
                        break;
                    case Status.Success:
                        masterToAdd.Status = "Success";
                        break;
                }
                _context.SaveChanges();
                return isSuccess;
            }

        }
        private void SendEmail(string filePath, bool succeeded)
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
            WriteLog("Email sent for file: " + filePath, Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
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

            // Add the CsvData entry to the context
            WriteLog($"Data inserted to database: {month}, {day}, {hour}, {csvData.EarthTemperature},{csvData.AirTemperature}", Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));

            
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
            WriteLog($"File moved to {destinationFolderPath}", Path.Combine(logDirectory, $"{DateTime.UtcNow.ToString("dd_MM_yyyy")}.log"));
        }
        private void WriteLog(string logMessage, string logFilePath)
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
