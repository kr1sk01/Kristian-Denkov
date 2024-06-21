using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Helper
{
    public static class PathHelper
    {
        public static string CorrectPath(string path)
        {
            // Check if path already contains double backslashes
            if (path.Contains("\\\\"))
            {
                return path; // Path is already correctly formatted
            }

            // Replace single backslashes with double backslashes
            return path.Replace("\\", "\\\\");
        }
    }
}
