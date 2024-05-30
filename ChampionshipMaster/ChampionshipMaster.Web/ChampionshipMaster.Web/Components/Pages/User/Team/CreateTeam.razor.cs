namespace ChampionshipMaster.Web.Components.Pages.User.Team;
using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.SHARED.DTO;
using ChampionshipMaster.Web.Components.Pages.User.Account;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

public partial class CreateTeam : ComponentBase
{
    [Inject] Radzen.DialogService dialogService { get; set; } = default!;
    [Inject] IConfiguration configuration { get; set; } = default!;
    [Inject] ITokenService tokenService { get; set; } = default!;
    [Inject] IHttpClientFactory httpClient { get; set; } = default!;
    [Inject] IWebHostEnvironment Environment { get; set; } = default!;
    [Inject] INotifier notifier { get; set; } = default!;
    [Inject] ProtectedLocalStorage _localStorage { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;


    [Parameter] public bool? isRedirectedFromChampionship { get; set; }
    [Parameter] public int? championshipId { get; set; }

    Variant variant = Variant.Outlined;

    string playerId = "";

    TeamDto model = new TeamDto();

    RadzenDropDown<string> teamTypeDropDown;
    ChampionshipTeamsDto? ChampionshipTeamsToAdd;
    //private GameType gameType = new GameType();
    private List<TeamType> teamTypes = new List<TeamType>(); // Assuming you have a list of TeamTypes to choose from

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var test = await client.GetFromJsonAsync<List<TeamType>>("api/TeamTypes");
            if(test != null)
            {
                teamTypes = test;
                if (isRedirectedFromChampionship == true)
                {
                    var championship = await client.GetFromJsonAsync<ChampionshipDto>($"api/Championship/{championshipId}");
                    await teamTypeDropDown.SelectItem(championship!.GameType!.TeamTypeName, false);
                    model.TeamTypeName = championship!.GameType!.TeamTypeName;
                    teamTypeDropDown.Disabled = true;
                    ;
                }
                StateHasChanged();
            }           
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
            playerId = token.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
        }
        model.CreatedBy = playerId;

        using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");

        var response = await client.PostAsJsonAsync("/api/Teams", model);
        var body = await response.Content.ReadAsStringAsync();
        Dictionary<string, object> object2 = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
        if (isRedirectedFromChampionship==true) 
        {
            ChampionshipTeamsToAdd = new ChampionshipTeamsDto
            {
                TeamId = int.Parse(object2["id"].ToString()),
                ChampionshipId = championshipId
            };
            await client.PostAsJsonAsync("api/Championship/join", ChampionshipTeamsToAdd);

        }
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var TeamTypeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            var id = TeamTypeResponse!["id"].ToString();

            notifier.SendSuccessNotification("Team created successfully!");

            NavigationManager.NavigateTo($"/editteam/{id}");
        }
        else
        {
            notifier.SendErrorNotification(content);

            NavigationManager.NavigateTo($"/manageteams");
        }
    }
}
