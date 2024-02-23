#pragma warning disable CS8601,CS8604
using System.Text.Json;

namespace Excel_Convertor_v2.Classes
{
    public class Odit
    {
        public Odit(Dictionary<string, string> cols, string jsonString)
        {
            this.cols = cols;
            this.DeserializeJson(jsonString);
        }

        public Dictionary<string, string> cols { get; set;}

        public List<Test>? JsonColumns { get; set;}

        public void DeserializeJson(string jsonString)
        {
            this.JsonColumns = JsonSerializer.Deserialize<List<Test>>(jsonString);
        }
    }
}
#pragma warning restore CS8601, CS8604