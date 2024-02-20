using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel_Convertor_v2
{
    public class Odit
    {
        public Odit(string DateTimeString, string UserType, string ActionType, string Object, string Identificator)
        {
            this.DateTimeString = DateTimeString;
            this.UserType = UserType;
            this.ActionType = ActionType;
            this.Object = Object;
            this.Identificator = Identificator;
            this.Objects = new List<Object>();
        }
        public Odit(string DateTimeString, string UserType, string ActionType, string Object, string Identificator, List<Object> Objects)
        {
            this.DateTimeString = DateTimeString;
            this.UserType = UserType;
            this.ActionType = ActionType;
            this.Object = Object;
            this.Identificator = Identificator; 
            this.Objects = Objects;
        }
        
        public string? DateTimeString { get; set; }
        public string? UserType { get; set; }
        public string? ActionType { get; set; }
        public string? Object { get; set; }
        public string? Identificator { get; set; }
        public List<Object>? Objects { get; set; }
    }
}
