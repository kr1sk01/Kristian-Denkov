using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Services
{
    public interface IEmailService
    {
        void SendErrorEmail(string csvFile, List<string> errorList);
    }
}
