using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.Web.Components.Pages.User.Championship;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.Admin.Championship
{
    public partial class EditChampionship : ComponentBase
    {
        [Inject] NavigationManager nManager { get; set; } = default!;
        [Inject] DialogService dialogService { get; set; } = default!;
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

        RadzenDataGrid<TeamDto> datagrid = default!;

        public ChampionshipDto? currentChampionship;

        IList<TeamDto>? selectedTeam;
        private List<TeamDto>? teams;
        private List<TeamDto>? teamToShow;
        string playerId = "";
        private string role = "";
        bool isAdmin = false;

        private bool disabledDelete = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (await tokenService.ValidateToken(true))
                {
                    isAdmin = true;
                }
                else
                {
                    notifier.SendErrorNotification("Access denied!");
                    notifier.SendWarningNotification("If you believe there is an error, please contact administrator!", 10);
                    NavigationManager.NavigateTo("/");
                }
                await GetTeamsData();
                selectedTeam = null;
                StateHasChanged();
            }
        }
        private async Task GetTeamsData()
        {
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");
            var test = await client.GetFromJsonAsync<ChampionshipDto>($"api/Championship/{championshipId}");
            if(test == null)
            {
                notifier.SendWarningNotification("Coudn't retreive information about this championship!");
            }

            var test2 = test.Teams.ToList();
            currentChampionship = test;

            teams = test2!;

            int i = 0;
            
            foreach (var team in teams)
            {
                if (team.CreatedOn != null)
                {
                    teams[i].CreatedOn = team.CreatedOn.Value.ToLocalTime();
                }

                i++;
            }

            var jsonString = JsonSerializer.Serialize(teams!.Select(x => x.CreatedBy).ToList());
            var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            var request = await client.PostAsync("api/Player/getPlayersById", content);
            var body = await request.Content.ReadAsStringAsync();

            if (request.IsSuccessStatusCode)
            {
                var playerUsernames = JsonSerializer.Deserialize<List<string>>(body);

                i = 0;
                foreach (var team in teams)
                {
                    if (team.CreatedBy != null)
                    {
                        teams[i].CreatedByUsername = playerUsernames[i];
                    }

                    i++;
                }
            }
            StateHasChanged();
        }
        void Update(DataGridRowMouseEventArgs<TeamDto> args)
        {
            if (args.Data != null)
                disabledDelete = false;
            else
                disabledDelete = true;

            StateHasChanged();
        }
        async Task Delete(string championshipId,string teamId) 
        {
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");

            var result = await client.DeleteAsync($"api/ChampionshipTeams/delete/{championshipId}/{teamId}");
            if (result.IsSuccessStatusCode)
            {
                
                if (teams != null)
                    teams.RemoveAll(x => x.Id == int.Parse(teamId));

                RefreshData();
            }
            else
            {
                notifier.SendErrorNotification("Something went wrong!");
            }

            StateHasChanged();
        }
        public async Task OpenJoinDialog(string championshipId, string championshipName) 
        {
            var dialogService = await DialogService.OpenAsync<JoinChampionship>($"",
                       new Dictionary<string, object>() {
                           { "championshipId", championshipId },
                           { "championshipName",  championshipName},
                           { "dontredirect", true },
                           { "OnDialogClosed", EventCallback.Factory.Create(this, RefreshData) }
                       },
                       new DialogOptions() { Width = "45%", Height = "60%", Draggable = true, CloseDialogOnEsc = true });
        }
        private async Task RefreshData()
        {
            await GetTeamsData();
            await datagrid.Reload();
        }
    }
}
