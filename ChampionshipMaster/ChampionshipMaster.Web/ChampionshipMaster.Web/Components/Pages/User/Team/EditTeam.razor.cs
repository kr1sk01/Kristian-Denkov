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
        NavigationManager NavigationManager { get; set; } = default!;

        [Parameter] public string id { get; set; }

        bool isValueInitial = true;
        bool isLogged = false;
        ChangeTeamMembers changeTeamMembers;
        ChangeName changeTeamName;
        ImageUpload changeTeamLogo;

        string nameRequestUrl = "api/Teams/changeTeamName";
        string logoRequestUrl = "api/Teams/changeTeamLogo";
        string teamMembersRequestUrl = "api/Teams/setPlayers";

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
                    changeTeamName.SetInitialValue(team.Name!);
                    nameRequestUrl += $"?teamId={id}";

                    changeTeamLogo.UpdateDisplayedImagePath(Convert.ToBase64String(team.Logo ?? new byte[0]));
                    logoRequestUrl += $"?teamId={id}";

                    teamMembersRequestUrl += $"?teamId={id}";
                    StateHasChanged();
                }
            }
        }

        public void CheckButtonState()
        {
            isValueInitial = changeTeamName.isValueInitial && changeTeamLogo.isValueInitial && changeTeamMembers.isValueInitial;
        }

        public async Task OnClick()
        {
            await changeTeamName.OnClick();
            await changeTeamLogo.UploadImage();
            await changeTeamMembers.SetTeamMembers();
            NavigationManager.NavigateTo("/manageteams");
        }
    }
}
