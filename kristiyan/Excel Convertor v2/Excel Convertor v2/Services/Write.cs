using Excel_Convertor_v2.Classes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2.Services
{
    public static class Write
    {
        public static void WriteData(string fileOutPath, List<TableRow> rows)
        {
            using (var package = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var worksheet = package.Workbook.Worksheets.Add(DateTime.Now.ToString());

                worksheet.DefaultColWidth = 35;


                for (int currentColumn = 0; currentColumn < rows[0].Columns.Count; currentColumn++)
                {
                    var value = rows[0].Columns[currentColumn];

                    var cell = worksheet.Cells[1, currentColumn + 1];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 12;

                    cell.Value = value.Name;
                }

                for (int currentRow = 0; currentRow < rows.Count; currentRow++)
                {
                    for (int currentColumn = 0; currentColumn < rows[currentRow].Columns.Count; currentColumn++)
                    {
                        var value = rows[currentRow].Columns[currentColumn];

                        var cell = worksheet.Cells[currentRow + 2, currentColumn + 1];

                        cell.Value = value.Value;

                    }
                }

                var saveFile = new FileInfo(fileOutPath);

                package.SaveAsAsync(saveFile);

            }
        }
    }
}
