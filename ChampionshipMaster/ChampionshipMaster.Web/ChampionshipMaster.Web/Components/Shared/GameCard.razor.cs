
namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class GameCard : ComponentBase
    {
        [Parameter] public required GameDto Game { get; set; }
        [Parameter] public string? Style { get; set; }
    }
}
