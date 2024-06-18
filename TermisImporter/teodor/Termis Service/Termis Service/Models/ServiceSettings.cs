using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Termis_Service.Models
{
    public class ServiceSettings : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("DatabaseName", IsRequired = true)]
        public string DatabaseName
        {
            get { return (string)this["DatabaseName"]; }
            set { this["DatabaseName"] = value; }
        }

        [ConfigurationProperty("CsvDirectory", IsRequired = true)]
        public string CsvDirectory
        {
            get { return (string)this["CsvFolder"]; }
            set { this["CsvFolder"] = value; }
        }

        [ConfigurationProperty("ProcessedDirectory", IsRequired = true)]
        public string ProcessedDirectory
        {
            get { return (string)this["ProcessedDirectory"]; }
            set { this["ProcessedDirectory"] = value; }
        }

        [ConfigurationProperty("ErrorDirectory", IsRequired = true)]
        public string ErrorDirectory
        {
            get { return (string)this["ErrorDirectory"]; }
            set { this["ErrorDirectory"] = value; }
        }

        [ConfigurationProperty("CsvSeparator", IsRequired = true)]
        public char CsvSeparator
        {
            get { return (char)this["CsvSeparator"]; }
            set { this["CsvSeparator"] = value; }
        }
    }
}
