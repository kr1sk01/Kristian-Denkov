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
            List<string> jsonColumnNames;

            if (userInput == null || userInput == "" )
            {
                return null;
            }
            else
            {
                var str = userInput.ToLower().Replace(" ", "");
                jsonColumnNames = str.Split(",").ToList();

            }
            return jsonColumnNames;
        }
    }
}
