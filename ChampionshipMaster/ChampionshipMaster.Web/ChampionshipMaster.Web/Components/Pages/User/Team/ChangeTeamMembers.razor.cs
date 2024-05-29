using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.User.Team
{
    public partial class ChangeTeamMembers : ComponentBase
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

        [Parameter]
        public EventCallback StateChange { get; set; }
        [Parameter]
        public string id { get; set; }

        List<RadzenDropDown<string>> dropDowns = new List<RadzenDropDown<string>>(4)
    {
        new RadzenDropDown<string>(),
        new RadzenDropDown<string>(),
        new RadzenDropDown<string>(),
        new RadzenDropDown<string>(),
    };

        List<PlayerDto> allPlayers = new List<PlayerDto>();
        List<PlayerDto> selectablePlayers = new List<PlayerDto>();
        List<PlayerDto> initialPlayers = new List<PlayerDto>();
        public List<PlayerDto?> selectedPlayers = new List<PlayerDto?>()
    {
        null, null, null, null
    };

        string userToAdd_Id = "";
        int teamMaxPlayers = 0;

        public bool IsValueInitial = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);

                var team = await client.GetFromJsonAsync<TeamDto>($"api/Teams/{id}");
                if (team == null)
                {
                    notifier.SendErrorNotification("Sorry, couldn't retrieve team information!");
                    NavigationManager.NavigateTo("/manageteams");
                }

                allPlayers = await client.GetFromJsonAsync<List<PlayerDto>>("/api/Player");
                if (allPlayers == null || allPlayers.Count == 0)
                {
                    notifier.SendErrorNotification("Sorry, couldn't retrieve players!");
                    NavigationManager.NavigateTo("/manageteams");
                }

                initialPlayers = team!.Players.ToList();
                teamMaxPlayers = team.TeamSize;
                selectablePlayers.AddRange(allPlayers!);
                StateHasChanged();

                for (int i = 0; i < initialPlayers.Count; i++)
                {
                    await dropDowns[i].SelectItem(initialPlayers[i], raiseChange: false);
                    selectedPlayers[i] = initialPlayers[i];
                    selectablePlayers.RemoveAll(x => selectedPlayers.Any(y => y != null && y.Id == x.Id));
                }
            }

            if (selectedPlayers.Count != 0)
            {
                int i = 0;
                foreach (var selectedPlayer in selectedPlayers)
                {
                    if (selectedPlayer != null)
                    {
                        await dropDowns[i].SelectItem(selectedPlayer, raiseChange: false);
                        dropDowns[i].Value = selectedPlayer.Id;
                    }

                    i++;
                }
            }
        }

        public async Task UpdateData(string args, int dropDownIndex)
        {
            var selectedPlayer = selectablePlayers.Where(x => x.Id == args).FirstOrDefault();
            if (selectedPlayer == null)
            {
                return;
            }
            selectablePlayers.RemoveAll(x => x.Id == args);

            var deselectedPlayer = allPlayers.FirstOrDefault(x => x.Id == dropDowns[dropDownIndex].Value?.ToString());
            if (deselectedPlayer != null)
            {
                selectablePlayers.Add(deselectedPlayer);
                selectedPlayers[dropDownIndex] = selectedPlayer;
            }
            else
            {
                selectedPlayers[dropDownIndex] = selectedPlayer;
            }

            dropDowns[dropDownIndex].Value = selectedPlayer.Id;

            IsValueInitial = initialPlayers.Select(p => p.Id).ToHashSet().SetEquals(selectedPlayers.Where(x => x != null).Select(p => p.Id).ToHashSet());
            StateHasChanged();
            await StateChange.InvokeAsync();
        }
    }
}
