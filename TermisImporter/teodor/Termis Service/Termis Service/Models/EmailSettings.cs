using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Termis_Service.Models
{
    public class EmailSettings : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("Host", IsRequired = true)]
        public string Host
        {
            get { return (string)this["Host"]; }
            set { this["Host"] = value; }
        }

        [ConfigurationProperty("Port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("Username", IsRequired = true)]
        public string Username
        {
            get { return (string)this["Username"]; }
            set { this["Username"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("EnableSsl", IsRequired = true)]
        public bool EnableSsl
        {
            get { return (bool)this["EnableSsl"]; }
            set { this["EnableSsl"] = value; }
        }

        [ConfigurationProperty("ToEmail", IsRequired = true)]
        public string ToEmail
        {
            get { return (string)this["ToEmail"]; }
            set { this["ToEmail"] = value; }
        }

        [ConfigurationProperty("DisplayName", IsRequired = true)]
        public string DisplayName
        {
            get { return (string)this["DisplayName"]; }
            set { this["DisplayName"] = value; }
        }
    }
}
