#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2.Classes
{
    public class Odit
    {
        public Odit(Dictionary<string, string> cols)
        {
            this.cols = cols;
        }

        public Dictionary<string, string> cols { get; set;}

    }
}
#pragma warning restore CS8601, CS8604