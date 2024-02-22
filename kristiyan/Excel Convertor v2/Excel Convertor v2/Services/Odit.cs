#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2
{
    public class Odit
    {
        public Odit(string col1, string col2, string col3, string col4, string col5)
        {
            this.col1 = col1;
            this.col2 = col2;
            this.col3 = col3;
            this.col4 = col4;
            this.col5 = col5;
        }

        public string? col1 { get; set; }
        public string? col2 { get; set; }
        public string? col3 { get; set; }
        public string? col4 { get; set; }
        public string? col5 { get; set; }

    }
}
#pragma warning restore CS8601, CS8604