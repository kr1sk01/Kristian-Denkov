using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2.Classes
{
    public class JsonCell
    {
        public string? Name { get; set; }
        public object? OriginalValue { get; set; }
        public object? CurrentValue { get; set; }
        public object? Value { get; set; }
    }
}
