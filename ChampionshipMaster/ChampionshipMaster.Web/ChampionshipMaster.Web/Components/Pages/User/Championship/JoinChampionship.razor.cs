using ChampionshipMaster.DATA.Models;
using Mapster;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;

namespace ChampionshipMaster.Web.Components.Pages.User.Championship
{
    public partial class JoinChampionship : ComponentBase
    {
        [Inject] NavigationManager nManager { get; set; } = default!;
        [Inject] Radzen.DialogService dialogService { get; set; } = default!;
        [Inject] IConfiguration configuration { get; set; } = default!;
        [Inject] ITokenService tokenService { get; set; } = default!;
        [Inject] IHttpClientFactory httpClient { get; set; } = default!;
        [Inject] IWebHostEnvironment Environment { get; set; } = default!;
        [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ContextMenuService ContextMenuService { get; set; } = default!;
        [Inject] DialogService DialogService { get; set; } = default!;
        [Inject] INotifier notifier { get; set; } = default!;


        [Parameter] public string? championshipId { get; set; }
        [Parameter] public string? championshipName { get; set; }

        bool isLogged = false;
        bool isAdmin = false;

        public List<TeamDto>? allTeams;
        public List<TeamDto>? selectableTeams;
        public List<TeamDto> selectableTeamsToShow = new();
        public ChampionshipDto? currentChampionship;
        public ChampionshipTeamsDto ChampionshipTeamsToAdd = new();
        RadzenDropDown<int?> teamDropDown = default!;

        public bool zeroTeamAvaiableToAdd = false;

        string? playerId;
        string? playerRole;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendWarningNotification("Your session has run out or you're not logged in");
                    NavigationManager.NavigateTo("/login");
                    return;
                }

                isLogged = true;

                var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());
                playerId = token.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
                playerRole = token.Claims.FirstOrDefault(x => x.Type == "role")?.Value ?? "";

                if (playerRole.ToLower() == "admin")
                {
                    isAdmin = true;
                }

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetToken());

                allTeams = await client.GetFromJsonAsync<List<TeamDto>>("api/Teams/active");

                if (allTeams == null)
                {
                    notifier.SendWarningNotification("Couldn't retreive teams!");
                    NavigationManager.NavigateTo("/championshipsmain");
                    return;
                }

                currentChampionship = await client.GetFromJsonAsync<ChampionshipDto>($"api/Championship/{championshipId}");

                if(currentChampionship == null)
                {
                    notifier.SendWarningNotification("Couldn't retreive championship!");
                    NavigationManager.NavigateTo("/championshipsmain");
                    return;
                }

                selectableTeams = isAdmin ? allTeams : allTeams.Where(x => x.CreatedBy == playerId).ToList();
                if (currentChampionship.GameType != null)
                {
                    selectableTeams = selectableTeams.Where(x => x.TeamTypeName == currentChampionship.GameType.TeamTypeName).ToList();
                }
                

                if (selectableTeams == null)
                {
                    notifier.SendWarningNotification("In order to join teams in championship you must own at least one!");
                    return;
                }


                foreach (var item in selectableTeams)
                {
                    if (!currentChampionship.Teams.Any(x=>x.Id == item.Id))
                        selectableTeamsToShow.Add(item);
                }
                if(selectableTeamsToShow.Count == 0)
                {
                    zeroTeamAvaiableToAdd = true;
                    notifier.SendInformationalNotification("You don't have any team available to add! If you want to add new team, click on 'create team'!", 10);
                }
                StateHasChanged();

            }
        }

        public async Task DropDownSelect(object args)
        {
            if (args == null) 
            {
                return;
            }

            var selectedTeam = args.Adapt<TeamDto>();

            var repeatingPlayerWithTeam = (from player in selectedTeam.Players
                                             from team in currentChampionship!.Teams
                                             where team.Players.Any(p => p.Id == player.Id)
                                             select new { Player = player, Team = team }).ToList().FirstOrDefault();

            if (repeatingPlayerWithTeam != null)
            {
                var playerName = repeatingPlayerWithTeam.Player.Name;
                var teamName = repeatingPlayerWithTeam.Team.Name;
                notifier.SendWarningNotification("The team you selected contains a player who's already registered in this Championship." +
                    $"\n Player [{playerName}] in Team [{teamName}]", 8);

                await teamDropDown.SelectItem(null, raiseChange:false);
                ChampionshipTeamsToAdd.TeamId = null;
            }
        }

        public async Task OnSubmit()
        {
            if (ChampionshipTeamsToAdd != null && championshipId != null)
            {
                ChampionshipTeamsToAdd.ChampionshipId = int.Parse(championshipId);

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetToken());

                var request = await client.PostAsJsonAsync("api/Championship/join", ChampionshipTeamsToAdd);
                if (request.IsSuccessStatusCode)
                {
                    notifier.SendSuccessNotification("Team joined successfully!");
                    NavigationManager.NavigateTo("/championshipsmain");

                }
            }

        }
        void OnInvalidSubmit()
        {

        }
        void ForwardToCreateTeam(bool isRedirectedFromChampionship, string championshipId)
        {
            NavigationManager.NavigateTo($"/createteam/{isRedirectedFromChampionship}/{championshipId}");
        }
    }
}
