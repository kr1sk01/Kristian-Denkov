namespace ChampionshipMaster.Web.Components.Pages.User.Team
{
    public partial class TeamHistory : ComponentBase
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
        public List<GameDto>? games;

        public bool isAdmin = false;
        public bool isLogged = false;
        class DataItem
        {
            public string Category { get; set; }
            public double Count { get; set; }
            public string Color { get; set; }
        }

        DataItem[] data = new DataItem[] {
        new DataItem
        {
            Category = "Wins",
            Count = 21,
            Color = "#00FF00"
        },
        new DataItem
        {
            Category = "Loses",
            Count = 56,
            Color = "#FF0000"
        },
    };
        string[] fills = { "#00FF00", "#FF0000" }; // Red, Green, Blue
        public int wins { get; set; }
        public int loses { get; set; }
        public double winratio { get; set; }
        public int gamesPlayed { get; set; } 
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
                var test = await client.GetFromJsonAsync<List<GameDto>>($"/api/Teams/Game_History/{id}");
                if(test != null)               
                    games = test;


                ;
                StateHasChanged();
            }
        }
    }
}
