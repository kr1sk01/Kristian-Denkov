namespace ServiceConsoleApp
{
    using System;
    using System.IO;
    using System.Net;
    using MailKit.Net.Smtp;
    using System.Threading;
    using System.Data.SqlClient;
    using ServiceConsoleApp;
    using System.Globalization;
    using Microsoft.EntityFrameworkCore;
    using System.Diagnostics.Metrics;
    using Org.BouncyCastle.Crypto.Macs;
    using MimeKit;
    internal class Program
    {
        private static FileSystemWatcher watcher = default!;

        private static string folderPath = @"C:\Users\a1bg535412\Documents"; // Change this to your desired folder path
        private static string failedFolderPath = @"C:\Users\a1bg535412\Documents\failed_to_process"; // Change this to your desired failed folder path
        private static string succeededFolderPath = @"C:\Users\a1bg535412\Documents\succeeded_to_process"; // Change this to your desired succeeded folder path

        private static readonly string logDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string logFileName = "app.log";
        private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);

        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private static Master masterToAdd = new Master();

        private static bool first = true;
        private static CsvContext context = new();

        private static string fromAddress = "championshipmaster.a1@gmail.com";
        private static string toAddress = "cross.2001@abv.bg";
        private static string host = "smtp.gmail.com"; // Update with your SMTP server
        private static int port = 465; // Update with your SMTP port

        private static bool enableSsl = true;
        private static string fromPassword = "fjen jevt wfgh miib"; // Update with your email password

        static void Main(string[] args)
        {
            EnsureCreatedDb();
            context = new CsvContext();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            try
            {
                OnStart(args);
                cancellationTokenSource.Token.WaitHandle.WaitOne();
            }
            finally
            {
                OnStop();
            }
        }
        private static void EnsureCreatedDb()
        {
            try
            {
                using (var context = new CsvContext())
                {
                    // Ensure the database is created
                    context.Database.EnsureCreated();

                    Console.WriteLine("Database created successfully.");
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private static void OnStart(string[] args)
        {
            try
            {
                // Initialize and configure FileSystemWatcher
                watcher = new FileSystemWatcher
                {
                    Path = folderPath,
                    Filter = "*.csv"
                };
                watcher.Created += OnFileCreated;

                // Start monitoring
                watcher.EnableRaisingEvents = true;
                WriteLog("Started service and watching directory: " + folderPath);
            }
            catch (Exception ex)
            {
                WriteLog("Error on start: " + ex.Message);
            }
        }
        private static void OnStop()
        {
            try
            {
                // Stop monitoring
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                WriteLog("Stopped service!");
            }
            catch (Exception ex)
            {
                WriteLog("Error on stop: " + ex.Message);
            }
        }
        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                // Handle new .csv file created
                string filePath = e.FullPath;
                WriteLog($"New .csv file created: {filePath}");

                // Read the CSV file and insert its contents into the database
                bool isSuccess = ReadCsvAndInsertToDatabase(filePath);

                // Move the file based on success or failure
                if (isSuccess)
                {
                    SendEmail(filePath, true);
                    MoveFileToFolder(filePath, succeededFolderPath);
                }
                else
                {
                    SendEmail(filePath, false);
                    MoveFileToFolder(filePath, failedFolderPath);
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error on file created: " + ex.Message);
            }
        }
        private static bool ReadCsvAndInsertToDatabase(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        // Split the line by spaces, removing empty entries
                        var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (columns.Length == 4 &&
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
                                context.Masters.Add(masterToAdd);
                                context.SaveChanges();
                                first = false;

                            }
                            InsertDataToDatabase(columns[0], columns[1], columns[2], columns[3], context);
                        }
                        else
                        {
                            WriteLog($"Invalid line format or missing values: {line}");
                            return false; // Indicate failure due to invalid line
                        }
                    }

                }
                return true; // Indicate success if all lines are valid
            }
            catch (Exception ex)
            {
                WriteLog("Error reading CSV file: " + ex.Message);
                return false; // Indicate failure due to exception
            }
        }
        private static void SendEmail(string filePath, bool succeeded)
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
                client.Connect(host, port, enableSsl);

                // Authenticate if using a password-protected email account
                if (!string.IsNullOrEmpty(fromAddress))
                {
                    client.Authenticate(fromAddress, fromPassword);
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Termis", fromAddress));
                message.To.Add(new MailboxAddress("Recipient", toAddress));
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
        private static bool InsertDataToDatabase(string col1, string col2, string col3, string col4, CsvContext context)
        {
            try
            {
                // Create a new CsvData entry and link it to the Master entry
                var csvData = new CsvData
                {
                    Month = col1, // Example mapping, adjust based on your CSV structure
                    Day = col2,
                    Hour = col3,
                    Parameter = double.Parse(col4, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                    Master = masterToAdd // Link the CsvData to the Master entry
                };
                var test = context.CsvData.FirstOrDefault(x => x.Day == csvData.Day && x.Month == csvData.Month && x.Hour == csvData.Hour);
                if (test != null)
                {
                    test.Master = masterToAdd;
                    context.Entry(test).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    context.CsvData.Add(csvData);
                    // Save changes to persist both Master and CsvData entries
                    context.SaveChanges();
                }
                // Add the CsvData entry to the context
                WriteLog($"Data inserted to database: {col1}, {col2}, {col3}, {col4}");
                return true;
            }
            catch (Exception ex)
            {
                WriteLog("Error inserting data to database: " + ex.Message);
                return false;
            }
        }
        private static void MoveFileToFolder(string filePath, string destinationFolderPath)
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
                WriteLog($"File moved to {destinationFolderPath}: {filePath}");
            }
            catch (Exception ex)
            {
                WriteLog($"Error moving file to {destinationFolderPath}: " + ex.Message);
            }
        }
        private static void WriteLog(string logMessage)
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
    }
}