#pragma warning disable CS8601,CS8604
using System.Collections.Generic;

namespace Excel_Convertor_v2.Classes
{
    public class Other : Odit
    {
        public Other(List<Dictionary<string, string>> cols, Dictionary<string, string> data) : base(cols)
        {
            this.data = data;
        }
        Dictionary<string, string> data = new Dictionary<string, string>();

        
    }
}
#pragma warning restore CS8601, CS8604