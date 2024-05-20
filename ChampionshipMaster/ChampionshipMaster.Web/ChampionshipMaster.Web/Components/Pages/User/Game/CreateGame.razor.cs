using ChampionshipMaster.SHARED.DTO;
using ChampionshipMaster.Web.Services;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;
using System.Net.Http.Json;

namespace ChampionshipMaster.Web.Components.Pages.User.Game
{
    public partial class CreateGame
    {
        [Inject] ITokenService tokenService { get; set; } = default!;
        [Inject] IHttpClientFactory httpClient { get; set; } = default!;
        [Inject] IWebHostEnvironment Environment { get; set; } = default!;
        [Inject] IConfiguration configuration { get; set; } = default!;
        [Inject] INotifier notifier { get; set; } = default!;
        [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;

        private bool isLogged = false;
        public GameDto game = new();
        private List<GameTypeDto>? gameTypes = new List<GameTypeDto>();
        private RadzenDropDown<GameTypeDto> gameTypeDropDown = new();
        private List<RadzenDropDown<int>> teamDropDowns =
        [
            new RadzenDropDown<int>(),
            new RadzenDropDown<int>()
        ];

        private List<TeamDto>? allTeams = new List<TeamDto>();
        private List<TeamDto> selectableTeams = new List<TeamDto>();
        private List<TeamDto?> selectedTeams = new List<TeamDto?>()
        {
            null, null
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    if (!await tokenService.ValidateToken())
                    {
                        notifier.SendWarningNotification("Your session has run out or you're not logged in");
                        NavigationManager.NavigateTo("/login");
                        return;
                    }

                    isLogged = true;

                    using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetToken());

                    gameTypes = await client.GetFromJsonAsync<List<GameTypeDto>>("api/GameTypes");
                    if (gameTypes == null || gameTypes.Count == 0)
                    {
                        notifier.SendErrorNotification("Couldn't retrieve game types!");
                        NavigationManager.NavigateTo("/");
                        return;
                    }

                    allTeams = await client.GetFromJsonAsync<List<TeamDto>>("api/Teams");
                    if (allTeams == null || allTeams.Count == 0)
                    {
                        notifier.SendErrorNotification("Couldn't retrieve teams!");
                        NavigationManager.NavigateTo("/");
                        return;
                    }

                    selectableTeams.AddRange(allTeams);

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    notifier.SendErrorNotification($"Error: {ex.Message}");
                    NavigationManager.NavigateTo("/error");
                }
            }

            if (selectedTeams.Count != 0)
            {
                int i = 0;
                foreach (var selectedTeam in selectedTeams)
                {
                    if (selectedTeam != null)
                    {
                        await teamDropDowns[i].SelectItem(selectedTeam, raiseChange: false);
                        teamDropDowns[i].Value = selectedTeam.Id;
                    }

                    i++;
                }
            }
        }

        // Your submit logic here
        public async Task OnSubmit()
        {
            if (teamDropDowns[0].SelectedItem == null || teamDropDowns[1].SelectedItem == null)
            {
                notifier.SendErrorNotification("There are fields you haven't filled.");
                return;
            }

            game.BlueTeamName = teamDropDowns[0].SelectedItem.Adapt<TeamDto>().Name;
            game.RedTeamName = teamDropDowns[1].SelectedItem.Adapt<TeamDto>().Name;
            game.GameTypeName = gameTypeDropDown.SelectedItem.Adapt<GameTypeDto>().Name;

            if (!await tokenService.ValidateToken())
            {
                notifier.SendInformationalNotification("You're not logged in or your session has expired");
                NavigationManager.NavigateTo("/login");
            }

            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = await client.PostAsJsonAsync("api/Game", game);

            if (request.IsSuccessStatusCode)
            {
                notifier.SendSuccessNotification("Game created successfuly");
                NavigationManager.NavigateTo("/");
            }
            else
            {
                var body = await request.Content.ReadAsStringAsync();
                notifier.SendErrorNotification(body);
            }
        }

        // Your invalid submit logic here
        public void OnInvalidSubmit()
        {
            notifier.SendErrorNotification("Please correct the errors and try again.");
        }

        public async Task OnGameTypeSelect(GameTypeDto args)
        {
            foreach(var teamDropDown in teamDropDowns)
            {
                await teamDropDown.SelectItem(null, raiseChange: false);
                teamDropDown.Value = null;
            }

            selectableTeams = allTeams!.Where(x => x.TeamTypeName == args.TeamTypeName).ToList();
            for (int i = 0; i < selectedTeams.Count; i++)
            {
                selectedTeams[i] = null;
            }
            StateHasChanged();
        }

        public void OnTeamSelect(int args, int dropDownIndex)
        {
            var selectedTeam = selectableTeams.FirstOrDefault(x => x.Id == args);
            if (selectedTeam == null)
            {
                return;
            }
            selectableTeams.RemoveAll(x => x.Id == args);

            if (teamDropDowns[dropDownIndex].Value != null)
            {
                var deselectedTeam = allTeams!.FirstOrDefault(x => x.Id == (int)teamDropDowns[dropDownIndex].Value);
                selectableTeams.Add(deselectedTeam!);
            }

            selectedTeams[dropDownIndex] = selectedTeam;

            teamDropDowns[dropDownIndex].Value = selectedTeam.Id;
        }

        public void DateRender(DateRenderEventArgs args)
        {
            args.Disabled = args.Disabled || ( args.Date < DateTime.UtcNow.Date);
        }

        public async Task OnResetClicked()
        {
            await gameTypeDropDown.SelectItem(null, raiseChange: false);
            StateHasChanged();
        }
    }
}
