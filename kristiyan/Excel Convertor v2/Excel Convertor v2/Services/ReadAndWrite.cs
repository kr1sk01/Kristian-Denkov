using Excel_Convertor_v2;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.CodeDom;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using static OfficeOpenXml.ExcelErrorValue;
#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2
{
    class ReadAndWrite : Form
    {


        public async Task<string> Main(string file)
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
            string fileOutPath = basePath + "\\" + DateTime.Now.ToShortDateString() + ".xlsx"; //Output file path + filename

            //Timer start
            stopwatch.Start();

            try
            {
                // Open the file for reading using a StreamReader
                //odits = await ReadExcelFile(file);
                // Create a new Excel package
                //await WriteExcelFile(fileOutPath, ColLabels, odits);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Имаше проблем при конвертирането. Свържете се с администратор!", "Неуспешно конвертиране!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogException(ex);
                return "0";
            }

            //Timer stop
            stopwatch.Stop();
            // Output elapsed time in a log file
            LogExecutionTime(stopwatch);
            return "1";
        }
        public async Task<HashSet<string>> ReadColTitles(string fileToRead)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            List<Odit>? odits = new List<Odit>();
            List<Object>? jsonList = new List<Object>();

            HashSet<string> uniqueNames = new HashSet<string>();

            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(fileToRead)))
            {

                // Get the first worksheet in the Excel file
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Determine the number of rows and columns in the worksheet
                int rows = worksheet.Dimension.Rows;
                int cols = worksheet.Dimension.Columns;
                int initialRow = FindStartRowIndex(package.Workbook.Worksheets[0]);

                if(initialRow == -1)//Check if program can't find row with data
                {
                    LogException(new Exception("Couldn't fine row with data!"));
                }
                for (int row = initialRow; row <= rows; row++)
                {
                    string jsonString = worksheet.Cells[row, 6].Value?.ToString();

                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        // Determine if the JSON represents an object or an array
                        JsonDocument doc = JsonDocument.Parse(jsonString);
                        if (doc.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            // If it's an array, iterate over each element
                            foreach (var element in doc.RootElement.EnumerateArray())
                            {
                                if (element.TryGetProperty("Name", out var nameProperty))
                                {
                                    string nameValue = nameProperty.GetString();
                                    uniqueNames.Add(nameValue);
                                }
                            }
                        }
                        else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            // If it's an object, extract "Name" property directly
                            if (doc.RootElement.TryGetProperty("Name", out var nameProperty))
                            {
                                string nameValue = nameProperty.GetString();
                                uniqueNames.Add(nameValue);
                            }
                        }
                    }
                }
            }
            return uniqueNames;
        }
        public void AddCheckBoxes(HashSet<string> names)
        {
            string[] asd = new string[] { "asdasdsa", "asdasddas", "asddasasdd" };
            foreach (string name in asd)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Name = name;
                checkBox.Text = name; // You can set text as you want
                checkBox.AutoSize = true; // Adjusts the size of the checkbox automatically
                checkBox.Location = new System.Drawing.Point(20, 20 + (checkBox.Height + 5) * Controls.Count); // Adjust the position
                Controls.Add(checkBox); // Add checkbox to the panel
            }
        }
        public async Task WriteExcelFile(string fileOutPath, string[] ColLabels, List<Odit> odits)
        {
            object? res;
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
                    int col = 1;
                    //Writing up the col titles
                    {
                        worksheet.Cells[row, 1].Value = o.DateTimeString; // Writing to column A
                        worksheet.Cells[row, 2].Value = o.UserType; // Writing to column B
                        worksheet.Cells[row, 3].Value = o.ActionType; // Writing to column C
                        worksheet.Cells[row, 4].Value = o.Object; // Writing to column D
                        worksheet.Cells[row, 5].Value = o.Identificator; // Writing to column E
                    }
                    if (o.Objects != null)
                        foreach (Object Jo in o.Objects)
                        {
                            worksheet.Cells[row, col].Value = Jo.ToString();
                            col++;
                        }

                    row++;
                }
                await package.SaveAsync();
            }
        }
        private static void LogExecutionTime(Stopwatch sWatch)
        {
            string logFilePath = "executionTimeLog.log";

            // Write the exception details to a log file
            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine($"[{DateTime.Now}] Execution time: {sWatch.Elapsed}");
            }
        }
        private static void LogException(Exception ex)
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
        public int FindStartRowIndex(ExcelWorksheet worksheet)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Define the string to search for
            string searchString = "Дата"; // Change this to your specific string

            // Loop through rows and check the first column for the search string
            for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
            {
                var cellValue = worksheet.Cells[row, 1].Value;
                if (cellValue != null && cellValue.ToString() == searchString)
                {
                    return row + 1; // Return the row index where the search string is found
                }
            }

            // If the search string is not found, return a default value or handle accordingly
            return -1;
        }
    }
}
#pragma warning restore CS8601, CS8604