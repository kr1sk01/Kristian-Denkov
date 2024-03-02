using Excel_Convertor_v2.Classes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Excel_Convertor_v2.Services
{
    public static class Read
    {

        public static Dictionary<string, string> JsonParser(int initialRow, Dictionary<string, string> uniqueNames, ExcelWorksheet worksheet, int colIndex)
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();

            int rows = worksheet.Dimension.Rows;
            for (int row = initialRow; row <= rows; row++)
            {
                string jsonString = worksheet.Cells[row, colIndex].Value?.ToString();

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
                                if (!uniqueNames.ContainsKey(nameValue) && !tempDict.ContainsKey(nameValue))
                                {
                                    tempDict.Add(nameValue, worksheet.Cells[initialRow-1, colIndex].Value.ToString());
                                }
                            }
                        }
                    }
                    else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        // If it's an object, extract "Name" property directly
                        if (doc.RootElement.TryGetProperty("Name", out var nameProperty))
                        {
                            string nameValue = nameProperty.GetString();
                            if (!uniqueNames.ContainsKey(nameValue) && !tempDict.ContainsKey(nameValue))
                            {
                                tempDict.Add(nameValue, worksheet.Cells[initialRow - 1, colIndex].Value.ToString());
                            }
                        }
                    }
                }
            }
            return tempDict;
        }
        public static Dictionary<string, string> ReadColTitles(string fileToRead, ref List<string>? jsonColNames)
        {
            if (jsonColNames == null)
            {
                jsonColNames = new List<string> { "стойност" };
                MessageBox.Show("Понеже не сте въвели json колони, при наличието на колона 'Стойност', тя ще бъде избрана за json колона", "Не сте въвели json колони", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
                
            Dictionary<string, string> uniqueNames = new Dictionary<string, string>();

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
                    var initialRow = Find.FindStartRowIndex(package.Workbook.Worksheets[0]).Result;
                    if (initialRow == -1)//Check if program can't find row with data
                    {
                        MessageBox.Show("Файлът трябва да има начална колона 'Дата'", "Грешка при четене", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Log.LogException(new Exception("Couldn't find row with data!"));
                    }
                    for (int colCounter = 1; colCounter <= cols; colCounter++)
                    {
                        var cell = worksheet.Cells[initialRow, colCounter].Value;

                        if (cell == null) { break; }

                        string cellValue = cell.ToString().ToLower().Replace(" ", "");
                        if (!jsonColNames.Contains(cellValue))
                        {
                            uniqueNames.Add(cell.ToString(), "");
                        }
                        else
                        {
                            var dictToAdd = JsonParser(initialRow + 1, uniqueNames, worksheet, colCounter);

                            foreach (var kvp in dictToAdd)
                            {
                                uniqueNames.Add(kvp.Key, kvp.Value);
                            }
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogException(e);
            }

            return uniqueNames;
        }
        public static List<TableRow> ReadData(string filePath, List<string> chosenPropsToShowList, List<string> jsonColNameStrings, Dictionary<string, string> jsonPropsOrigin)
        {
            var rows = new List<TableRow>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var ws = package.Workbook.Worksheets[0];

                var rowsCount = ws.Dimension.Rows;
                var columnsCount = ws.Dimension.Columns;

                var tableHeader = new List<string>();

                int startRowIndex = Find.FindStartRowIndex(ws).Result;
                tableHeader = CreateTableHeader(ws, startRowIndex, columnsCount, jsonColNameStrings);
                var tableRowHeaderIndex = startRowIndex;

                var valuesStartingRow = tableRowHeaderIndex + 1;

                for (int currentRowIndex = valuesStartingRow; currentRowIndex <= rowsCount; currentRowIndex++)
                {
                    var tableRow = new TableRow();

                    foreach (var chosenProp in chosenPropsToShowList)
                    {
                        var tableHeaderIndex = tableHeader.IndexOf(chosenProp);

                        if (!tableHeader.Contains(chosenProp))
                        {
                            var jsonColumnOrigin = jsonPropsOrigin[chosenProp];

                            for (int col = 1; col <= columnsCount; col++)
                            {
                                var headerCell = ws.Cells[startRowIndex, col].Value?.ToString();
                                if (jsonColumnOrigin == headerCell)
                                {
                                    var jsonString = ws.Cells[currentRowIndex, col].Value?.ToString();
                                    var column = CreateJsonColumn(jsonString, chosenProp);
                                    tableRow.Columns.Add(column);
                                }
                            }
                        }
                        else
                        {
                            for (int col = 1; col <= columnsCount; col++)
                            {
                                var headerCell = ws.Cells[startRowIndex, col].Value?.ToString();

                                if (chosenProp == headerCell)
                                {
                                    var value = ws.Cells[currentRowIndex, col].Value?.ToString() ?? string.Empty;
                                    tableRow.Columns.Add(new TableColumn(chosenProp, value));
                                }
                            }
                        }
                    }

                    rows.Add(tableRow);
                }
            }

            return rows;
        }
        private static List<string> CreateTableHeader(ExcelWorksheet ws, int headerRowIndex, int columnsCount, List<string> jsonColNames)
        {
            var tableHeader = new List<string>();

            for (int currentColumnIndex = 1; currentColumnIndex <= columnsCount; currentColumnIndex++)
            {
                var cellValue = ws.Cells[headerRowIndex, currentColumnIndex].Value?.ToString() ?? string.Empty;

                if (!jsonColNames.Contains(cellValue.Replace(" ", "").ToLower()))
                {
                    tableHeader.Add(cellValue);
                }
            }

            return tableHeader;
        }
        private static TableColumn CreateJsonColumn(string? jsonString, string chosenProp)
        {
            var json = new List<JsonCell>();

            if (jsonString != null)
            {
                json = JsonSerializer.Deserialize<List<JsonCell>>(jsonString);
            }

            var value = string.Empty;

            var test = json?.FirstOrDefault(x => x.Name == chosenProp);

            if (test != null)
            {
                if (chosenProp == "AccessFailedCount")
                {
                    value = $"{nameof(test.OriginalValue)}:{test.OriginalValue}, {nameof(test.CurrentValue)}:{test.CurrentValue}";
                }
                else
                {
                    value = test.Value?.ToString() ?? string.Empty;
                }
            }

            return new TableColumn(chosenProp, value);
        }
    }
}
