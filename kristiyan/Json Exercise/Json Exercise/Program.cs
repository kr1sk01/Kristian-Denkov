using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.CodeDom;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
namespace Json_Exercise
{
    public class Other : Odit
    {
        public Other(string col1, string col2, string col3, string col4, string col5, Dictionary<string, string> data) : base(col1, col2, col3, col4, col5)
        {
            base.col1 = col1;
            base.col2 = col2;
            base.col3 = col3;
            base.col4 = col4;
            base.col5 = col5;
            this.data = data;
        }
        Dictionary<string, string> data = new Dictionary<string, string>();
    }
    public class RenewJSON
    {
        public RenewJSON(string Name, string OriginalValue, string CurrentValue)
        {
            this.Name = Name;
            this.OriginalValue = OriginalValue;
            this.CurrentValue = CurrentValue;
        }
        public string? Name { get; set; }
        public string? OriginalValue { get; set; }
        public string? CurrentValue { get; set; }
    }
    public class Renew : Odit
    {
        public Renew(string col1, string col2, string col3, string col4, string col5, RenewJSON? rJson) : base(col1, col2, col3, col4, col5)
        {
            base.col1 = col1;
            base.col2 = col2;
            base.col3 = col3;
            base.col4 = col4;
            base.col5 = col5;
            this.rJson = rJson;
        }
        public RenewJSON? rJson { get; set; }

    }
    public class Odit
    {
        public Odit(string col1, string col2, string col3, string col4, string col5)
        {
            this.col1 = col1;
            this.col2 = col2;
            this.col3 = col3;
            this.col4 = col4;
            this.col5 = col5;
        }

        public string? col1 { get; set; }
        public string? col2 { get; set; }
        public string? col3 { get; set; }
        public string? col4 { get; set; }
        public string? col5 { get; set; }

    }
    public class Program
    {

        public void ReadData()
        {
            string filePath = "C:\\Users\\a1bg535412\\Desktop\\test1.xlsx";


            List<Odit> odits = new List<Odit>();

            List<string> checkBoxNameChecked = new List<string> { "OriginalValue", "RegisterAddress" };

            Renew renew;
            RenewJSON renewJSON = new RenewJSON("","","");
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
                                foreach (var kvp in item)
                                {
                                    if (checkBoxNameChecked.Contains(kvp.Key))
                                        dictToAdd.Add(kvp.Key, kvp.Value);                                   
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
            foreach(var o in odits)
            {
                Console.WriteLine(o.ToString());
            }
        }
    }
}
