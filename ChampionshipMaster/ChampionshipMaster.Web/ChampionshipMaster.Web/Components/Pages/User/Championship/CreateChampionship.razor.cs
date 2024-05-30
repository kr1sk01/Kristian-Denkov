using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.Web.Components.Pages.User.Team;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

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
    [Inject] INotifier notifier { get; set; } = default!;

   

    public bool isLogged = false;
    public bool isAdmin = false;
    //Display data lists
    public List<ChampionshipStatusDto>? championshipStatuses = new List<ChampionshipStatusDto>();
    public List<ChampionshipTypeDto>? championshipTypes = new List<ChampionshipTypeDto>();
    public List<GameTypeDto>? gameTypes = new List<GameTypeDto>();
    //Champpaddd
    public ChampionshipDto? championshipToAdd = new ChampionshipDto { ChampionshipStatusName = "Coming" };
    //Choosen data
    public GameTypeDto? selectedGameType;
    public ChampionshipTypeDto? selectedChampionshipType;
    public ChampionshipStatusDto? selectedChampionshipStatusDto;
    public int GameTypeId;


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendWarningNotification("Your session has run out or you're not logged in");
                    NavigationManager.NavigateTo("/login");
                    return;
                }
                
                if(!await tokenService.ValidateToken(true))
                {
                    notifier.SendErrorNotification("You don't have permission!");
                    NavigationManager.NavigateTo("/");
                    return;
                }

                isAdmin = true;
                isLogged = true;

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetToken());

                championshipTypes = await client.GetFromJsonAsync<List<ChampionshipTypeDto>>("api/ChampionshipTypes");
                if (championshipTypes == null || championshipTypes.Count == 0)
                {
                    notifier.SendErrorNotification("Couldn't retrieve championship types!");
                    NavigationManager.NavigateTo("/");
                    return;
                }

                championshipStatuses = await client.GetFromJsonAsync<List<ChampionshipStatusDto>>("api/ChampionshipStatus");
                if (championshipStatuses == null || championshipStatuses.Count == 0)
                {
                    notifier.SendErrorNotification("Couldn't retrieve championship statuses!");
                    NavigationManager.NavigateTo("/");
                    return;
                }

                gameTypes = await client.GetFromJsonAsync<List<GameTypeDto>>("api/GameTypes");
                if (gameTypes == null || gameTypes.Count == 0)
                {
                    notifier.SendErrorNotification("Couldn't retrieve game types!");
                    NavigationManager.NavigateTo("/");
                    return;
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                notifier.SendErrorNotification($"Error: {ex.Message}");
                NavigationManager.NavigateTo("/error");
            }
        }

    }
    public async Task OnSubmit()
    {
        if (!await tokenService.ValidateToken())
        {
            notifier.SendInformationalNotification("You're not logged in or your session has expired");
            NavigationManager.NavigateTo("/login");
        }

        var token = await tokenService.GetToken();
        using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        championshipToAdd!.GameType = gameTypes!.FirstOrDefault(x => x.Id == GameTypeId);
        var request = await client.PostAsJsonAsync("api/Championship", championshipToAdd);

        if (request.IsSuccessStatusCode)
        {
            notifier.SendSuccessNotification("Championship created successfully!");
            NavigationManager.NavigateTo("/championshipsMain");
        }
        else
        {
            var body = await request.Content.ReadAsStringAsync();
            notifier.SendErrorNotification(body);
        }
    }

    public void OnInvalidSubmit()
    {
        notifier.SendErrorNotification("Please correct the errors and try again.");
    }

    public void DateRender(DateRenderEventArgs args)
    {
        if(championshipToAdd != null && championshipToAdd.LotDate != null)
            args.Disabled = args.Disabled || (args.Date < championshipToAdd.LotDate);
    }
    public void LotDateRender(DateRenderEventArgs args)
    {        
            args.Disabled = args.Disabled || (args.Date < DateTime.UtcNow);
    }
}
