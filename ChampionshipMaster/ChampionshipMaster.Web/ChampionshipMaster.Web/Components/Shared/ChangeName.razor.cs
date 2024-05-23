using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class ChangeName : ComponentBase
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

        [Parameter]
        public EventCallback StateChange { get; set; }
        [Parameter]
        public required string Title { get; set; }
        [Parameter]
        public string? InitialValue { get; set; }
        [Parameter]
        public required string RequestUrl { get; set; }
        [Parameter]
        public string? ResponseProperty { get; set; }
        [Parameter]
        public string? ToLocalStorage { get; set; }
        [Parameter]
        public bool ReloadPageOnComplete { get; set; } = false;
        [Parameter]
        public string? Style { get; set; }
        [Parameter]
        public string? Class { get; set; }

        RadzenTextBox textBox = default!;
        string? currentValue = "";
        public bool isValueInitial = true;

        public void SetInitialValue(string initialValue)
        {
            InitialValue = initialValue;
            currentValue = initialValue;
            StateHasChanged();
        }

        async Task OnChange(string? value)
        {
            if (!string.IsNullOrEmpty(value) && value != InitialValue)
            {
                isValueInitial = false;
                currentValue = value;
                StateHasChanged();
                await StateChange.InvokeAsync();
            }
            else
            {
                isValueInitial = true;
                currentValue = value;
                StateHasChanged();
                await StateChange.InvokeAsync();
            }
        }

        public async Task OnClick()
        {
            if (!isValueInitial)
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendInformationalNotification("You're not logged in or your session has expired");
                    NavigationManager.NavigateTo("/login");
                }

                var token = await tokenService.GetToken();
                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var newName = textBox.Value;
                Dictionary<string, string> content = new Dictionary<string, string>
             {
                 { "newName", newName }
             };

                var request = await client.PostAsJsonAsync(RequestUrl, content);
                var body = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(ResponseProperty))
                        {
                            var responseValue = JsonSerializer.Deserialize<Dictionary<string, string>>(body)![ResponseProperty];
                            if (!string.IsNullOrEmpty(ToLocalStorage))
                            {
                                await _localStorage.SetAsync(ToLocalStorage, responseValue);
                            }
                        }

                        if (ReloadPageOnComplete) { NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true); }
                    }
                    catch (Exception ex)
                    {
                        notifier.SendErrorNotification("Something went wrong!");
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    notifier.SendErrorNotification(body);
                }
            }
        }
    }
}
