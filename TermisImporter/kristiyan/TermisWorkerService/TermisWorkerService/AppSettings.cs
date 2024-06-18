using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService;
public class AppSettings
{
    // Parameterless constructor
    public AppSettings() { }
    public string FolderPath { get; set; } = default!;
    public string FailedFolderPath { get; set; } = default!;
    public string SucceededFolderPath { get; set; } = default!;
}
