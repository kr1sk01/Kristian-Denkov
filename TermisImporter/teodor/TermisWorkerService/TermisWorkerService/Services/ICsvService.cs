using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Services
{
    public interface ICsvService
    {
        void ProcessCsvFile(string filePath);
    }
}
