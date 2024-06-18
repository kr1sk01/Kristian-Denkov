using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService;
public class ColumnIndexes
{
    // Parameterless constructor
    public ColumnIndexes() { }
    public int MonthColumnIndex { get; set; } = -1;
    public int DateColumnIndex { get; set; } = -1;
    public int HourColumnIndex { get; set; } = -1;
    public int EarthTempColumnIndex { get; set; } = -1;
    public int AirTempColumnIndex { get; set; } = -1;
}
