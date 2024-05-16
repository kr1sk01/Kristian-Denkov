namespace ChampionshipMaster.Web.Components.Pages.User.Championship
{
    public partial class ChampionShipDetails : ComponentBase
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
        [Inject]
        Radzen.DialogService dialogService { get; set; } = default!;

        [Parameter] public string? id { get; set; }

        RadzenDataGrid<GameDto>? gamesList;

        private string? data;

        private ChampionshipDto? championshipWithDetails;
        private IEnumerable<GameDto>? games;

        protected override async Task OnInitializedAsync()
        {

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var test = await client.GetFromJsonAsync<ChampionshipDto>($"api/Championship/{id}");
            if (test != null)
            {
                championshipWithDetails = test;
                games = championshipWithDetails.Games;
            }
        }
    }
}
