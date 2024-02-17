using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using OfficeOpenXml;
using System.Text.Json;
using System.Net.Http;
using OfficeOpenXml.Drawing.Slicer.Style;
using static OfficeOpenXml.ExcelErrorValue;
using System.Collections;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System;
using System.Collections.Generic;
using static WinFormsApp1.Form1;


namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        public string[] file;
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            label1.Text = "Моля пусни с мишката екселския файл, който искаш да конвертираш!";
            this.Text = "Ексел файл конвертор";
            label2.Text = "";
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            file = (string[])e.Data.GetData(DataFormats.FileDrop);
            MyProgram mp = new MyProgram();
            mp.Main(file[0]);

        }
        public class Tuple
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
        public class Odit
        {
            public string DateTimeString { get; set; }
            public string UserType { get; set; }
            public string ActionType { get; set; }
            public string Object { get; set; }
            public string Identificator { get; set; }
            public List<Tuple> Tuples { get; set; }
        }

        public class MyProgram
        {

            public async Task Main(string file)
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
                    odits = await ReadExcelFile(file);
                    // Create a new Excel package
                    await WriteExcelFile(fileOutPath, ColLabels, odits);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Имаше проблем при конвертирането. Свържете се с администратор!", "Неуспешно конвертиране!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogException(ex);                    
                }

                //Timer stop
                stopwatch.Stop();
                // Output elapsed time
                Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed}");

            }
            public async Task<List<Odit>> ReadExcelFile(string fileToRead)
            {
                List<Odit> odits = new List<Odit>();
                List<Tuple> jsonList = new List<Tuple>();

                using (var package = await Task.Run(() => new ExcelPackage(new System.IO.FileInfo(fileToRead))))
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
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

                        jsonList = JsonConvert.DeserializeObject<List<Tuple>>(json);

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

                return odits;
            }
            public async Task WriteExcelFile(string fileOutPath, string[] ColLabels, List<Odit> odits)
            {

                using (var package = await Task.Run(() => new ExcelPackage(new FileInfo(fileOutPath))))
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
                        worksheet.Cells[row, 6].Value = o.Tuples.FirstOrDefault((x) => x.Name == "Name").Value.ToString();
                        worksheet.Cells[row, 7].Value = o.Tuples.FirstOrDefault((x) => x.Name == "IpAddress").Value.ToString();
                        worksheet.Cells[row, 8].Value = o.Tuples.FirstOrDefault((x) => x.Name == "StationConfigurationName").Value.ToString();
                        worksheet.Cells[row, 9].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "StationNumber").Value.ToString());
                        worksheet.Cells[row, 10].Value = long.Parse(o.Tuples.FirstOrDefault((x) => x.Name == "LogicalHeatmeterNumber").Value.ToString());
                        row++;
                    }
                    await package.SaveAsync();

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
}
