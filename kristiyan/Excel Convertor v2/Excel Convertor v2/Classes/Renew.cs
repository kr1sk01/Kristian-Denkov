#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2.Classes
{
    public class Renew : Odit
    {
        public Renew(Dictionary<string, string> cols, RenewJSON? rJson) : base(cols)
        {          
            this.rJson = rJson;
        }
        public RenewJSON? rJson { get; set; }

    }
}
#pragma warning restore CS8601, CS8604