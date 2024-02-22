using Excel_Convertor_v2.Classes;
using OfficeOpenXml;
using System.CodeDom;
using System.Diagnostics;
using Newtonsoft;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using static OfficeOpenXml.ExcelErrorValue;
using System.Security.Principal;
#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2
{

    public class ReadAndWrite
    {
        public async Task<int> FindJsonColumn(ExcelWorksheet worksheet)
        {
            var startingRow = FindStartRowIndex(worksheet).Result;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                string? test = worksheet.Cells[startingRow - 1, col].Value?.ToString();
                if (test == "Стойност")
                    return col;
            }
            return worksheet.Dimension.End.Column;
        }
        public async Task<Dictionary<string, string>> ReadStaticCols(ExcelWorksheet worksheet)
        {
            Dictionary<string, string> staticCols = new Dictionary<string, string>();
            
            // Get the first worksheet in the Excel file    
            int rows = worksheet.Dimension.Rows;
            int cols = worksheet.Dimension.Columns;
            int initialRow = await FindStartRowIndex(worksheet);

            for (int col = 1; col < cols; col++)
            {
                
                var somestaticvar = worksheet.Cells[initialRow - 1, col].Value?.ToString();
                if (somestaticvar == null || somestaticvar.ToLower() == "стойност")
                    return staticCols;
                staticCols.Add(worksheet.Cells[initialRow - 1, col].Value?.ToString(), "");
                
            }

            return staticCols;
        }//Ne se polzva
        public async Task<HashSet<string>> ReadColTitles(string fileToRead)
        {


            List<Object>? jsonList = new List<Object>();

            HashSet<string> uniqueNames = new HashSet<string>();
            try
            {

                using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(fileToRead)))
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    // Get the first worksheet in the Excel file
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // Determine the number of rows and columns in the worksheet
                    int rows = worksheet.Dimension.Rows;
                    int cols = worksheet.Dimension.Columns;
                    var initialRow = FindStartRowIndex(package.Workbook.Worksheets[0]).Result;
                    if (initialRow == -1)//Check if program can't find row with data
                    {
                        LogException(new Exception("Couldn't find row with data!"));
                    }
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 1].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 2].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 3].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 4].Value?.ToString());
                    uniqueNames.Add(worksheet.Cells[initialRow - 1, 5].Value?.ToString());

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
            }
            catch (Exception e)
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
            Odit tempOdit;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Get the first worksheet in the Excel file
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Determine the number of rows and columns in the worksheet
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                var jsonColIndex = await FindJsonColumn(worksheet);
                var initialRow = await FindStartRowIndex(worksheet);
                Console.WriteLine($"rowcount: {rowCount}");
                for (int row = initialRow; row <= rowCount; row++)
                {
                    string jsonString = worksheet.Cells[row, jsonColIndex].Value?.ToString();
                    Console.WriteLine($"row {row} json string:{jsonString}");
                }
                ;
                for (int row = initialRow; row <= rowCount; row++)
                {
                    string jsonString = worksheet.Cells[row, jsonColIndex].Value?.ToString();
                    if (jsonString != null)
                    {
                        var deserializedList1 =
                               JsonSerializer.Deserialize<List<Test>>(jsonString);

                        Dictionary<string, object> dictToAdd1 = new Dictionary<string, object>();
                        foreach (var item in deserializedList1)
                        {
                            var key = item.Name;
                            var value = item.Value?.ToString();

                            if (checkBoxChecked.Contains(key))
                            {
                                dictToAdd1.Add(key, value);
                            }


                        }
                        //var dictToAdd1 = await ReadStaticCols(worksheet);
                        //other = new Other(dictToAdd1, dictToAdd);
                        //odits.Add(other);

                        if (worksheet.Cells[row, 3].Value?.ToString() == "Обновяване")
                        {


                            JsonDocument doc = JsonDocument.Parse(jsonString);

                            // Get the root element of the JSON document (assuming it's an array)
                            JsonElement root = doc.RootElement;

                            // Check if the root element is an array
                            if (root.ValueKind == JsonValueKind.Array)
                            {

                                foreach (JsonElement item in root.EnumerateArray())
                                {
                                    // Extract the properties from the JSON object
                                    var name = item.GetProperty("Name").ToString();
                                    var originalValue = item.GetProperty("OriginalValue").ToString();
                                    var currentValue = item.GetProperty("CurrentValue").ToString();

                                    // Create an instance of AccessFailedCountData class and populate its properties
                                    renewJSON = new RenewJSON(name, originalValue, currentValue);


                                    // Print the properties of the deserialized object
                                    Console.WriteLine($"Name: {renewJSON.Name}");
                                    Console.WriteLine($"OriginalValue: {renewJSON.OriginalValue}");
                                    Console.WriteLine($"CurrentValue: {renewJSON.CurrentValue}");

                                }
                                var dictToAdd = ReadStaticCols(worksheet).Result;
                                int tempColIndex = 1;
                                foreach(var item in dictToAdd)
                                {
                                    dictToAdd[item.Key.ToString()] = worksheet.Cells[row, tempColIndex].Value?.ToString();
                                    tempColIndex++;
                                }
                                renew = new Renew(dictToAdd, renewJSON);
                                odits.Add(renew);
                            }

                        }
                        else
                        {
                            var deserializedList = 
                                JsonSerializer.Deserialize<List<Test>>(jsonString);

                            Dictionary<string, object> dictToAdd = new Dictionary<string, object>();
                            foreach (var item in deserializedList) 
                            {
                                var key = item.Name;
                                var value = item.Value?.ToString();

                                if (checkBoxChecked.Contains(key))
                                {
                                    dictToAdd.Add(key, value);
                                }


                            }
                            var dictToAdd2 = await ReadStaticCols(worksheet);
                            other = new Other(dictToAdd2, dictToAdd);
                            odits.Add(other);
                        }
                    }
                    else
                    {
                        var dictToAdd = await ReadStaticCols(worksheet);
                        int tempColIndex = 1;
                        foreach (var item in dictToAdd)
                        {
                            dictToAdd[item.Key.ToString()] = worksheet.Cells[row, tempColIndex].Value?.ToString();
                            tempColIndex++;
                        }
                        tempOdit = new Odit(dictToAdd);
                        odits.Add(tempOdit);
                    }
                }
            }
            foreach (var o in odits)
            {
                Console.WriteLine(o.ToString());
            }

            return odits;
        }
        public async Task WriteExcelFile(string fileOutPath, List<Odit> odits)
        {

        }//TODO
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
        public async Task<int> FindStartRowIndex(ExcelWorksheet worksheet)
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