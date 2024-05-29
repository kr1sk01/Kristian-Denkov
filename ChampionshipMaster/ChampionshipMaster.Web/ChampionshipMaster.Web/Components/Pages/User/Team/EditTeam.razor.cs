using ChampionshipMaster.Web.Services;

namespace ChampionshipMaster.Web.Components.Pages.User.Team
{
    public partial class EditTeam : ComponentBase
    {
        [Inject]
        ITokenService tokenService { get; set; } = default!;
        [Inject]
        IHttpClientFactory httpClient { get; set; } = default!;
        [Inject]
        INotifier notifier { get; set; } = default!;
        [Inject]
        IConfiguration configuration { get; set; } = default!;
        [Inject]
        IImageService imageService { get; set; } = default!;
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        [Parameter] public string id { get; set; }

        bool isValueInitial = true;
        bool isLogged = false;
        ChangeTeamMembers changeTeamMembers;
        ChangeName changeTeamName;
        ImageUpload changeTeamLogo;
        TeamDto editedTeam = new();
        string requestUrl = "api/Teams";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendWarningNotification("Your session has ran out or you're not logged in");
                    NavigationManager.NavigateTo("/login");
                }

                isLogged = true;
                StateHasChanged();

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);

                var team = await client.GetFromJsonAsync<TeamDto>($"api/Teams/{id}");
                if (team != null)
                {
                    editedTeam.Id = team.Id;

                    changeTeamName.SetInitialValue(team.Name!);

                    changeTeamLogo.UpdateDisplayedImagePath(Convert.ToBase64String(team.Logo ?? new byte[0]));

                    requestUrl += $"?teamId={id}";
                    StateHasChanged();
                }
            }
        }

        public async Task CheckButtonState()
        {
            isValueInitial = changeTeamName.IsValueInitial && changeTeamLogo.IsValueInitial && changeTeamMembers.IsValueInitial;

            editedTeam.Name = changeTeamName.IsValueInitial ? null : changeTeamName.CurrentValue;
            editedTeam.Logo = changeTeamLogo.IsValueInitial ? null : Convert.FromBase64String(await imageService.ConvertToBase64String(changeTeamLogo.UploadedImage!));

            if (!changeTeamMembers.IsValueInitial)
            {
                foreach (var player in changeTeamMembers.selectedPlayers)
                {
                    if (player != null) { editedTeam.Players.Add(player); }
                }
            }
        }

        public async Task OnClick()
        {
            if (!await tokenService.ValidateToken())
            {
                notifier.SendInformationalNotification("You're not logged in or your session has expired");
                NavigationManager.NavigateTo("/login");
            }

            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = await client.PutAsJsonAsync(requestUrl, editedTeam);
            var body = await request.Content.ReadAsStringAsync();

            if (request.IsSuccessStatusCode)
            {
                notifier.SendSuccessNotification(body);
                NavigationManager.NavigateTo("/manageteams");
            }
            else
            {
                notifier.SendErrorNotification(body);
            }
        }
    }
}
