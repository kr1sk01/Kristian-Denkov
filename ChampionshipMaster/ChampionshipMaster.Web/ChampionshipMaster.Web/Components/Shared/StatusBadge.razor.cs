namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class StatusBadge : ComponentBase
    {
        [Parameter] public required string Status { get; set; }
        [Parameter] public string? Style { get; set; }

        private string Icon => Status switch
        {
            "Coming" => "schedule",
            "Open" => "door_open",
            "Live" => "live_tv",
            "Finished" => "check_circle",
            "Cancelled" => "cancel",
            _ => "help_outline"
        };

        private string StatusClass => Status switch
        {
            "Coming" => "status-coming",
            "Open" => "status-open",
            "Live" => "status-live",
            "Finished" => "status-finished",
            "Cancelled" => "status-cancelled",
            _ => ""
        };
    }
}
