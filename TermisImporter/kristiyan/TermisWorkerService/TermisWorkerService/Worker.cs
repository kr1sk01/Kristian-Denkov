using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private static readonly string logFileName = "app.log";
        private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private static Master masterToAdd = new Master();
        private static bool first = true;

        public Worker(IOptions<AppSettings> appSettings, IOptions<EmailSettings> emailSettings, IOptions<ColumnIndexes> columnIndexes, IServiceScopeFactory scopeFactory)
        {
            _appSettings = appSettings.Value;
            _emailSettings = emailSettings.Value;
            _columnIndexes = columnIndexes.Value;
            _scopeFactory = scopeFactory;
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

            WriteLog("Started service and watching directory: " + _appSettings.FolderPath);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private void EnsureCreatedDb()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<CsvContext>();
                    context.Database.EnsureCreated();
                    Console.WriteLine("Database created successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    string filePath = e.FullPath;
                    WriteLog($"New .csv file created: {filePath}");

                    bool isSuccess = false;
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<CsvContext>();
                        isSuccess = ReadCsvAndInsertToDatabase(filePath, dbContext);
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
                }
                catch (Exception ex)
                {
                    WriteLog("Error on file created: " + ex.Message);
                }
            });
        }

        private bool ReadCsvAndInsertToDatabase(string filePath, CsvContext dbContext)
        {
            try
            {
                List<int> indexes =
                [
                    _columnIndexes.MonthColumnIndex,
                    _columnIndexes.DateColumnIndex,
                    _columnIndexes.HourColumnIndex,
                    _columnIndexes.EarthTempColumnIndex,
                    _columnIndexes.AirTempColumnIndex,
                ];
                if (HasDuplicates(indexes))
                {
                    WriteLog("You have setup 2 different properies in 1 colums, check the config file!");
                    return false;
                }

                if (_columnIndexes.MonthColumnIndex > -1 || _columnIndexes.DateColumnIndex > -1 || _columnIndexes.HourColumnIndex > -1)
                {

                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (columns.Length > 3 &&
                                !string.IsNullOrWhiteSpace(columns[0]) &&
                                !string.IsNullOrWhiteSpace(columns[1]) &&
                                !string.IsNullOrWhiteSpace(columns[2]) &&
                                !string.IsNullOrWhiteSpace(columns[3]))
                            {
                                if (first)
                                {
                                    masterToAdd = new Master
                                    {
                                        ImportDate = DateTime.Now,
                                        ForecastDate = new DateTime(DateTime.UtcNow.Year, int.Parse(columns[0]), int.Parse(columns[1])),
                                    };
                                    dbContext.Masters.Add(masterToAdd);
                                    dbContext.SaveChanges();
                                    first = false;
                                }
                                CsvData csvData = new CsvData
                                {
                                    Month = columns[_columnIndexes.MonthColumnIndex],
                                    Day = columns[_columnIndexes.DateColumnIndex],
                                    Hour = columns[_columnIndexes.HourColumnIndex],
                                    EarthTemperature = _columnIndexes.EarthTempColumnIndex > -1 ? double.Parse(columns[_columnIndexes.EarthTempColumnIndex], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) : 0,
                                    AirTemperature = _columnIndexes.AirTempColumnIndex > -1 ? double.Parse(columns[_columnIndexes.AirTempColumnIndex], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) : 0,
                                    Master = masterToAdd
                                };

                                InsertDataToDatabase(columns[_columnIndexes.MonthColumnIndex], columns[_columnIndexes.DateColumnIndex], columns[_columnIndexes.HourColumnIndex], csvData, dbContext);
                            }
                            else
                            {
                                WriteLog($"Invalid line format or missing values: {line}");
                                return false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    WriteLog("Error reading CSV file please check the config !");
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error reading CSV file: " + ex.Message);
                return false;
            }
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

                client.Send(message);
                WriteLog("Success email sent for file: " + filePath);
            }
            catch (Exception ex)
            {
                WriteLog("Error sending email: " + ex.Message);
            }
        }

        private bool InsertDataToDatabase(string month, string day, string hour, CsvData csvData, CsvContext context)
        {
            try
            {
                var test = context.CsvData.FirstOrDefault(x => x.Day == csvData.Day && x.Month == csvData.Month && x.Hour == csvData.Hour);
                if (test != null)
                {
                    test.EarthTemperature = csvData.EarthTemperature;
                    test.AirTemperature = csvData.AirTemperature;
                    test.Master = masterToAdd;
                    context.Entry(test).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    context.CsvData.Add(csvData);
                    context.SaveChanges();
                }

                // Add the CsvData entry to the context
                WriteLog($"Data inserted to database: {month}, {day}, {hour}, {csvData.EarthTemperature},{csvData.AirTemperature}");
                return true;
            }
            catch (Exception ex)
            {
                WriteLog("Error inserting data to database: " + ex.Message);
                return false;
            }
        }

        private void MoveFileToFolder(string filePath, string destinationFolderPath)
        {
            try
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
            catch (Exception ex)
            {
                WriteLog($"Error moving file to {destinationFolderPath}: " + ex.Message);
            }
        }

        private void WriteLog(string logMessage)
        {
            try
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
            catch (Exception ex)
            {
                // Handle any exceptions that occur during file operations
                // Log the error to the console
                Console.WriteLine($"Error writing log: {ex.Message}");
            }
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
public class AppSettings
{
    public string FolderPath { get; set; }
    public string FailedFolderPath { get; set; }
    public string SucceededFolderPath { get; set; }

    // Parameterless constructor
    public AppSettings() { }
}

public class EmailSettings
{
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string FromPassword { get; set; }

    // Parameterless constructor
    public EmailSettings() { }
}

public class ColumnIndexes
{
    public int MonthColumnIndex { get; set; } = -1;
    public int DateColumnIndex { get; set; } = -1;
    public int HourColumnIndex { get; set; } = -1;
    public int EarthTempColumnIndex { get; set; } = -1;
    public int AirTempColumnIndex { get; set; } = -1;

    // Parameterless constructor
    public ColumnIndexes() { }
}
