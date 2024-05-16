namespace ChampionshipMaster.Web.Components.Pages.User.Game
{
    public partial class CreateGame : ComponentBase
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

        Variant variant = Variant.Outlined;
        bool isLogged = false;
        GameDto game;
        List<GameTypeDto>? gameTypes = new List<GameTypeDto>();

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

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");

                gameTypes = await client.GetFromJsonAsync<List<GameTypeDto>>("api/GameTypes");
                if (gameTypes == null || gameTypes.Count == 0)
                {
                    notifier.SendErrorNotification("Couldn't retrieve game types!");
                    NavigationManager.NavigateTo("/");
                }

                StateHasChanged();
            }
        }

        public void OnSubmit()
        {

        }

        public void OnInvalidSubmit()
        {

        }
    }
}
