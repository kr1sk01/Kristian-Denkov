
namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class GameCard : ComponentBase
    {
        [Inject] IImageService imageService { get; set; } = default!;
        [Parameter] public required GameDto Game { get; set; }
        [Parameter] public string? Style { get; set; }

        string? blueTeamName;
        string blueTeamLogoPath = string.Empty;
        string? redTeamName;
        string redTeamLogoPath = string.Empty;

        protected override void OnInitialized()
        {
            blueTeamName = Game.BlueTeam?.Name;
            redTeamName = Game.RedTeam?.Name;

            if (Game.BlueTeam?.Logo == null || Game.BlueTeam.Logo.Length == 0)
            {
                blueTeamLogoPath = "images/defaultTeam.png";
            }
            else
            {
                string base64image = Convert.ToBase64String(Game.BlueTeam.Logo);
                var imageType = imageService.GetImageFileType(base64image);
                blueTeamLogoPath = $"data:image/{imageType};base64,{base64image}";
            }

            if (Game.RedTeam?.Logo == null || Game.RedTeam.Logo.Length == 0)
            {
                redTeamLogoPath = "images/defaultTeam.png";
            }
            else
            {
                string base64image = Convert.ToBase64String(Game.RedTeam.Logo);
                var imageType = imageService.GetImageFileType(base64image);
                redTeamLogoPath = $"data:image/{imageType};base64,{base64image}";
            }

            base.OnInitialized();
        }
    }
}
