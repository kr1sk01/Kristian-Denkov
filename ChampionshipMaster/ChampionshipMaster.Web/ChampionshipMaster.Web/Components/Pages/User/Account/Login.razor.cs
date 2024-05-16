using ChampionshipMaster.SHARED.ViewModels;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.User.Account
{
    public partial class Login : ComponentBase
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
        ProtectedLocalStorage _localStorage { get; set; } = default!;

        Variant variant = Variant.Outlined;

        private LoginViewModel model = new LoginViewModel();
        private string? errorMessage;
        private bool showError = false;

        private bool isLogged = false;

        bool hidePassword = true;

        async Task OnSubmit(LoginViewModel login)
        {
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var request = await client.PostAsJsonAsync("api/Player/login", login);

            if (!request.IsSuccessStatusCode)
            {
                var responseBody = await request.Content.ReadAsStringAsync();
                errorMessage = responseBody;
                showError = true;
                return;
            }

            var successfulLogin = await request.Content.ReadAsStringAsync();
            var jwtToken = JsonSerializer.Deserialize<Dictionary<string, string>>(successfulLogin)!["jwtToken"];
            var image = JsonSerializer.Deserialize<Dictionary<string, string>>(successfulLogin)!["image"];

            await _localStorage.SetAsync("jwtToken", jwtToken);
            await _localStorage.SetAsync("playerAvatar", image);
            isLogged = true;
            StateHasChanged();
            notifier.SendSuccessNotification("Logged in successfully!");
            NavigationManager.NavigateTo("/", forceLoad: true);
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            showError = false;
        }
        void TogglePassword()
        {
            hidePassword = !hidePassword;
        }
    }
}
