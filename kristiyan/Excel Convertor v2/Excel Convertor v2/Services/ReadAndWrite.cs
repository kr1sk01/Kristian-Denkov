using Excel_Convertor_v2.Classes;
using OfficeOpenXml;
using System.CodeDom;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using static OfficeOpenXml.ExcelErrorValue;
#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2
{
    public class ReadAndWrite
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
        }//Ne se polzva
        public async Task<HashSet<string>> ReadColTitles(string fileToRead)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            List<Object>? jsonList = new List<Object>();

            HashSet<string> uniqueNames = new HashSet<string>();
            try
            {
                
                using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(fileToRead)))
                {

                    // Get the first worksheet in the Excel file
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // Determine the number of rows and columns in the worksheet
                    int rows = worksheet.Dimension.Rows;
                    int cols = worksheet.Dimension.Columns;
                    int initialRow = FindStartRowIndex(package.Workbook.Worksheets[0]);
                    if (initialRow == -1)//Check if program can't find row with data
                    {
                        LogException(new Exception("Couldn't find row with data!"));
                    }
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 1].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 2].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 3].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 4].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 5].Value?.ToString());
                    List<string> someStr = new List<string> { "CurrentValue", "OriginalValue" };

                    for (int row = initialRow; row <= rows; row++)
                    {

                        //var test = JsonConvert.DeserializeObject<Object>(worksheet.Cells[row, 6].Value?.ToString());
                        //var props = test?.GetType().GetProperties();
                        //var test1 = props.Where(x => someStr.Contains(x.Name)).ToList();
                        //foreach (var item in test1)
                        //{
                        //    var value = item.GetValue(test, null);
                        //}
                        ////var test2 = test1.GetValue(test, null);
                        string jsonString = worksheet.Cells[row, 6].Value?.ToString();
                        //;
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
            }catch(Exception e)
            {
                LogException(e);
            }
            
            return uniqueNames;
        }
        public async Task<List<Odit>> ReadData(string filePath, List<string> checkBoxChecked)
        {
            
            List<Odit> odits = new List<Odit>();

            Renew renew;
            RenewJSON renewJSON = new RenewJSON("", "", "");
            Other other;
            string[] columnStrings = new string[5];
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Get the first worksheet in the Excel file
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Determine the number of rows and columns in the worksheet
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                Console.WriteLine($"rowcount: {rowCount}");
                for (int row = 1; row <= rowCount; row++)
                {
                    string jsonString = worksheet.Cells[row, 6].Value?.ToString();
                    Console.WriteLine($"row {row} json string:{jsonString}");
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    for (int c = 1; c <= 5; c++)
                        columnStrings[c - 1] = worksheet.Cells[row, c].Value?.ToString();

                    string jsonString = worksheet.Cells[row, 6].Value?.ToString();
                    if (jsonString != null)
                    {
                        if (worksheet.Cells[row, 3].Value?.ToString() == "Обновяване")
                        {

                            JsonDocument doc = JsonDocument.Parse(jsonString);

                            // Get the root element of the JSON document (assuming it's an array)
                            JsonElement root = doc.RootElement;

                            // Check if the root element is an array
                            if (root.ValueKind == JsonValueKind.Array)
                            {
                                string name;
                                string originalValue;
                                string currentValue;
                                foreach (JsonElement item in root.EnumerateArray())
                                {
                                    // Extract the properties from the JSON object
                                    name = item.GetProperty("Name").GetString();
                                    originalValue = item.GetProperty("OriginalValue").GetString();
                                    currentValue = item.GetProperty("CurrentValue").GetString();

                                    // Create an instance of AccessFailedCountData class and populate its properties
                                    renewJSON = new RenewJSON(name, originalValue, currentValue);


                                    // Print the properties of the deserialized object
                                    Console.WriteLine($"Name: {renewJSON.Name}");
                                    Console.WriteLine($"OriginalValue: {renewJSON.OriginalValue}");
                                    Console.WriteLine($"CurrentValue: {renewJSON.CurrentValue}");

                                }
                                renew = new Renew(columnStrings[0],
                                                columnStrings[1],
                                                columnStrings[2],
                                                columnStrings[3],
                                                columnStrings[4],
                                                renewJSON);
                                odits.Add(renew);
                            }

                        }
                        else
                        {
                            List<Dictionary<string, string>> deserializedList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString);
                            Dictionary<string, string> dictToAdd = new Dictionary<string, string>();
                            foreach (var item in deserializedList)
                            {
                                foreach (var kv in item)
                                {
                                    if (checkBoxChecked.Contains(kv.Key))
                                        dictToAdd.Add(kv.Key, kv.Value);
                                }
                            }
                            other = new Other(columnStrings[0],
                                columnStrings[1],
                                columnStrings[2],
                                columnStrings[3],
                                columnStrings[4],
                                dictToAdd);
                            odits.Add(other);
                        }
                    }
                }
            }
            foreach (var o in odits)
            {
                Console.WriteLine(o.ToString());
            }
            return odits;
        }
        public async Task WriteExcelFile(string fileOutPath, string[] ColLabels, List<Odit> odits)
        {
           
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