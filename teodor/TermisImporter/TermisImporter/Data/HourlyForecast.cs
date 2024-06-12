using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisImporter.Data
{
    [Table("HourlyForecasts")]
    public class HourlyForecast
    {
            [Key, Column(Order = 0)]
            public int Month { get; set; }

            [Key, Column(Order = 1)]
            public int Day { get; set; }

            [Key, Column(Order = 2)]
            public int Hour { get; set; }

            public double Temp { get; set; }

            public double SoilTemp { get; set; }
    }
}
