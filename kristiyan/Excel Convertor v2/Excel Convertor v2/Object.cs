using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2
{

    public class Object
    {
        /*
        public Object()
        public Object(string Name, object? Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public Object(string Name, object? Value, 
            object? OriginalValue, 
            object? CurrentValue)
        {
            this.Name = Name;
            this.Value = Value;
            this.OriginalValue = OriginalValue;
            this.CurrentValue = CurrentValue;
        }
        */
        public string? Name { get; set; } = "";
        public object? Value { get; set; } = "";
        public object? OriginalValue { get; set; } = "";
        public object? CurrentValue { get; set; } = "";
    }
}
