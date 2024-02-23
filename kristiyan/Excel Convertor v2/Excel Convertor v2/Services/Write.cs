using Excel_Convertor_v2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel_Convertor_v2;
using OfficeOpenXml;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Excel_Convertor_v2.Services
{
    public static class Write
    {
        public static List<Odit> testOdits = new List<Odit>();

        public static List<string> chosenPropsToShowList = new List<string> {"Дата", "Потребител", "IdIPAddress", "IsActive", "MaxValue", "MinValue", "NewValue", "Note", "Offset", "ParameterId", "Port", "Priority", "ProtocolType", "RegisterAddress", "RequestId" };
        public static async Task WriteExcelFile(string fileOutPath, List<Odit> odits, List<string> chosenPropsToShowList)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                //print the headers for those checkboxes
                await WriteHeaders(worksheet, chosenPropsToShowList);
                
                //print data for the chechboxes


                var excelFile = new FileInfo("output.xlsx");
                package.SaveAs(excelFile);
            }


        }//TODO

        public static async Task WriteHeaders(ExcelWorksheet worksheet, List<string> chosenPropsToShowList)
        {
            await Task.Run(()=>
                {
                for (int col = 1; col <= chosenPropsToShowList.Count(); col++)
                {
                    worksheet.Cells[1, col].Value = chosenPropsToShowList[col - 1];
                }
            });
            
        }

        public static async Task PrintOditData(ExcelWorksheet worksheet, List<string> chosenPropsToShowList)
        {
            for (int row = 2; row <= testOdits.Count()+1; row++)
            {
                for(int col = 1; col <= chosenPropsToShowList.Count(); col++)
                {
                    //worksheet.Cells[row, col].Value = 
                }
            }
        }
        
        public static void TestFunctionForOdits()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo("C:\\Users\\a1bg535413\\Downloads\\Input-Mixed.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                var staticCols = new Dictionary<string, string>();
                string? jsonString = null;
                for (int row = 10; row<=worksheet.Dimension.End.Row; row++)
                {
                    for (int col = 1; col <= 5; col++)
                    {
                        staticCols.Add(worksheet.Cells[10, col].Value.ToString(), worksheet.Cells[row, col].Value.ToString());
                    }

                    jsonString = worksheet.Cells[row, 6].Value.ToString();

                    testOdits.Add(new Odit(staticCols, jsonString));
                }

                
            }
            
        }

    }
}
