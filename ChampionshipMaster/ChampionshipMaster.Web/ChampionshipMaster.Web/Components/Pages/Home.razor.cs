using ChampionshipMaster.Web.Components.Pages.User.Championship;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace ChampionshipMaster.Web.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ITokenService tokenService { get; set; } = default!;
        [Inject] IHttpClientFactory httpClient { get; set; } = default!;
        [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;
        [Inject] IConfiguration configuration { get; set; } = default!;
        private string username = "";

        private HubConnection? _hubConnection;

        private readonly List<string> _messages = new();

        TeamDto bestTeam = new TeamDto { Id = 1 };
        protected override async void OnInitialized()
        {
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);

            var team = await client.GetFromJsonAsync<TeamDto>($"api/Teams/bestTeam");

            if (team != null)
                bestTeam = team;

            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (await tokenService.ValidateToken())
                {
                    var tokenString = await tokenService.GetToken();
                    if (tokenString == null)
                    {
                        NavigationManager.NavigateTo("/login");
                    }
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

                    username = token.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value ?? "";

                    StateHasChanged();
                }
                else { NavigationManager.NavigateTo("/login"); }

                
            }
        }
        void openChampionship()
        {
            NavigationManager.NavigateTo("/championshipsmain");
        }
        void openCreateTeam()
        {
            NavigationManager.NavigateTo("/createteam");

        }
        void openRulesPage()
        {
            NavigationManager.NavigateTo("/info");
        }
    }
}
