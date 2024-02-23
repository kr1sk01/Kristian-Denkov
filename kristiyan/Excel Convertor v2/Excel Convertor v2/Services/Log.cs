using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2.Services
{
    public class Log
    {
        public static void LogExecutionTime(Stopwatch sWatch)
        {
            string logFilePath = "executionTimeLog.log";

            // Write the exception details to a log file
            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine($"[{DateTime.Now}] Execution time: {sWatch.Elapsed}");
            }
        }
        public static void LogException(Exception ex)
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
