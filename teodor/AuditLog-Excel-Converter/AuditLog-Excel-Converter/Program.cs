using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Diagnostics;

namespace AuditLog_Excel_Converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //Change file paths to appropriately
            string inputFileName = "C:\\Users\\a1bg535413\\source\\repos\\interns\\teodor\\AuditLog-Excel-Converter\\AuditLog-Excel-Converter\\Input.xlsx";
            string outputFileName = "C:\\Users\\a1bg535413\\source\\repos\\interns\\teodor\\AuditLog-Excel-Converter\\AuditLog-Excel-Converter\\Output.xlsx";

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Parse JSON for all rows in the worksheet
            var jsonDataDict = new Dictionary<int, JArray>();
            using (var inputPackage = new ExcelPackage(new FileInfo(inputFileName)))
            {
                var inputWorksheet = inputPackage.Workbook.Worksheets[0];
                int rowCount = inputWorksheet.Dimension.Rows;

                for (int row = 12; row <= rowCount; row++)
                {
                    var json = inputWorksheet.Cells[row, 6].Value?.ToString();
                    if (!string.IsNullOrEmpty(json))
                    {
                        jsonDataDict[row] = JArray.Parse(json);
                    }
                }

                // Create output Excel file
                using (var outputPackage = new ExcelPackage(new FileInfo(outputFileName)))
                {
                    var outputWorksheet = outputPackage.Workbook.Worksheets[0];

                    // Process rows in parallel
                    Parallel.For(12, rowCount + 1, (row, state) =>
                    {
                        if (jsonDataDict.TryGetValue(row, out var jsonData))
                        {
                            string ipAddress = null;
                            long? logicalHeatmeterNumber = null;
                            string name = null;
                            string stationConfigurationName = null;
                            long? stationNumber = null;

                            foreach (JObject jsonObject in jsonData)
                            {
                                string nameProp = (string)jsonObject["Name"];
                                string valueProp = (string)jsonObject["Value"];

                                if (nameProp == "IpAddress")
                                {
                                    ipAddress = valueProp;
                                }
                                else if (nameProp == "LogicalHeatmeterNumber")
                                {
                                    logicalHeatmeterNumber = long.Parse(valueProp);
                                }
                                else if (nameProp == "Name")
                                {
                                    name = valueProp;
                                }
                                else if (nameProp == "StationConfigurationName")
                                {
                                    stationConfigurationName = valueProp;
                                }
                                else if (nameProp == "StationNumber")
                                {
                                    stationNumber = long.Parse(valueProp);
                                }
                            }

                            // Populate output
                            outputWorksheet.Cells[row - 10, 1].Value = inputWorksheet.Cells[row, 1].Value;
                            outputWorksheet.Cells[row - 10, 2].Value = inputWorksheet.Cells[row, 2].Value;
                            outputWorksheet.Cells[row - 10, 3].Value = inputWorksheet.Cells[row, 3].Value;
                            outputWorksheet.Cells[row - 10, 4].Value = inputWorksheet.Cells[row, 4].Value;
                            outputWorksheet.Cells[row - 10, 5].Value = inputWorksheet.Cells[row, 5].Value;
                            outputWorksheet.Cells[row - 10, 6].Value = name;
                            outputWorksheet.Cells[row - 10, 7].Value = ipAddress;
                            outputWorksheet.Cells[row - 10, 8].Value = stationConfigurationName;
                            outputWorksheet.Cells[row - 10, 9].Value = stationNumber;
                            outputWorksheet.Cells[row - 10, 10].Value = logicalHeatmeterNumber;
                        }
                    });

                    // Save output Excel file
                    outputPackage.SaveAs(new FileInfo(outputFileName));
                }
            }

            // Stop the stopwatch and print the elapsed time
            stopwatch.Stop();
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} milliseconds");
        }
    }
}
