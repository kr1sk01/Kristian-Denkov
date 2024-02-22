#pragma warning disable CS8601,CS8604
namespace Excel_Convertor_v2
{
    public class RenewJSON
    {
        public RenewJSON(string Name, string OriginalValue, string CurrentValue)
        {
            this.Name = Name;
            this.OriginalValue = OriginalValue;
            this.CurrentValue = CurrentValue;
        }
        public string? Name { get; set; }
        public string? OriginalValue { get; set; }
        public string? CurrentValue { get; set; }
    }
}
#pragma warning restore CS8601, CS8604