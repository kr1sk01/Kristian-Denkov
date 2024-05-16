namespace ChampionshipMaster.Web.Components.Pages.User.Team;
using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.SHARED.DTO;
using ChampionshipMaster.Web.Components.Pages.User.Account;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

public partial class CreateTeam : ComponentBase
{

    [Inject] NavigationManager nManager {  get; set; } = default!;
    [Inject] Radzen.DialogService dialogService { get; set; } = default!;

    [Inject] IConfiguration configuration { get; set; } = default!;
    [Inject] ITokenService tokenService { get; set; } = default!;
    [Inject] IHttpClientFactory httpClient { get; set; } = default!;
    [Inject] IWebHostEnvironment Environment { get; set; } = default!;
    [Inject] INotifier notifier { get; set; } = default!;
    [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    Variant variant = Variant.Outlined;

    string username = "";
    string playerId = "";
    string teamid = "";

    TeamDto model = new TeamDto();

    //private GameType gameType = new GameType();
    private List<TeamType> teamTypes = new List<TeamType>(); // Assuming you have a list of TeamTypes to choose from

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var test = await client.GetFromJsonAsync<List<TeamType>>("api/TeamTypes");
            teamTypes = test;
            StateHasChanged();
        }
    }

    private async Task OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        //TeamDto team = new TeamDto { };
    }
    private async Task OnSubmit(TeamDto model)
    {
        if (await tokenService.ValidateToken())
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());
            username = token.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value ?? "";
            playerId = token.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
        }
        model.CreatedBy = playerId;

        using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");

        var response = await client.PostAsJsonAsync("/api/Teams", model);

        var content = await response.Content.ReadAsStringAsync();
        var TeamTypeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
        var id = TeamTypeResponse!["id"].ToString();

        notifier.SendSuccessNotification("Team created successfully!");

        NavigationManager.NavigateTo($"/editteam/{id}");
        //TeamDto team = new TeamDto { };
    }
}
