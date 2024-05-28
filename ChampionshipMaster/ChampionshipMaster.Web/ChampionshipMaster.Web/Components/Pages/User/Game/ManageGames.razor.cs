using ChampionshipMaster.Web.Components.Pages.User.Team;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

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
        [Inject] INotifier notifier { get; set; } = default!;
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
            if ((args.Data.CreatedBy == playerId || isAdmin) && args.Data.GameStatusName != "Finished")
                disabledEdit = false;
            else
                disabledEdit = true;

            StateHasChanged();
        }
        public async Task OpenCreateGame()
        {
            await DialogService.OpenAsync<CreateGame>($"",
                   new Dictionary<string, object>() { },
                   new DialogOptions() { Width = "45%", Height = "53%", Draggable = true, CloseDialogOnEsc = true });
        }

        public async Task OpenEditGame(string id)
        {
            await DialogService.OpenAsync<EditGame>($"",
                  new Dictionary<string, object>() { { "id", id } },
                  new DialogOptions() { Width = "75vw", Height = "95%", CloseDialogOnEsc = true });
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
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await tokenService.GetToken()}");
            var test = await client.GetFromJsonAsync<List<GameDto>>("/api/Game");
            if (test == null || test.Count == 0)
            {
                notifier.SendErrorNotification("Couldn't retrieve games");
                NavigationManager.NavigateTo("/");
            }

            games = test!;

            int i = 0;
            foreach (var game in games)
            {
                if (game.Date != null)
                {
                    games[i].Date = game.Date.Value.ToLocalTime();
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
                foreach (var game in games)
                {
                    if (game.CreatedBy != null)
                    {
                        games[i].CreatedByUsername = playerUsernames[i];
                    }

                    i++;
                }
            }

            StateHasChanged();
        }


        void OnCellContextMenu(DataGridCellMouseEventArgs<GameDto> args)
        {
            if (args == null)
                return;
            if (args.Data == null)
                return;

            selectedTeam = new List<GameDto>() { args.Data };
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
                   await OpenEditGame(args.Data.Id.ToString());
                   ContextMenuService.Close();
               }
           }
        );
            }
            else
                return;

        }


    }

}

