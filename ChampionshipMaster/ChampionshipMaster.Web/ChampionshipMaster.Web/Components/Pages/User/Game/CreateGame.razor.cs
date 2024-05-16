using ChampionshipMaster.SHARED.DTO;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

namespace ChampionshipMaster.Web.Components.Pages.User.Game
{
    public partial class CreateGame
    {
        [Inject] ITokenService tokenService { get; set; } = default;
        [Inject] IHttpClientFactory httpClient { get; set; } = default;
        [Inject] IWebHostEnvironment Environment { get; set; } = default;
        [Inject] IConfiguration configuration { get; set; } = default;
        [Inject] INotifier notifier { get; set; } = default;

        [Inject] ProtectedLocalStorage _localStorage { get; set; } = default;
        [Inject] NavigationManager NavigationManager { get; set; } = default;

        private Variant variant = Variant.Outlined;
        private bool isLogged = false;
        public GameDto game = new GameDto();
        private List<GameTypeDto>? gameTypes = new List<GameTypeDto>();

        private bool isFirstRender = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && isFirstRender)
            {
                isFirstRender = false;

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
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");

                    gameTypes = await client.GetFromJsonAsync<List<GameTypeDto>>("api/GameTypes");
                    if (gameTypes == null || gameTypes.Count == 0)
                    {
                        notifier.SendErrorNotification("Couldn't retrieve game types!");
                        NavigationManager.NavigateTo("/");
                        return;
                    }

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    notifier.SendErrorNotification($"Error: {ex.Message}");
                    NavigationManager.NavigateTo("/error");
                }
            }
        }

        // Your submit logic here
        public void OnSubmit()
        {
            notifier.SendSuccessNotification("Game created successfully!");
        }

        // Your invalid submit logic here
        public void OnInvalidSubmit()
        {
            notifier.SendErrorNotification("Please correct the errors and try again.");
        }
    }
}
