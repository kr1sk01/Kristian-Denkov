using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.Admin.Game.GameStatuses
{
    public partial class GameStatusMain : ComponentBase
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

        private bool dataLoaded = false;

        RadzenDataGrid<GameStatus> gameStatusGrid = default!;

        List<GameStatus> gameStatusList = default!;

        List<GameStatus> gameStatusesToInsert = new List<GameStatus>();
        List<GameStatus> gameStatusesToUpdate = new List<GameStatus>();

        GameStatus Original { get; set; } = new GameStatus();

        void Reset()
        {
            gameStatusesToInsert.Clear();
            gameStatusesToUpdate.Clear();
        }

        void Reset(GameStatus gameStatus)
        {
            gameStatusesToInsert.Remove(gameStatus);
            gameStatusesToUpdate.Remove(gameStatus);
        }

        async Task EditRow(GameStatus gameStatus)
        {

            if (gameStatusesToInsert.Count() > 0)
            {
                Reset();
            }

            Original = new GameStatus { Id = gameStatus.Id, Name = gameStatus.Name };

            gameStatusesToUpdate.Add(gameStatus);

            await gameStatusGrid.EditRow(gameStatus);
        }

        async Task OnUpdateRow(GameStatus gameStatus)
        {
            Reset(gameStatus);

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var result = await client.PutAsJsonAsync($"/api/GameStatus/{gameStatus.Id}", gameStatus);

        }

        async Task SaveRow(GameStatus gameStatus)
        {
            if (gameStatusList.Any(x => x.Name!.ToLower() == gameStatus.Name!.ToLower()
                && x.Id != gameStatus.Id))
            {
                notifier.SendErrorNotification("There is already a game status with that name");
                return;
            }

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var response = await client.PostAsJsonAsync("/api/GameStatus", gameStatus);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gameStatusResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                int id = int.Parse(gameStatusResponse!["id"].ToString()!);

                gameStatus.Id = id;

                gameStatusList.Add(gameStatus);
            }

            await gameStatusGrid.UpdateRow(gameStatus);
        }

        async Task CancelEdit(GameStatus gameStatus)
        {
            Reset(gameStatus);

            gameStatus.Name = Original.Name;

            gameStatusGrid.CancelEditRow(gameStatus);

            await gameStatusGrid.Reload();
        }

        async Task DeleteRow(GameStatus gameStatus)
        {
            Reset(gameStatus);

            if (gameStatusList!.Contains(gameStatus))
            {
                gameStatusList.Remove(gameStatus);

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                await client.DeleteAsync($"/api/GameStatus/{gameStatus.Id}");

                await gameStatusGrid.Reload();

            }
            else
            {
                gameStatusGrid.CancelEditRow(gameStatus);
                await gameStatusGrid.Reload();
            }
        }

        async Task InsertRow()
        {
            Reset();
            // ??????????????????????????????????????????????????? check input data ?????????????????
            var gameStatus = new GameStatus();

            gameStatusesToInsert.Add(gameStatus);

            await gameStatusGrid.InsertRow(gameStatus);


        }

        void OnCreateRow(GameStatus gameStatus)
        {
            gameStatusesToInsert.Remove(gameStatus);
        }

        bool isAdmin = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (await tokenService.ValidateToken(true))
                {
                    isAdmin = true;
                    var token = await tokenService.GetToken();
                    using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    gameStatusList = await client.GetFromJsonAsync<List<GameStatus>>("/api/GameStatus") ?? new List<GameStatus>();
                    StateHasChanged();
                }
                else
                {
                    notifier.SendErrorNotification("Access denied!");
                    notifier.SendWarningNotification("If you believe there is an error, please contact administrator!", 10);
                    NavigationManager.NavigateTo("/");

                }
            }
        }
    }
}
