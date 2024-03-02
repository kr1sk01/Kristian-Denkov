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

        public static List<string> JsonParser(int initialRow, List<string>? jsonColNames, List<string> uniqueNames, ExcelWorksheet worksheet, int colIndex)
        {
            List<string> tempList = new List<string>();


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
                                if (!uniqueNames.Contains(nameValue) && !tempList.Contains(nameValue))
                                {
                                    tempList.Add(nameValue);
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
                            if (!uniqueNames.Contains(nameValue) && !tempList.Contains(nameValue))
                            {
                                tempList.Add(nameValue);
                            }
                        }
                    }
                }
            }
            return tempList;
        }
        public static List<string> ReadColTitles(string fileToRead, List<string>? jsonColNames)
        {
            if (jsonColNames == null)
            {
                jsonColNames = new List<string> { "стойност" };//TODO MAKE INPUT FROM FORM TO LOWER!!!!!!!!!!
                MessageBox.Show("Понеже не сте въвели json колони, при наличието на колона 'Стойност', тя ще бъде избрана за json колона", "Не сте въвели json колони", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
                

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
                            uniqueNames.Add(cell.ToString());
                        }
                        else
                        {
                            uniqueNames.AddRange(JsonParser(initialRow + 1, jsonColNames, uniqueNames, worksheet, colCounter));
                        }
                        
                    }

                    /*initialRow += 1; Почваме от следващия ред да взимаме Prop-совете на JsonString-овете
                    
                    List<int> jsonColIndexes = new List<int>();
                    foreach (var item in uniqueNames)
                    {
                        if (jsonColNames.Contains(item.ToLower()))
                        {
                            jsonColIndexes.Add(uniqueNames.IndexOf(item) + 1);
                        }
                    }
                    foreach (var index in jsonColIndexes)
                    {
                        List<string> temp = new List<string>(JsonParser(initialRow, jsonColNames, uniqueNames, worksheet, index));
                        uniqueNames.AddRange(temp);
                    }*/
                }
            }
            catch (Exception e)
            {
                Log.LogException(e);
            }

            return uniqueNames;
        }
        public static List<TableRow> ReadData(string filePath, List<string> chosenPropsToShowList, List<string> jsonColNameStrings)
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
                //Getting list of json column idenxes
                List<int> jsonColumnIndexes = new List<int>();
                foreach (var item in jsonColNameStrings)
                {
                    jsonColumnIndexes.Add(tableHeader.IndexOf(item));
                }
                /* We already got all indexes of the json columns
                 * Now we should make a loop in which if it is json col and not regular one we desrialize and we are starting to search if 
                 * one of the chosenProps are in this json or not, so we would be able to decide if we will add it to the row class
                 * as column class or not 
                 */
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
