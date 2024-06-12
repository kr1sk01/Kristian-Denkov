using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using TermisImporter.Data;

namespace TermisImporter
{
    public partial class NimhDataService : ServiceBase
    {
        private Timer timer;
        private string csvDirectory;
        private string processedDirectory;
        private string errorDirectory;
        private string connectionString;

        public NimhDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            InitializeService();
            timer = new Timer(7000); // Set timer to 7 seconds
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            ProcessCsvFiles();
        }

        private void InitializeService()
        {
            csvDirectory = ConfigurationManager.AppSettings["CsvDirectory"];
            processedDirectory = ConfigurationManager.AppSettings["ProcessedDirectory"];
            errorDirectory = ConfigurationManager.AppSettings["ErrorDirectory"];
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;

            CreateDirectoryIfNotExists(csvDirectory);
            CreateDirectoryIfNotExists(processedDirectory);
            CreateDirectoryIfNotExists(errorDirectory);

            EnsureDatabaseExists();
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
            using (var context = new ServiceDbContext(connectionString))
            {
                if (!context.Database.Exists())
                {
                    context.Database.Create();
                }
            }
        }

        private void ProcessCsvFiles()
        {
            var csvFiles = Directory.GetFiles(csvDirectory, "*.csv");

            foreach (var csvFile in csvFiles.ToList())
            {
                int row = 0;
                try
                {
                    var lines = File.ReadAllLines(csvFile);

                    foreach (var line in lines)
                    {
                        row++;
                        string[] values;
                        values = line.Contains(",") ? line.Split(',') : Regex.Replace(line, @"\s{2,}", " ").Split(' ');


                        if (values.Length != 5)
                        {
                            HandleError($"Invalid data format in row [{row}]. This row does not contain 5 values.", csvFile);
                            break;
                        }

                        //Parse Month value
                        if (!int.TryParse(values[0], out int month))
                        {
                            HandleError($"Invalid data format in row [{row}]. Month value could not be parsed.", csvFile);
                            break;
                        }
                        if (month > 12)
                        {
                            HandleError($"Invalid data format in row [{row}]. Month value is not valid.", csvFile);
                            break;
                        }

                        //Parse Day value
                        if (!int.TryParse(values[1], out int day))
                        {
                            HandleError($"Invalid data format in row [{row}]. Day value could not be parsed.", csvFile);
                            break;
                        }
                        if (day > 31)
                        {
                            HandleError($"Invalid data format in row [{row}]. Day value is not valid.", csvFile);
                            break;
                        }

                        //Parse Hour value
                        if (!int.TryParse(values[2], out int hour))
                        {
                            HandleError($"Invalid data format in row [{row}]. Hour value could not be parsed.", csvFile);
                            break;
                        }
                        if (hour > 23)
                        {
                            HandleError($"Invalid data format in row [{row}]. Hour value is not valid.", csvFile);
                            break;
                        }

                        //Parse Temperature value
                        if (!double.TryParse(values[3], out double temp))
                        {
                            HandleError($"Invalid data format in row [{row}]. Temperature value is not valid.", csvFile);
                            break;
                        }

                        //Parse Temperature value
                        if (!double.TryParse(values[4], out double soilTemp))
                        {
                            HandleError($"Invalid data format in row [{row}]. Temperature value is not valid.", csvFile);
                            break;
                        }

                        var hourlyForecast = new HourlyForecast
                        {
                            Month = month,
                            Day = day,
                            Hour = hour,
                            Temp = temp,
                            SoilTemp = soilTemp
                        };

                        using (var context = new ServiceDbContext(connectionString))
                        {
                            var existingRecord = context.HourlyForecasts
                                .FirstOrDefault(x => x.Month == month && x.Day == day && x.Hour == hour);

                            if (existingRecord != null)
                            {
                                // Update existing record
                                existingRecord.Temp = temp;
                                existingRecord.SoilTemp = soilTemp;
                            }
                            else
                            {
                                // Add new record
                                context.HourlyForecasts.Add(hourlyForecast);
                            }

                            context.SaveChanges();
                        }
                    }

                    var processedFileName = Path.Combine(processedDirectory, Path.GetFileName(csvFile));
                    File.Move(csvFile, processedFileName);
                }
                catch (Exception ex)
                {
                    HandleError(ex.ToString(), csvFile);
                }
            }
        }

        private void HandleError(string message, string csvFile)
        {
            var errorFileName = Path.Combine(errorDirectory, Path.GetFileNameWithoutExtension(csvFile) + ".err");
            File.WriteAllText(errorFileName, message);

            var unreadFileName = Path.Combine(errorDirectory, Path.GetFileName(csvFile));
            File.Move(csvFile, unreadFileName);
        }
    }
}
