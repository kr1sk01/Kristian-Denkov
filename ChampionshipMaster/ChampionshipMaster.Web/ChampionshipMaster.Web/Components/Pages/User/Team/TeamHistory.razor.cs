using Blazorise;

namespace ChampionshipMaster.Web.Components.Pages.User.Team
{
    public partial class TeamHistory : ComponentBase
    {
        [Inject] ITokenService tokenService { get; set; } = default!;
        [Inject] IHttpClientFactory httpClient { get; set; } = default!;
        [Inject] INotifier notifier { get; set; } = default!;
        [Inject] IConfiguration configuration { get; set; } = default!;
        [Inject] IImageService imageService { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public string id { get; set; } = default!;
        [Parameter] public bool fromHomePage { get; set; } = false;

        public TeamDto currentTeam = new();

        public List<GameDto>? games;

        public bool isAdmin = false;
        public bool isLogged = false;
        class DataItem
        {
            public string Category { get; set; } = default!;
            public double Count { get; set; } = default!;
            public string Color { get; set; } = default!;
        }

        DataItem[] data = new DataItem[] {
                    new DataItem
                    {
                        Category = "Wins",
                        Count = 0,
                        Color = "#00FF00"
                    },
                    new DataItem
                    {
                        Category = "Loses",
                        Count = 0,
                        Color = "#FF0000"
                    },
                };

        string[] fills = { "#00FF00", "#FF0000" }; // Red, Green, Blue

        public int wins { get; set; }
        public int loses { get; set; }
        public double winratio { get; set; }
        public int gamesPlayed { get; set; }
        public double winrationPercentage { get; set; }

        TeamDto team = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (await tokenService.ValidateToken())
                {
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());
                    if (await tokenService.ValidateToken(true)) isAdmin = true;

                    StateHasChanged();
                }
                else { NavigationManager.NavigateTo("/login"); }

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                if (!fromHomePage)
                {
                    var tempTeam = await client.GetFromJsonAsync<TeamDto>($"/api/Teams/{id}");
                    if (tempTeam != null)
                        team = tempTeam;
                }
                else
                {
                    var tempTeam = await client.GetFromJsonAsync<TeamDto>($"api/Teams/bestTeam");
                    if(tempTeam != null)
                        team = tempTeam;
                }
                
                if (team != null)
                    currentTeam = team;
                var test = await client.GetFromJsonAsync<List<GameDto>>($"/api/Teams/Game_History/{id}");
                
                if (test != null)
                    games = test;

                if (games != null)
                    foreach (var item in games)
                    {
                        if (item.GameStatusName == "Finished" && item.Winner != null)
                        {
                            
                            if (item.Winner.Id == int.Parse(id)) wins++;
                            if (item.BlueTeam!.Id == int.Parse(id) || item.RedTeam!.Id == int.Parse(id)) gamesPlayed++;

                            loses = gamesPlayed - wins;
                            data[0].Count = wins;
                            data[1].Count = loses;
                        }
                        if (gamesPlayed > 0)
                        {
                            winrationPercentage = (double)(((double)wins / (double)gamesPlayed) * 100);
                        }
                    }


                ;
                StateHasChanged();
            }
        }
    }
}
