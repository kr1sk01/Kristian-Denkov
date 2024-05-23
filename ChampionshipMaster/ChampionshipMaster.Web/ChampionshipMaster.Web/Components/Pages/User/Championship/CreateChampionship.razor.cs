using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ChampionshipMaster.Web.Components.Pages.User.Championship;

public partial class CreateChampionship : ComponentBase
{
    [Inject] NavigationManager nManager { get; set; } = default!;
    [Inject] Radzen.DialogService dialogService { get; set; } = default!;
    [Inject] IConfiguration configuration { get; set; } = default!;
    [Inject] ITokenService tokenService { get; set; } = default!;
    [Inject] IHttpClientFactory httpClient { get; set; } = default!;
    [Inject] IWebHostEnvironment Environment { get; set; } = default!;

    [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] ContextMenuService ContextMenuService { get; set; } = default!;
    [Inject] DialogService DialogService { get; set; } = default!;



}
