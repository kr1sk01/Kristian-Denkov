#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2.Classes
{
    public class Other : Odit
    {
        public Other(string col1, string col2, string col3, string col4, string col5, Dictionary<string, string> data) : base(col1, col2, col3, col4, col5)
        {
            base.col1 = col1;
            base.col2 = col2;
            base.col3 = col3;
            base.col4 = col4;
            base.col5 = col5;
            this.data = data;
        }
        Dictionary<string, string> data = new Dictionary<string, string>();
    }
}
#pragma warning restore CS8601, CS8604