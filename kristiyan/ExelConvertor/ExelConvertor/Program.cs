using System.Text;
using System.Text.Json.Serialization;
using OfficeOpenXml;
using System.Text.Json;
using System.Net.Http.Json;
using System;
using OfficeOpenXml.Drawing.Slicer.Style;
using static OfficeOpenXml.ExcelErrorValue;
using System.Diagnostics;

namespace ExelConvertor
{
    public class Tuple
    {
        public string Name { get; set; }
        public object Value { get; set; }
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
            Stopwatch stopwatch = new Stopwatch();

            // Start the stopwatch
            stopwatch.Start();
            List<Odit> odits = new List<Odit>();

            List<Tuple> jsonList = new List<Tuple>();
            
            string basePath = "C:\\Users\\a1bg535412\\source\\repos\\interns\\kristiyan\\ExelConvertor\\ExelConvertor";
            // Specify the file path
            string filePath = "Input1.xlsx";
            string fileToRead = basePath + "\\" + filePath;
            string fileToWrite = basePath + "\\" + "Output.txt";
            // Open the file for reading using a StreamReader
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

            //Creating new Excel file 
            string fileOutPath = basePath + "\\" + "Output2.xlsx";

            // Create a new Excel package
            using (var package = new ExcelPackage(new FileInfo(fileOutPath)))
            {
                // Add a new worksheet to the Excel package
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(DateTime.Now.ToString());
                int row = 2;
                worksheet.Cells[1, 1].Value = "Дата"; // Writing to column A
                worksheet.Cells[1, 2].Value = "Потребител"; // Writing to column B
                worksheet.Cells[1, 3].Value = "Тип"; // Writing to column C
                worksheet.Cells[1, 4].Value = "Обект"; // Writing to column D
                worksheet.Cells[1, 5].Value = "Идентификатор"; // Writing to column E
                worksheet.Cells[1, 6].Value = "Абонатна станция";
                worksheet.Cells[1, 7].Value = "IP адрес";
                worksheet.Cells[1, 8].Value = "Мнемосхема";
                worksheet.Cells[1, 9].Value = "Номер на станция";
                worksheet.Cells[1, 10].Value = "Логически номер на топломер";
                Console.WriteLine("Odits count: " + odits.Count());
                foreach (Odit o in odits)
                {
                    worksheet.Cells[row, 1].Value = o.DateTimeString; // Writing to column A
                    worksheet.Cells[row, 2].Value = o.UserType; // Writing to column B
                    worksheet.Cells[row, 3].Value = o.ActionType; // Writing to column C
                    worksheet.Cells[row, 4].Value = o.Object; // Writing to column D
                    worksheet.Cells[row, 5].Value = o.Identificator; // Writing to column E
                    worksheet.Cells[row, 6].Value = o.Tuples.FirstOrDefault((x)=> x.Name == "Name").Value.ToString();
                    worksheet.Cells[row, 7].Value = o.Tuples.FirstOrDefault((x) => x.Name == "IpAddress").Value.ToString();
                    worksheet.Cells[row, 8].Value = o.Tuples.FirstOrDefault((x) => x.Name == "StationConfigurationName").Value.ToString();
                    worksheet.Cells[row, 9].Value = o.Tuples.FirstOrDefault((x) => x.Name == "StationNumber").Value;
                    worksheet.Cells[row, 10].Value = o.Tuples.FirstOrDefault((x) => x.Name == "LogicalHeatmeterNumber").Value;
                    row++;
                }
                package.Save();
                stopwatch.Stop(); // Stop the stopwatch

                // Output elapsed time
                Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed}");
            }

        }

    }
}

