using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Termis_Service.Models
{
    public class ColumnIndexSettings : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("MonthColumnIndex", IsRequired = true)]
        public int MonthColumnIndex
        {
            get { return (int)this["MonthColumnIndex"]; }
            set { this["MonthColumnIndex"] = value; }
        }

        [ConfigurationProperty("DayColumnIndex", IsRequired = true)]
        public int DayColumnIndex
        {
            get { return (int)this["DayColumnIndex"]; }
            set { this["DayColumnIndex"] = value; }
        }

        [ConfigurationProperty("HourColumnIndex", IsRequired = true)]
        public int HourColumnIndex
        {
            get { return (int)this["HourColumnIndex"]; }
            set { this["HourColumnIndex"] = value; }
        }

        [ConfigurationProperty("TempColumnIndex", IsRequired = true)]
        public int TempColumnIndex
        {
            get { return (int)this["TempColumnIndex"]; }
            set { this["TempColumnIndex"] = value; }
        }

        [ConfigurationProperty("SoilTempColumnIndex", IsRequired = true)]
        public int SoilTempColumnIndex
        {
            get { return (int)this["SoilTempColumnIndex"]; }
            set { this["SoilTempColumnIndex"] = value; }
        }

        [ConfigurationProperty("HasSoilTempColumn", IsRequired = true)]
        public bool HasSoilTempColumn
        {
            get { return (bool)this["HasSoilTempColumn"]; }
            set { this["HasSoilTempColumn"] = value; }
        }
    }
}
