using ChampionshipMaster.Web.Components.Pages.User.Team;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace ChampionshipMaster.Web.Components.Pages.User.Game
{
    public partial class ManageGames : ComponentBase
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

        IList<GameDto>? selectedTeam;
        private bool disabledEdit = true;
        bool isAdmin = false;
        string playerId = "";
        private string role = "";
        private List<GameDto>? games;

        void Update(DataGridRowMouseEventArgs<GameDto> args)
        {
            if (args.Data.CreatedBy == playerId || isAdmin)
                disabledEdit = false;
            else
                disabledEdit = true;

            StateHasChanged();
        }
        public async Task OpenCreateTeam()
        {
            //await DialogService.OpenAsync<CreateTeam>($"Create Game",
            //       new Dictionary<string, object>() { },
            //       new DialogOptions() { Width = "45%", Height = "53%", Draggable = true, CloseDialogOnEsc = true });
        }

        public async Task OpenEditGame(string id)
        {
            //await DialogService.OpenAsync<EditTeam>($"",
            //       new Dictionary<string, object>() { { "id", id } },
            //       new DialogOptions() { Width = "75%", Height = "75%", CloseDialogOnEsc = true });
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
            var test = await client.GetFromJsonAsync<List<GameDto>>("/api/Game");
            games = test;

        }

        void OnCellContextMenu(DataGridCellMouseEventArgs<GameDto> args)
        {
            

        }
    }

}

