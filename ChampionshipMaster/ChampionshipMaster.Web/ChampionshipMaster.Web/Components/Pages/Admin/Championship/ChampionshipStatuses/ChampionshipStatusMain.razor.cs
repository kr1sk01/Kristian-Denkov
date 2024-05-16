using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Text.Json;


namespace ChampionshipMaster.Web.Components.Pages.Admin.Championship.ChampionshipStatuses
{
    public partial class ChampionshipStatusMain : ComponentBase
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

        RadzenDataGrid<ChampionshipStatus> championshipStatusGrid = default!;

        List<ChampionshipStatus> championshipStatusList = default!;

        List<ChampionshipStatus> championshipStatusesToInsert = new List<ChampionshipStatus>();
        List<ChampionshipStatus> championshipStatusesToUpdate = new List<ChampionshipStatus>();

        ChampionshipStatus Original { get; set; } = new ChampionshipStatus();

        void Reset()
        {
            championshipStatusesToInsert.Clear();
            championshipStatusesToUpdate.Clear();
        }

        void Reset(ChampionshipStatus championshipStatus)
        {
            championshipStatusesToInsert.Remove(championshipStatus);
            championshipStatusesToUpdate.Remove(championshipStatus);
        }

        async Task EditRow(ChampionshipStatus championshipStatus)
        {

            if (championshipStatusesToInsert.Count() > 0)
            {
                Reset();
            }

            Original = new ChampionshipStatus { Id = championshipStatus.Id, Name = championshipStatus.Name };

            championshipStatusesToUpdate.Add(championshipStatus);

            await championshipStatusGrid.EditRow(championshipStatus);
        }

        async Task OnUpdateRow(ChampionshipStatus championshipStatus)
        {
            Reset(championshipStatus);

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var result = await client.PutAsJsonAsync($"/api/ChampionshipStatus/{championshipStatus.Id}", championshipStatus);

        }

        async Task SaveRow(ChampionshipStatus championshipStatus)
        {
            if (championshipStatusList.Any(x => x.Name!.ToLower() == championshipStatus.Name!.ToLower()
                && x.Id != championshipStatus.Id))
            {
                notifier.SendErrorNotification("There is already a championship status with that name", 4);

                return;
            }

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var response = await client.PostAsJsonAsync("/api/ChampionshipStatus", championshipStatus);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var championshipStatusResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                int id = int.Parse(championshipStatusResponse!["id"].ToString()!);

                championshipStatus.Id = id;

                championshipStatusList.Add(championshipStatus);
            }

            await championshipStatusGrid.UpdateRow(championshipStatus);
        }

        async Task CancelEdit(ChampionshipStatus championshipStatus)
        {
            Reset(championshipStatus);

            championshipStatus.Name = Original.Name;

            championshipStatusGrid.CancelEditRow(championshipStatus);

            await championshipStatusGrid.Reload();
        }

        async Task DeleteRow(ChampionshipStatus championshipStatus)
        {
            Reset(championshipStatus);

            if (championshipStatusList!.Contains(championshipStatus))
            {
                championshipStatusList.Remove(championshipStatus);

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                await client.DeleteAsync($"/api/ChampionshipStatus/{championshipStatus.Id}");

                await championshipStatusGrid.Reload();

            }
            else
            {
                championshipStatusGrid.CancelEditRow(championshipStatus);
                await championshipStatusGrid.Reload();
            }
        }

        async Task InsertRow()
        {
            Reset();
            // ??????????????????????????????????????????????????? check input data ?????????????????
            var championshipStatus = new ChampionshipStatus();

            championshipStatusesToInsert.Add(championshipStatus);

            await championshipStatusGrid.InsertRow(championshipStatus);


        }

        void OnCreateRow(ChampionshipStatus championshipStatus)
        {
            championshipStatusesToInsert.Remove(championshipStatus);
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
                    championshipStatusList = await client.GetFromJsonAsync<List<ChampionshipStatus>>("/api/ChampionshipStatus") ?? new List<ChampionshipStatus>();
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
