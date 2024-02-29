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

        public static void JsonParser(int initialRow, List<string>? jsonColNames, ref List<string> uniqueNames, ExcelWorksheet worksheet, int colIndex)
        {
            int rows = worksheet.Dimension.Rows;
            for (int row = initialRow; row <= rows; row++)
            {
                string jsonString = worksheet.Cells[row, colIndex].Value?.ToString();
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
                                if (!uniqueNames.Contains(nameValue))
                                {
                                    uniqueNames.Add(nameValue);
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
                            if (!uniqueNames.Contains(nameValue))
                            {
                                uniqueNames.Add(nameValue);
                            }
                        }
                    }
                }
            }
        }
        public static List<string> ReadColTitles(string fileToRead, List<string>? jsonColNames)
        {
            if (jsonColNames == null)
                jsonColNames = new List<string> { "стойност", "нещо" };//TODO MAKE INPUT FROM FORM TO LOWER!!!!!!!!!!

            List<Object>? jsonList = new List<Object>();
            List<string> uniqueNames = new List<string>();

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
                        Log.LogException(new Exception("Couldn't find row with data!"));
                    }
                    for (int colCounter = 1; colCounter <= cols; colCounter++)
                    {
                        var cell = worksheet.Cells[initialRow, colCounter].Value;

                        if (cell == null) { break; }

                        uniqueNames.Add(cell.ToString());
                    }

                    initialRow += 1; //Почваме от следващия ред да взимаме Prop-совете на JsonString-овете

                    foreach (var item in uniqueNames)
                    {
                        if (jsonColNames.Contains(item.ToLower()))
                        {
                            JsonParser(initialRow, jsonColNames, ref uniqueNames, worksheet, uniqueNames.IndexOf(item) + 1);
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
        public static List<TableRow> ReadData(string filePath, List<string> chosenPropsToShowList)
        {
            var rows = new List<TableRow>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var ws = package.Workbook.Worksheets[0];

                var rowsCount = ws.Dimension.Rows;
                var columnsCount = ws.Dimension.Columns;

                Console.WriteLine($"rowcount: {rowsCount}");

                var startingHeaderRowFirstCellText = "Дата";

                var jsonColumnHeaderText = "Стойност";

                var tableRowHeaderIndex = 0;

                var tableHeader = new List<string>();

                for (int currentRowIndex = 1; currentRowIndex <= rowsCount; currentRowIndex++)
                {
                    var cellValue = ws.Cells[currentRowIndex, 1].Value?.ToString();

                    if (cellValue is null)
                    {
                        continue;
                    }

                    if (!startingHeaderRowFirstCellText.Equals(cellValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    tableRowHeaderIndex = currentRowIndex;


                    tableHeader = CreateTableHeader(ws, currentRowIndex, columnsCount);

                    break;
                }


                var jsonColumnIndex = tableHeader.IndexOf(jsonColumnHeaderText) + 1;

                var valuesStartingRow = tableRowHeaderIndex + 1;


                for (int currentRowIndex = valuesStartingRow; currentRowIndex <= rowsCount; currentRowIndex++)
                {
                    var tableRow = new TableRow();

                    foreach (var chosenProp in chosenPropsToShowList)
                    {
                        var tableHeaderIndex = tableHeader.IndexOf(chosenProp);

                        if (tableHeaderIndex == -1)
                        {
                            var jsonString = ws.Cells[currentRowIndex, jsonColumnIndex].Value?.ToString();

                            var column = CreateJsonColumn(jsonString, chosenProp);

                            tableRow.Columns.Add(column);
                        }
                        else
                        {

                            var value = ws.Cells[currentRowIndex, tableHeaderIndex + 1].Value?.ToString() ?? string.Empty;

                            tableRow.Columns.Add(new TableColumn(chosenProp, value));
                        }
                    }

                    rows.Add(tableRow);
                }
            }

            return rows;
        }
        private static List<string> CreateTableHeader(ExcelWorksheet ws, int currentRowIndex, int columnsCount)
        {
            var tableHeader = new List<string>();

            for (int currentColumnIndex = 1; currentColumnIndex <= columnsCount; currentColumnIndex++)
            {
                var cellValue = ws.Cells[currentRowIndex, currentColumnIndex].Value?.ToString() ?? string.Empty;

                tableHeader.Add(cellValue);
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
