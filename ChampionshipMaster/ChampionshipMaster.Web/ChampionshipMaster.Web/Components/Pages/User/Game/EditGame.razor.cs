using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Net.Http;

namespace ChampionshipMaster.Web.Components.Pages.User.Game
{
    public partial class EditGame : ComponentBase
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

        [Parameter] public string id { get; set; }

        bool isValueInitial = true;
        bool isLogged = false;

        ChangeName changeGameName;

        string nameRequestUrl = "api/Game/changeGameName";
        //string logoRequestUrl = "api/Teams/changeTeamLogo";
        //string teamMembersRequestUrl = "api/Teams/setPlayers";

        int team1Points = 0;
        int team2Points = 0;

        int maxTeamPoints;

        string status = "";

        List<string> teams = new List<string>();
        string? winnerTeamName;
        List<GameStatusDto>? gameStatuses;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendWarningNotification("Your session has ran out or you're not logged in");
                    NavigationManager.NavigateTo("/login");
                }
                isLogged = true;
                StateHasChanged();


                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);

                var game = await client.GetFromJsonAsync<GameDto>($"api/Game/{id}");
                var statuses = await client.GetFromJsonAsync<List<GameStatusDto>>($"api/GameStatus");
                if(statuses != null)
                {
                    gameStatuses = statuses;
                }
                
                if (game != null)
                {
                    changeGameName.SetInitialValue(game.Name!);
                    var maxPoints = game.MaxPoints;
                    if (maxPoints != null)
                    {
                        maxTeamPoints = maxPoints.Value;
                        teams.Add(game.BlueTeamName!);
                        teams.Add(game.RedTeamName!);
                    }

                    StateHasChanged();
                }
            }
        }
        public void CheckButtonState()
        {
            isValueInitial = changeGameName.isValueInitial && changeGameName.isValueInitial && changeGameName.isValueInitial;
        }

        public async Task OnClick()
        {

            await changeGameName.OnClick();
            return;
        }
        void OnChange(object value)
        {
            if (value != null)
            {
                string selectedValue = value.ToString();
                winnerTeamName = selectedValue; // Update the selected value

                // Assuming teams.FirstOrDefault() and teams.Skip(1).FirstOrDefault() are strings
                if (selectedValue == teams.FirstOrDefault())
                {
                    team1Points = maxTeamPoints;
                    team2Points = 0; // Clear points for the other team
                }
                else if (selectedValue == teams.Skip(1).FirstOrDefault())
                {
                    team2Points = maxTeamPoints;
                    team1Points = 0; // Clear points for the other team
                }
            }
        }

    }
}
