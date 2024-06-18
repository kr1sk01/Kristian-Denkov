using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService;
public class EmailSettings
{
    // Parameterless constructor
    public EmailSettings() { }
    public string FromAddress { get; set; } = default!;
    public string ToAddress { get; set; } = default!;
    public string Host { get; set; } = default!;
    public int Port { get; set; } = default!;
    public bool EnableSsl { get; set; } = default!;
    public string FromPassword { get; set; } = default!;
}