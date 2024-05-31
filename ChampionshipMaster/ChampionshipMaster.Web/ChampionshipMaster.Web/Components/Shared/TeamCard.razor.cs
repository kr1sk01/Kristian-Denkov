using static System.Net.Mime.MediaTypeNames;

namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class TeamCard : ComponentBase
    {
        [Inject] IImageService imageService { get; set; } = default!;

        [Parameter] public required TeamDto Team { get; set; }
        [Parameter] public string? Style { get; set; }
        [Parameter] public EventCallback<TeamDto> OnClick { get; set; }

        string teamImagePath = string.Empty;

        protected override void OnInitialized()
        {
            if (Team.Logo == null || Team.Logo.Length == 0)
            {
                teamImagePath = "images/defaultTeam.png";
            }
            else
            {
                string base64image = Convert.ToBase64String(Team.Logo);
                var imageType = imageService.GetImageFileType(base64image);
                teamImagePath = $"data:image/{imageType};base64,{base64image}";
            }

            base.OnInitialized();
        }

        async Task OnCardClick()
        {
            await OnClick.InvokeAsync(Team);
        }

        string GetPlayerImagePath(PlayerDto player)
        {
            if(player == null)
            {
                return "";
            }
            if (player.Avatar == null || player.Avatar.Length == 0)
            {
                return "images/defaultAvatar.png";
            }
            else
            {
                string base64image = Convert.ToBase64String(player.Avatar);
                var imageType = imageService.GetImageFileType(base64image);
                return $"data:image/{imageType};base64,{base64image}";
            }
        }
    }
}
