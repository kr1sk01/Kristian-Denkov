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

    IList<TeamDto>? selectedTeam;
    private bool disabledEdit = true;
    bool isAdmin = false;
    string playerId = "";
    private string role = "";
    private List<TeamDto>? teams;

    public async Task OpenCreateTeam()
    {
        await DialogService.OpenAsync<CreateTeam>($"Create Team",
               new Dictionary<string, object>() { },
               new DialogOptions() { Width = "45%", Height = "53%", Draggable = true, CloseDialogOnEsc = true });
    }

    public async Task OpenEditTeam(string id)
    {
        await DialogService.OpenAsync<EditTeam>($"Create Team",
               new Dictionary<string, object>() { { "id", id } },
               new DialogOptions() { Width = "75%", Height = "75%", CloseDialogOnEsc = true });
    }
    
    void Update(DataGridRowMouseEventArgs<TeamDto> args)
    {
        if (args.Data.CreatedBy == playerId || isAdmin)
            disabledEdit = false;
        else
            disabledEdit = true;

        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());
            playerId = token.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
            role = token.Claims.FirstOrDefault(x => x.Type == "role")?.Value ?? "";
            isAdmin = role == "admin";
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

    //void EditTeam(string id)
    //{
    //    NavigationManager.NavigateTo($"/editteam/{id}");
    //}
    void OnCellContextMenu(DataGridCellMouseEventArgs<TeamDto> args)
    {
        if (args == null)
            return;
        if (args.Data == null)
            return;

        selectedTeam = new List<TeamDto>() { args.Data };
        if (args.Data.CreatedBy == playerId || isAdmin)
        {
            ContextMenuService.Open(args,
       new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "Edit", Value = 1, Icon = "edit" },
                                       },
       async (e) =>
       {
           if (e.Text == "Edit")
           {
               await OpenEditTeam(args.Data.Id.ToString());
               ContextMenuService.Close();
           }
       }
    );
        }
        else
            return;

    }
}
