using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http;
using ChampionshipMaster.SHARED.DTO;
using Radzen.Blazor;
using System.IdentityModel.Tokens.Jwt;
using ChampionshipMaster.Web.Components.Pages.User.Account;
namespace ChampionshipMaster.Web.Components.Pages.User.Team;

public partial class ManageTeams : ComponentBase
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

    public async Task OpenCreateTeam()
    {
        await DialogService.OpenAsync<CreateTeam>($"Create Team",
               new Dictionary<string, object>() { },
               new DialogOptions() { Width = "45%", Height = "53%", Resizable = true, Draggable = true });
    }
    IList<TeamDto>? selectedTeam;
    private bool disabledEdit = true;

    void Update(DataGridRowMouseEventArgs<TeamDto> args)
    {
        if (args.Data.CreatedBy == playerId)
            disabledEdit = false;
        else
            disabledEdit = true;

        StateHasChanged();
    }

    bool forceAdmin = true;
    bool isAdmin = false;
    bool myTeamOnlyMode = false;
    string playerId = "";
    private string username = "";
    private string role = "";
    RadzenDataGrid<TeamDto>? teamsList;

    private List<TeamDto>? teams;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());
            username = token.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value ?? "";
            playerId = token.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
            role = token.Claims.FirstOrDefault(x => x.Type == "role")?.Value ?? "";
            isAdmin = role == "admin" || forceAdmin ? true : false;
            await GetData();
            selectedTeam = null;
            StateHasChanged();
        }
    }


    private async Task GetData()
    {
        using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
        var test = await client.GetFromJsonAsync<List<TeamDto>>("/api/Teams/active");
        teams = test;

    }

    void EditTeam(string id)
    {
        NavigationManager.NavigateTo($"/editteam/{id}");
    }
    void OnCellContextMenu(DataGridCellMouseEventArgs<TeamDto> args)
    {
        if (args == null)
            return;
        if (args.Data == null)
            return;

        selectedTeam = new List<TeamDto>() { args.Data };
        if (args.Data.CreatedBy == playerId)
        {
            ContextMenuService.Open(args,
       new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "Edit", Value = 1, Icon = "edit" },
                                       },
       (e) =>
       {
           if (e.Text == "Edit")
           {
               EditTeam(args.Data.Id.ToString());
               ContextMenuService.Close();
           }
       }
    );
        }
        else
            return;

    }
}
