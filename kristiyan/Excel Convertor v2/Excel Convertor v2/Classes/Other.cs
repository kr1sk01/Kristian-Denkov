#pragma warning disable CS8601,CS8604
using System.Collections.Generic;

namespace Excel_Convertor_v2.Classes
{
    public class Other : Odit
    {
        public Other(Dictionary<string, string> cols, Dictionary<string, object> data) : base(cols)
        {
            this.data = data;
        }
        public Dictionary<string, object> data = new Dictionary<string, object>();

        
    }
}
#pragma warning restore CS8601, CS8604