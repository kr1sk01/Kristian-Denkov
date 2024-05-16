using ChampionshipMaster.SHARED.ViewModels;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

namespace ChampionshipMaster.Web.Components.Pages.User.Account;

public partial class ChangePassword : ComponentBase
{
    Variant variant = Variant.Outlined;

    [Inject] IConfiguration configuration { get; set; } = default!;
    [Inject] ITokenService tokenService { get; set; } = default!;
    [Inject] IHttpClientFactory httpClient { get; set; } = default!;
    [Inject] INotifier notifier { get; set; } = default!;

    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;

    private ChangePasswordViewModel model = new ChangePasswordViewModel();
    private string? errorMessage;
    private bool showError = false;
    bool hidePassword = true;
    bool hideNewPassword = true;
    bool hideConfirmPassword = true;
    bool loggedIn;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await tokenService.ValidateToken())
            {
                loggedIn = true;
                StateHasChanged();
            }
            else
            {
                NavigationManager.NavigateTo("/login");
            }
        }
    }

    async Task OnSubmit(ChangePasswordViewModel changePassword)
    {
        using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");
        var request = await client.PostAsJsonAsync("api/Player/changePassword", changePassword);

        if (!request.IsSuccessStatusCode)
        {
            var responseBody = await request.Content.ReadAsStringAsync();
            errorMessage = responseBody;
            showError = true;
            return;
        }

        notifier.SendSuccessNotification("Changed password successfully!");
        NavigationManager.NavigateTo("/profile");
    }

    void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        showError = false;
    }
    void TogglePassword()
    {
        hidePassword = !hidePassword;
    }
    void ToggleNewPassword()
    {
        hideNewPassword = !hideNewPassword;
    }
    void ToggleConfirmPassword()
    {
        hideConfirmPassword = !hideConfirmPassword;
    }
}
