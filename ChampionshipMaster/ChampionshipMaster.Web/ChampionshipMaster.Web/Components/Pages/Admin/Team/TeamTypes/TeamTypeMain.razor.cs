using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.Admin.Team.TeamTypes
{
    public partial class TeamTypeMain : ComponentBase
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

        RadzenDataGrid<TeamType> TeamTypeGrid = default!;

        List<TeamType> TeamTypeList = default!;

        List<TeamType> TeamTypeesToInsert = new List<TeamType>();
        List<TeamType> TeamTypeesToUpdate = new List<TeamType>();

        TeamType Original { get; set; } = new TeamType();

        void Reset()
        {
            TeamTypeesToInsert.Clear();
            TeamTypeesToUpdate.Clear();
        }

        void Reset(TeamType TeamType)
        {
            TeamTypeesToInsert.Remove(TeamType);
            TeamTypeesToUpdate.Remove(TeamType);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();


        }

        async Task EditRow(TeamType TeamType)
        {

            if (TeamTypeesToInsert.Count() > 0)
            {
                Reset();
            }

            Original = new TeamType { Id = TeamType.Id, Name = TeamType.Name };

            TeamTypeesToUpdate.Add(TeamType);

            await TeamTypeGrid.EditRow(TeamType);
        }

        async Task OnUpdateRow(TeamType TeamType)
        {
            Reset(TeamType);

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var result = await client.PutAsJsonAsync($"/api/TeamTypes/{TeamType.Id}", TeamType);

        }

        async Task SaveRow(TeamType TeamType)
        {
            if (TeamTypeList.Any(x => x.Name!.ToLower() == TeamType.Name!.ToLower()
                && x.Id != TeamType.Id))
            {
                notifier.SendErrorNotification("There is already a game status with that name", 4);

                return;
            }

            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            var response = await client.PostAsJsonAsync("/api/TeamTypes", TeamType);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var TeamTypeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                int id = int.Parse(TeamTypeResponse!["id"].ToString()!);

                TeamType.Id = id;

                TeamTypeList.Add(TeamType);
            }

            await TeamTypeGrid.UpdateRow(TeamType);
        }

        async Task CancelEdit(TeamType TeamType)
        {
            Reset(TeamType);

            TeamType.Name = Original.Name;

            TeamTypeGrid.CancelEditRow(TeamType);

            await TeamTypeGrid.Reload();
        }

        async Task DeleteRow(TeamType TeamType)
        {
            Reset(TeamType);

            if (TeamTypeList!.Contains(TeamType))
            {
                TeamTypeList.Remove(TeamType);

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                await client.DeleteAsync($"/api/TeamTypes/{TeamType.Id}");

                await TeamTypeGrid.Reload();

            }
            else
            {
                TeamTypeGrid.CancelEditRow(TeamType);
                await TeamTypeGrid.Reload();
            }
        }

        async Task InsertRow()
        {
            Reset();
            // ??????????????????????????????????????????????????? check input data ?????????????????
            var TeamType = new TeamType();

            TeamTypeesToInsert.Add(TeamType);

            await TeamTypeGrid.InsertRow(TeamType);


        }

        void OnCreateRow(TeamType TeamType)
        {
            TeamTypeesToInsert.Remove(TeamType);
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
                    TeamTypeList = await client.GetFromJsonAsync<List<TeamType>>("/api/TeamTypes") ?? new List<TeamType>();
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
