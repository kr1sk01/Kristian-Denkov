using System.Text;
using System.Text.Json.Serialization;
using OfficeOpenXml;
using System.Text.Json;
using System.Net.Http.Json;
using System;
using OfficeOpenXml.Drawing.Slicer.Style;
using static OfficeOpenXml.ExcelErrorValue;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace ExelConvertor
{
    public class Tuple
    {
        public string? Name { get; set; }
        public object? Value { get; set; }
    }
    public class Odit
    {
        public string? DateTimeString { get; set; }
        public string? UserType { get; set; }
        public string? ActionType { get; set; }
        public string? Object { get; set; }
        public string? Identificator { get; set; }
        public List<Tuple>? Tuples { get; set; }
    }

    public class Program
    {

        static void Main(string[] args)
        {
            //Column names ( will be displayed in the output excel file ) 
            string[] ColLabels = new string[]{ "Дата",
                "Потребител",
                "Тип",
                "Обект",
                "Идентификатор",
                "Абонатна станция",
                "IP адрес",
                "Мнемосхема",
                "Номер на станция",
                "Логически номер на топломер"};

            Stopwatch stopwatch = new Stopwatch(); //Creating Timer

            List<Odit> odits = new List<Odit>(); //Creating list of objects that represent each row

            string basePath = Directory.GetCurrentDirectory(); // Getting current folder (where.exe is)


            string fileName = "Input.xlsx"; //Name of the input file ( should be witin the same folder )

            string fileToRead = basePath + "\\" + fileName; //Full path to the file

            string fileOutPath = basePath + "\\" + DateTime.Now.ToShortDateString() + ".xlsx"; //Output file path + filename

            //Timer start
            stopwatch.Start();

            try
            {
                // Open the file for reading using a StreamReader
                ReadExcelFile(fileToRead, ref odits);
                // Create a new Excel package
                WriteExcelFile(fileOutPath, ColLabels, odits);
            }
            catch (Exception ex)
            {
                LogException(ex);
                Console.WriteLine(ex.Message);
            }

            //Timer stop
            stopwatch.Stop();
            // Output elapsed time
            Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed}");

        }
        public static void ReadExcelFile(string fileToRead, ref List<Odit> odits)
        {
            List<Tuple> jsonList = new List<Tuple>();
            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(fileToRead)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Get the first worksheet in the Excel file
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Determine the number of rows and columns in the worksheet
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Iterate through each row and column to read the data
                for (int row = 12; row <= rowCount; row++)
                {
                    //List<Tuple> tuples;
                    string json = worksheet.Cells[row, 6].Value.ToString();

                    jsonList = JsonSerializer.Deserialize<List<Tuple>>(json);

                    odits.Add(new Odit
                    {
                        DateTimeString = worksheet.Cells[row, 1].Value.ToString(),
                        UserType = worksheet.Cells[row, 2].Value.ToString(),
                        ActionType = worksheet.Cells[row, 3].Value.ToString(),
                        Object = worksheet.Cells[row, 4].Value.ToString(),
                        Identificator = worksheet.Cells[row, 5].Value.ToString(),
                        Tuples = new List<Tuple>(jsonList)
                    });
                }

            }
        }
        public static void WriteExcelFile(string fileOutPath, string[] ColLabels, List<Odit> odits)
        {
            using (var package = new ExcelPackage(new FileInfo(fileOutPath)))
            {
                // Add a new worksheet to the Excel package
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(DateTime.Now.ToString());
                int row = 2;
                for (int i = 0; i < ColLabels.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = "\r\n" + ColLabels[i];
                }

                foreach (Odit o in odits)
                {
                    worksheet.Cells[row, 1].Value = o.DateTimeString; // Writing to column A
                    worksheet.Cells[row, 2].Value = o.UserType; // Writing to column B
                    worksheet.Cells[row, 3].Value = o.ActionType; // Writing to column C
                    worksheet.Cells[row, 4].Value = o.Object; // Writing to column D
                    worksheet.Cells[row, 5].Value = o.Identificator; // Writing to column E
                    worksheet.Cells[row, 6].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "Name").Value.ToString());
                    worksheet.Cells[row, 7].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "IpAddress").Value.ToString());
                    worksheet.Cells[row, 8].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "StationConfigurationName").Value.ToString());
                    worksheet.Cells[row, 9].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "StationNumber").Value.ToString());
                    worksheet.Cells[row, 10].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "LogicalHeatmeterNumber").Value.ToString());
                    row++;
                }
                package.Save();

            }
        }
        static void LogException(Exception ex)
        {
            string logFilePath = "error.log";

            // Write the exception details to a log file
            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine($"[{DateTime.Now}] Exception: {ex.Message}");
                writer.WriteLine($"StackTrace: {ex.StackTrace}");
                writer.WriteLine();
            }
            Console.WriteLine("Exception logged to error.log file.");
        }
    }
}

