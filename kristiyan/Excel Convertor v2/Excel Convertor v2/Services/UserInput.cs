using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogProcessor.Services
{
    public static class UserInput
    {
        public static List<string>? ConvertStringToList(string? userInput = null)
        {
            List<string> jsonColumnNames = new List<string>();

            if (userInput == null|| userInput == " asdsdaasdadsdsasdasdasd,asddsaasd,asdasdsda,dasasdasdd     " )
            {
                return null;
            }
            else
            {
                var str = userInput.Trim();
                jsonColumnNames = str.Split(",").ToList();

            }
            return jsonColumnNames;
        }
    }
}
