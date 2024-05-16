using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.Admin.Championship.ChampionshipTypes
{
    public partial class ChampionshipTypeMain : ComponentBase 
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

        RadzenDataGrid<ChampionshipType> championshipTypeGrid = default!;

        List<ChampionshipType> championshipTypeList = default!;

        List<ChampionshipType> championshiptypesToInsert = new List<ChampionshipType>();
        List<ChampionshipType> championshiptypesToUpdate = new List<ChampionshipType>();

        ChampionshipType Original { get; set; } = new ChampionshipType();

        void Reset()
        {
            championshiptypesToInsert.Clear();
            championshiptypesToUpdate.Clear();
        }

        void Reset(ChampionshipType championshipType)
        {
            championshiptypesToInsert.Remove(championshipType);
            championshiptypesToUpdate.Remove(championshipType);
        }

        async Task EditRow(ChampionshipType championshipType)
        {

            if (championshiptypesToInsert.Count() > 0)
            {
                Reset();
            }

            Original = new ChampionshipType { Id = championshipType.Id, Name = championshipType.Name };

            championshiptypesToUpdate.Add(championshipType);

            await championshipTypeGrid.EditRow(championshipType);
        }

        async Task OnUpdateRow(ChampionshipType championshipType)
        {
            Reset(championshipType);

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var result = await client.PutAsJsonAsync($"/api/ChampionshipTypes/{championshipType.Id}", championshipType);

        }

        async Task SaveRow(ChampionshipType championshipType)
        {
            if (championshipTypeList.Any(x => x.Name!.ToLower() == championshipType.Name!.ToLower()
                && x.Id != championshipType.Id))
            {
                notifier.SendErrorNotification("There is already a championship type with that name", 4);
                return;
            }

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var response = await client.PostAsJsonAsync("/api/ChampionshipTypes", championshipType);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var championshipTypeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                int id = int.Parse(championshipTypeResponse!["id"].ToString()!);

                championshipType.Id = id;

                championshipTypeList.Add(championshipType);
            }

            await championshipTypeGrid.UpdateRow(championshipType);
        }

        async Task CancelEdit(ChampionshipType championshipType)
        {
            Reset(championshipType);

            championshipType.Name = Original.Name;

            championshipTypeGrid.CancelEditRow(championshipType);

            await championshipTypeGrid.Reload();
        }

        async Task DeleteRow(ChampionshipType championshipType)
        {
            Reset(championshipType);

            if (championshipTypeList!.Contains(championshipType))
            {
                championshipTypeList.Remove(championshipType);

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                await client.DeleteAsync($"/api/ChampionshipTypes/{championshipType.Id}");

                await championshipTypeGrid.Reload();

            }
            else
            {
                championshipTypeGrid.CancelEditRow(championshipType);
                await championshipTypeGrid.Reload();
            }
        }

        async Task InsertRow()
        {
            Reset();
            // ??????????????????????????????????????????????????? check input data ?????????????????
            var championshipType = new ChampionshipType();

            championshiptypesToInsert.Add(championshipType);

            await championshipTypeGrid.InsertRow(championshipType);


        }

        void OnCreateRow(ChampionshipType championshipType)
        {
            championshiptypesToInsert.Remove(championshipType);
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
                    championshipTypeList = await client.GetFromJsonAsync<List<ChampionshipType>>("/api/ChampionshipTypes") ?? new List<ChampionshipType>();
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
