using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2.Services
{
    public static class Find
    {
        public static async Task<int> FindStartRowIndex(ExcelWorksheet worksheet)
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
                    return row; // Return the row index where the search string is found
                }
            }

            // If the search string is not found, return a default value or handle accordingly
            return -1;
        }
    }
}
