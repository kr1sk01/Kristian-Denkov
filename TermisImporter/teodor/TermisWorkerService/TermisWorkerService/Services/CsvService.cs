using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermisWorkerService.Models;

namespace TermisWorkerService.Services
{
    public class CsvService : ICsvService
    {
        private readonly ServiceDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ServiceSettings _serviceSettings;
        private readonly ColumnIndexSettings _columnIndexSettings;
        private readonly ILogger<CsvService> _logger;

        public CsvService(ServiceDbContext context, IEmailService emailService, IOptions<ServiceSettings> serviceSettings, IOptions<ColumnIndexSettings> columnIndexSettings, ILogger<CsvService> logger)
        {
            _context = context;
            _emailService = emailService;
            _serviceSettings = serviceSettings.Value;
            _columnIndexSettings = columnIndexSettings.Value;
            _logger = logger;
        }

        public void ProcessCsvFile(string filePath)
        {
            int row = 0;
            bool isSuccess = true;
            List<string> errorList = [];

            try
            {
                var master = new Master
                {
                    Date = DateTime.Now,
                    Status = CompletionStatus.Success
                };

                _context.Masters.Add(master);

                var lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                    throw new Exception($"File [{filePath}] is empty");

                foreach (var line in lines)
                {
                    row++;
                    string[] values = line.TrimStart().Split(_serviceSettings.CsvSeparator, StringSplitOptions.RemoveEmptyEntries);

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
                        master.Status = CompletionStatus.Failed;
                        isSuccess = false;
                        errorList.Add(parsingResponse.ErrorMessage!);
                    }
                }

                if (!isSuccess && master.ForecastDate != null)
                    master.Status = CompletionStatus.Partial;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                isSuccess = false;
            }

            if (isSuccess)
            {
                var processedFileName = Path.Combine(_serviceSettings.ProcessedDirectory, Path.GetFileName(filePath));
                OverwriteFile(filePath, processedFileName);
            }
            else
            {
                HandleCsvError(errorList, filePath);
            }
        }

        private void HandleCsvError(List<string> errors, string csvFile)
        {
            var errorFileName = Path.Combine(_serviceSettings.ErrorDirectory, Path.GetFileNameWithoutExtension(csvFile) + ".err");
            File.WriteAllLines(errorFileName, errors);

            var unreadFileName = Path.Combine(_serviceSettings.ErrorDirectory, Path.GetFileName(csvFile));
            OverwriteFile(csvFile, unreadFileName);

            _emailService.SendErrorEmail(unreadFileName, errors);
        }

        private void OverwriteFile(string sourceFilePath, string destinationFilePath)
        {
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
                _logger.LogInformation($"File [{destinationFilePath}] has been overwritten");
            }
            File.Move(sourceFilePath, destinationFilePath);
            _logger.LogInformation($"File [{sourceFilePath}] moved successfully to [{Path.GetDirectoryName(destinationFilePath)}]");
        }

        private DetailParseResponse ParseDetail(string[] values, int row, out Detail? detail)
        {
            var response = new DetailParseResponse
            {
                IsSuccess = true
            };

            detail = null;

            try
            {
                if (_columnIndexSettings.HasSoilTempColumn && values.Length != 5 || !_columnIndexSettings.HasSoilTempColumn && values.Length != 4)
                {
                    string errorMessage = $"Invalid data format in row [{row}]. This row does not contain the necessary amount of columns.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }

                if (!int.TryParse(values[_columnIndexSettings.MonthColumnIndex], out int month))
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

                if (!int.TryParse(values[_columnIndexSettings.DayColumnIndex], out int day))
                {
                    string errorMessage = $"Invalid data format in row [{row}]. Day value could not be parsed.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }
                if (day > 31 || day < 1 || month == 2 && day > 29)
                {
                    string errorMessage = $"Invalid data format in row [{row}]. Day value is not valid.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }

                if (!int.TryParse(values[_columnIndexSettings.HourColumnIndex], out int hour))
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

                if (!double.TryParse(values[_columnIndexSettings.TempColumnIndex].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double temp))
                {
                    string errorMessage = $"Invalid data format in row [{row}]. Temperature value is not valid.";
                    response.IsSuccess = false;
                    response.ErrorMessage = errorMessage;
                    return response;
                }

                double soilTemp = 0;
                if (_columnIndexSettings.HasSoilTempColumn)
                {
                    if (!double.TryParse(values[_columnIndexSettings.SoilTempColumnIndex].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out soilTemp))
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
                    SoilTemp = _columnIndexSettings.HasSoilTempColumn ? soilTemp : null
                };

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"Data parsing error in row [{row}]. {ex.Message}";
            }

            return response;
        }

        private bool IsDetailToUpdate(Detail detail, out Detail? detailToUpdate)
        {
            detailToUpdate = _context.Details.FirstOrDefault(x =>
                    x.Month == detail.Month &&
                    x.Day == detail.Day &&
                    x.Hour == detail.Hour);

            return detailToUpdate != null;
        }
    }
}
