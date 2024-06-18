namespace ServiceConsoleApp
{
    using System;
    using System.IO;
    using System.Net;
    using MailKit.Net.Smtp;
    using System.Threading;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using MimeKit;
    using System.Globalization;

    internal class Program
    {
        private static FileSystemWatcher watcher = default!;

        private static string folderPath = "";
        private static string failedFolderPath = "";
        private static string succeededFolderPath = "";

        private static readonly string logDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string logFileName = "app.log";
        private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);

        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private static Master masterToAdd = new Master();

        private static bool first = true;
        private static CsvContext dbcontext = new();

        private static string fromAddress = "";
        private static string toAddress = "";
        private static string host = "";
        private static int port;
        private static bool enableSsl;
        private static string fromPassword = "";


        private static int monthColumnIndex = 0;
        private static int dateColumnIndex = 0;
        private static int hourColumnIndex = 0;

        private static int earthTempColumnIndex = 0;
        private static int airTempColumnIndex = 0;





        static void Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
                try
                {
                    folderPath = config["AppSettings:FolderPath"]!;
                    failedFolderPath = config["AppSettings:FailedFolderPath"]!;
                    succeededFolderPath = config["AppSettings:SucceededFolderPath"]!;
                    fromAddress = config["AppSettings:FromAddress"]!;
                    toAddress = config["AppSettings:ToAddress"]!;
                    host = config["AppSettings:Host"]!;
                    port = int.Parse(config["AppSettings:Port"]!);
                    enableSsl = bool.Parse(config["AppSettings:EnableSsl"]!);
                    fromPassword = config["AppSettings:FromPassword"]!;
                    monthColumnIndex = int.Parse(config["AppSettings:MonthColumnIndex"]!);
                    dateColumnIndex = int.Parse(config["AppSettings:DateColumnIndex"]!);
                    hourColumnIndex = int.Parse(config["AppSettings:HourColumnIndex"]!);
                    earthTempColumnIndex = int.Parse(config["AppSettings:EarthTempColumnIndex"]!);
                    airTempColumnIndex = int.Parse(config["AppSettings:AirTempColumnIndex"]!);
                }
                catch (Exception ex)
                {
                    WriteLog("Couldn't load data from config" + ex);
                }
            }
            catch (Exception ex)
            {
                WriteLog("Couldn't load config" + ex);
            }

            EnsureCreatedDb();
            dbcontext = new CsvContext();

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
                if (monthColumnIndex != 0 || dateColumnIndex != 0 || hourColumnIndex != 0)
                {                
                    if (earthTempColumnIndex != 0 && airTempColumnIndex == 0)
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
                                        dbcontext.Masters.Add(masterToAdd);
                                        dbcontext.SaveChanges();
                                        first = false;
                                    }
                                    var month = columns[monthColumnIndex-1];
                                    var day = columns[dateColumnIndex - 1];
                                    var hour = columns[hourColumnIndex - 1];
                                    var earthTemperature = columns[earthTempColumnIndex - 1];

                                    InsertDataToDatabase(month, day, hour, true, false, dbcontext, earthTemp: earthTemperature);
                                }
                                else
                                {
                                    WriteLog($"Invalid line format or missing values: {line}");
                                    return false; // Indicate failure due to invalid line
                                }
                            }
                        }
                        return true; // Indicate success if all lines are valid
                    }else if(earthTempColumnIndex == 0 && airTempColumnIndex != 0)
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
                                        dbcontext.Masters.Add(masterToAdd);
                                        dbcontext.SaveChanges();
                                        first = false;
                                    }
                                    var month = columns[monthColumnIndex - 1];
                                    var day = columns[dateColumnIndex - 1];
                                    var hour = columns[hourColumnIndex - 1];
                                    var airTemperature = columns[airTempColumnIndex - 1];

                                    InsertDataToDatabase(month, day, hour, false, true, dbcontext, airTemp: airTemperature);
                                }
                                else
                                {
                                    WriteLog($"Invalid line format or missing values: {line}");
                                    return false; // Indicate failure due to invalid line
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                // Split the line by spaces, removing empty entries
                                var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                if (columns.Length == 5 &&
                                    !string.IsNullOrWhiteSpace(columns[0]) &&
                                    !string.IsNullOrWhiteSpace(columns[1]) &&
                                    !string.IsNullOrWhiteSpace(columns[2]) &&
                                    !string.IsNullOrWhiteSpace(columns[3]) &&
                                    !string.IsNullOrWhiteSpace(columns[4]))
                                {
                                    if (first)
                                    {
                                        masterToAdd = new Master
                                        {
                                            ImportDate = DateTime.Now,
                                            ForecastDate = new DateTime(DateTime.UtcNow.Year, int.Parse(columns[0]), int.Parse(columns[1])),
                                        };
                                        dbcontext.Masters.Add(masterToAdd);
                                        dbcontext.SaveChanges();
                                        first = false;
                                    }
                                    var month = columns[monthColumnIndex - 1];
                                    var day = columns[dateColumnIndex - 1];
                                    var hour = columns[hourColumnIndex - 1];
                                    var earthTemperature = columns[earthTempColumnIndex - 1];
                                    var airTemperature = columns[airTempColumnIndex - 1];

                                    InsertDataToDatabase(month, day, hour, true, true, dbcontext, earthTemp: earthTemperature, airTemp: airTemperature);
                                }
                                else
                                {
                                    WriteLog($"Invalid line format or missing values: {line}");
                                    return false; // Indicate failure due to invalid line
                                }
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    WriteLog("Error reading CSV file please check the config ! ");
                    return false;
                }
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

        private static bool InsertDataToDatabase(string month, string day, string hour, bool hasEarthTemp, bool hasAirTemp, CsvContext context, string earthTemp = "0", string airTemp = "0")
        {
            try
            {
                if (hasEarthTemp == true && hasAirTemp == false)
                {
                    // Create a new CsvData entry and link it to the Master entry
                    var csvData = new CsvData
                    {
                        Month = month, // Example mapping, adjust based on your CSV structure
                        Day = day,
                        Hour = hour,
                        EarthTemperature = double.Parse(earthTemp, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                        AirTemperature = 0,
                        Master = masterToAdd // Link the CsvData to the Master entry
                    };
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
                        // Save changes to persist both Master and CsvData entries
                        context.SaveChanges();
                    }
                    // Add the CsvData entry to the context
                    WriteLog($"Data inserted to database: {month}, {day}, {hour}, {earthTemp}");
                }else if (hasEarthTemp == false && hasAirTemp == true)
                {
                    // Create a new CsvData entry and link it to the Master entry
                    var csvData = new CsvData
                    {
                        Month = month, // Example mapping, adjust based on your CSV structure
                        Day = day,
                        Hour = hour,
                        EarthTemperature = 0,
                        AirTemperature = double.Parse(airTemp, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                        Master = masterToAdd // Link the CsvData to the Master entry
                    };
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
                        // Save changes to persist both Master and CsvData entries
                        context.SaveChanges();
                    }
                    // Add the CsvData entry to the context
                    WriteLog($"Data inserted to database: {month}, {day}, {hour}, {airTemp}");
                }
                else
                {
                    // Create a new CsvData entry and link it to the Master entry
                    var csvData = new CsvData
                    {
                        Month = month, // Example mapping, adjust based on your CSV structure
                        Day = day,
                        Hour = hour,
                        EarthTemperature = double.Parse(earthTemp, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                        AirTemperature = double.Parse(airTemp, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                        Master = masterToAdd // Link the CsvData to the Master entry
                    };
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
                        // Save changes to persist both Master and CsvData entries
                        context.SaveChanges();
                    }
                    // Add the CsvData entry to the context
                    WriteLog($"Data inserted to database: {month}, {day}, {hour}, {airTemp},{earthTemp}");
                }

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
