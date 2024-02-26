using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2.Classes
{
    public class TableRow
    {
        public List<TableColumn> Columns { get; set; } = new();
    }
}
