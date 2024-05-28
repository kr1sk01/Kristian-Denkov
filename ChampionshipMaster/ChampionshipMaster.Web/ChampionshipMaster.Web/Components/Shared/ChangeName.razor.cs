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
        public string? InitialValue { get; set; }

        //[Parameter]
        //public required string RequestUrl { get; set; }
        //[Parameter]
        //public string? ResponseProperty { get; set; }
        //[Parameter]
        //public string? ToLocalStorage { get; set; }
        //[Parameter]
        //public bool ReloadPageOnComplete { get; set; } = false;

        [Parameter]
        public string? Style { get; set; }
        [Parameter]
        public string? Class { get; set; }

        public string? CurrentValue = "";
        public bool IsValueInitial = true;

        public void SetInitialValue(string initialValue)
        {
            InitialValue = initialValue;
            CurrentValue = initialValue;
            StateHasChanged();
        }

        async Task OnChange(string? value)
        {
            if (!string.IsNullOrEmpty(value) && value != InitialValue)
            {
                IsValueInitial = false;
                CurrentValue = value;
                StateHasChanged();
                await StateChange.InvokeAsync();
            }
            else
            {
                IsValueInitial = true;
                CurrentValue = value;
                StateHasChanged();
                await StateChange.InvokeAsync();
            }
        }
    }
}
