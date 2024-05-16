using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.Admin.Game.GameTypes
{
    public partial class GameTypeMain : ComponentBase
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

        RadzenDataGrid<GameType> GameTypeGrid = default!;

        List<GameType> GameTypeList = default!;

        List<GameType> GameTypeesToInsert = new List<GameType>();
        List<GameType> GameTypeesToUpdate = new List<GameType>();

        GameType Original { get; set; } = new GameType();

        void Reset()
        {
            GameTypeesToInsert.Clear();
            GameTypeesToUpdate.Clear();
        }

        void Reset(GameType GameType)
        {
            GameTypeesToInsert.Remove(GameType);
            GameTypeesToUpdate.Remove(GameType);
        }

        async Task EditRow(GameType GameType)
        {

            if (GameTypeesToInsert.Count() > 0)
            {
                Reset();
            }

            Original = new GameType { Id = GameType.Id, Name = GameType.Name };

            GameTypeesToUpdate.Add(GameType);

            await GameTypeGrid.EditRow(GameType);
        }

        async Task OnUpdateRow(GameType GameType)
        {
            Reset(GameType);

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var result = await client.PutAsJsonAsync($"/api/GameTypes/{GameType.Id}", GameType);

        }

        async Task SaveRow(GameType GameType)
        {
            if (GameTypeList.Any(x => x.Name!.ToLower() == GameType.Name!.ToLower()
                && x.Id != GameType.Id))
            {
                notifier.SendErrorNotification("There is already a game status with that name");
                return;
            }

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var response = await client.PostAsJsonAsync("/api/GameTypes", GameType);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var GameTypeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                int id = int.Parse(GameTypeResponse!["id"].ToString()!);

                GameType.Id = id;

                GameTypeList.Add(GameType);
            }

            await GameTypeGrid.UpdateRow(GameType);
        }

        async Task CancelEdit(GameType GameType)
        {
            Reset(GameType);

            GameType.Name = Original.Name;

            GameTypeGrid.CancelEditRow(GameType);

            await GameTypeGrid.Reload();
        }

        async Task DeleteRow(GameType GameType)
        {
            Reset(GameType);

            if (GameTypeList!.Contains(GameType))
            {
                GameTypeList.Remove(GameType);

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                await client.DeleteAsync($"/api/GameTypes/{GameType.Id}");

                await GameTypeGrid.Reload();

            }
            else
            {
                GameTypeGrid.CancelEditRow(GameType);
                await GameTypeGrid.Reload();
            }
        }

        async Task InsertRow()
        {
            Reset();
            // ??????????????????????????????????????????????????? check input data ?????????????????
            var GameType = new GameType();

            GameTypeesToInsert.Add(GameType);

            await GameTypeGrid.InsertRow(GameType);


        }

        void OnCreateRow(GameType GameType)
        {
            GameTypeesToInsert.Remove(GameType);
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
                    GameTypeList = await client.GetFromJsonAsync<List<GameType>>("/api/GameTypes") ?? new List<GameType>();
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
