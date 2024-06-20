using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http;
using ChampionshipMaster.SHARED.DTO;
using Radzen.Blazor;
using System.IdentityModel.Tokens.Jwt;
using ChampionshipMaster.Web.Components.Pages.User.Account;
using System.Text.Json;
namespace ChampionshipMaster.Web.Components.Pages.User.Team;

public partial class ManageTeams : ComponentBase
{
    [Inject] NavigationManager nManager { get; set; } = default!;
    [Inject] Radzen.DialogService dialogService { get; set; } = default!;
    [Inject] IConfiguration configuration { get; set; } = default!;
    [Inject] ITokenService tokenService { get; set; } = default!;
    [Inject] IHttpClientFactory httpClient { get; set; } = default!;
    [Inject] IWebHostEnvironment Environment { get; set; } = default!;
    [Inject] INotifier notifier { get; set; } = default!;
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
        await DialogService.OpenAsync<CreateTeam>($"",
               new Dictionary<string, object>() { },
               new DialogOptions() { Width = "45%", Height = "53%", Draggable = true, CloseDialogOnEsc = true });
    }

    public async Task OpenEditTeam(string id)
    {
        await DialogService.OpenAsync<EditTeam>($"",
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
            if (!await tokenService.ValidateToken())
            {
                notifier.SendWarningNotification("Your session has run out or you're not logged in");
                NavigationManager.NavigateTo("/login");
                return;
            }

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
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");
        var test = await client.GetFromJsonAsync<List<TeamDto>>("/api/Teams/active");
        if (test == null || test.Count == 0)
        {
            notifier.SendErrorNotification("Couldn't retrieve games");
            NavigationManager.NavigateTo("/");
        }
        teams = test!;

        int i = 0;
        foreach (var team in teams)
        {
            if (team.CreatedOn != null)
            {
                teams[i].CreatedOn = team.CreatedOn.Value.ToLocalTime();
            }

            i++;
        }

        var jsonString = JsonSerializer.Serialize(test!.Select(x => x.CreatedBy).ToList());
        var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
        var request = await client.PostAsync("api/Player/getPlayersById", content);
        var body = await request.Content.ReadAsStringAsync();

        if (request.IsSuccessStatusCode)
        {
            var playerUsernames = JsonSerializer.Deserialize<List<string>>(body);

            i = 0;
            foreach (var team in teams)
            {
                if (team.CreatedBy != null)
                {
                    teams[i].CreatedByUsername = playerUsernames[i];
                }

                i++;
            }
        }

        StateHasChanged();

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
