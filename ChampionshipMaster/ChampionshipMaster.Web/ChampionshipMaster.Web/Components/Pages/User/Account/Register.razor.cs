using ChampionshipMaster.SHARED.ViewModels;
using ChampionshipMaster.Web.Services;
using Radzen;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.User.Account
{
    public partial class Register : ComponentBase
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


        private RegisterViewModel model { get; set; } = new RegisterViewModel();
        private string? errorMessage;
        private bool showError = false;

        bool hidePassword = true;
        bool hideConfirmPassword = true;

        async Task OnSubmit(RegisterViewModel register)
        {
            if (model.ConfirmPassword == null)
            {
                ShowError("Confirm password is required!");
                return;
            }
            if (model.Password == null)
            {
                ShowError("Password is required!");
                return;
            }
            if (model.Password != null && (model.Password.Length < 8 || model.Password.Length > 100))
            {
                ShowError("Password must be between 8 and 100 symbols!");
                return;
            }
            if (model.Password != model.ConfirmPassword)
            {
                ShowError("Password doesn't match!"); return;
            }
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            register.CreatedOn = DateTime.UtcNow;
            register.CreatedBy = register.UserName;
            var request = await client.PostAsJsonAsync("api/Player/register", register);

            if (!request.IsSuccessStatusCode)
            {
                var responseBody = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(await request.Content.ReadAsStringAsync());
                errorMessage = responseBody!.First()["description"];
                showError = true;
                return;
            }


            notifier.SendSuccessNotification("Registered successfully!");
            notifier.SendInformationalNotification("In order to use the app you need to confirm your email and sing in!", 10);
            NavigationManager.NavigateTo("/login");
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            showError = false;
        }

        void TogglePassword()
        {
            hidePassword = !hidePassword;
        }
        void ToggleConfirmPassword()
        {
            hideConfirmPassword = !hideConfirmPassword;
        }
        void ShowError(string message)
        {

            showError = true;
            errorMessage = message;
            StateHasChanged();

        }
    }
}
