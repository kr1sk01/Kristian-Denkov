using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.ServiceProcess;

namespace Service
{
    public partial class Service1 : ServiceBase
    {
        private FileSystemWatcher watcher;
        private string folderPath = @"C:\Users\cross\Documents"; // Change this to your desired folder path
        private string failedFolderPath = @"C:\Users\cross\Documents\failed_to_process"; // Change this to your desired failed folder path
        private string succeededFolderPath = @"C:\Users\cross\Documents\succeeded_to_process"; // Change this to your desired succeeded folder path

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
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

        protected override void OnStop()
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

        private void OnFileCreated(object sender, FileSystemEventArgs e)
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
                    MoveFileToFolder(filePath, succeededFolderPath);
                }
                else
                {
                    MoveFileToFolder(filePath, failedFolderPath);
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error on file created: " + ex.Message);
            }
        }

        private bool ReadCsvAndInsertToDatabase(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var columns = line.Split(' ');
                        if (columns.Length == 4 && !string.IsNullOrWhiteSpace(columns[0]) && !string.IsNullOrWhiteSpace(columns[1]) && !string.IsNullOrWhiteSpace(columns[2]) && !string.IsNullOrWhiteSpace(columns[3]))
                        {
                            var result = InsertDataToDatabase(columns[0], columns[1], columns[2], columns[3]);
                            if (!result)
                                return false;
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
        private void SendSuccessEmail(string filePath)
        {
            try
            {
                var fromAddress = new MailAddress("your_email@example.com", "Your Name");
                var toAddress = new MailAddress("recipient@example.com", "Recipient Name");
                const string fromPassword = "your_email_password"; // Update with your email password
                const string subject = "CSV File Processing Succeeded";
                string body = $"The CSV file '{Path.GetFileName(filePath)}' was successfully processed and moved to the succeeded_to_process folder.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.example.com", // Update with your SMTP server
                    Port = 587, // Update with your SMTP port
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                WriteLog("Success email sent for file: " + filePath);
            }
            catch (Exception ex)
            {
                WriteLog("Error sending email: " + ex.Message);
            }
        }
        private bool InsertDataToDatabase(string col1, string col2, string col3, string col4)
        {
            string connectionString = "CSVConnectionString"; // Replace with your actual connection string

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO CsvData (Column1, Column2, Column3, Column4) VALUES (@col1, @col2, @col3, @col4)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@col1", col1);
                        command.Parameters.AddWithValue("@col2", col2);
                        command.Parameters.AddWithValue("@col3", col3);
                        command.Parameters.AddWithValue("@col4", col4);

                        command.ExecuteNonQuery();
                    }
                    
                }
                WriteLog($"Data inserted to database: {col1}, {col2}, {col3}, {col4}");
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
                WriteLog($"File moved to {destinationFolderPath}: {filePath}");
            }
            catch (Exception ex)
            {
                WriteLog($"Error moving file to {destinationFolderPath}: " + ex.Message);
            }
        }

        // Define the directory and file name for the log
        private static readonly string logDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string logFileName = "app.log";
        private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);

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
                EventLog.WriteEntry("Service1", logMessage, EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during file operations
                // Log the error to Event Viewer
                EventLog.WriteEntry("Service1", $"Error writing log: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
