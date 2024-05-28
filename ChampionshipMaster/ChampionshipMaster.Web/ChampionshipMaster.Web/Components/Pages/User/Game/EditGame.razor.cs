using ChampionshipMaster.Web.Services;
using Radzen.Blazor;
using System.Net.Http;
using System.Text.Json;

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
        RadzenRadioButtonList<string> radioButtonList = new RadzenRadioButtonList<string>();
        List<RadzenNumeric<int>> radzenNumerics = new()
        {
            new(),
            new()
        };

        string nameRequestUrl = "api/Game/changeGameName";

        //int teamPoints[0] = 0;
        //int teamPoints[1] = 0;
        List<int> teamPoints = new() { 0, 0 };

        int team1InitialPoints = 0;
        int team2InitialPoints = 0;

        int maxTeamPoints;

        string? currentGameStatus;
        string? initialGameStatus;
        List<string> teams = new() { "", "" };
        //string team1 = "";
        //string team2 = "";
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
                else
                {
                    notifier.SendErrorNotification("Couldn't retrieve game statuses");
                    NavigationManager.NavigateTo("/managegames");
                }
                
                if (game != null)
                {
                    changeGameName.SetInitialValue(game.Name!);
                    nameRequestUrl += $"?gameId={id}";

                    initialGameStatus = game.GameStatusName;
                    currentGameStatus = initialGameStatus;

                    teamPoints[0] = game.BluePoints ?? 0;
                    teamPoints[1] = game.RedPoints ?? 0;
                    team1InitialPoints = game.BluePoints ?? 0;
                    team2InitialPoints = game.RedPoints ?? 0;

                    var maxPoints = game.MaxPoints;
                    if (maxPoints != null)
                    {
                        maxTeamPoints = maxPoints.Value;
                        teams[0] = game.BlueTeamName!;
                        teams[1] = game.RedTeamName!;
                    }

                    StateHasChanged();
                }
                else
                {
                    notifier.SendErrorNotification("Couldn't retrieve game info");
                    NavigationManager.NavigateTo("/managegames");
                }
            }

            //for (int i = 0; i < 2; i++)
            //{
            //    radzenNumerics[i].Value = teamPoints[i];
            //}

            //StateHasChanged();
        }
        private bool highlightedLeft = false;
        private bool highlightedRight = false;

        // Other variables and methods...

        private void HighLightLeft(bool state)
        {
            highlightedLeft = state;
        }

        private void HighLightRight(bool state)
        {
            highlightedRight = state;
        }

        public void CheckButtonState()
        {
            isValueInitial = changeGameName.IsValueInitial 
                && (winnerTeamName == null) 
                && (initialGameStatus == currentGameStatus || currentGameStatus == null)
                && (teamPoints[0] == team1InitialPoints && teamPoints[1] == team2InitialPoints);
        }

        public async Task OnClickSave()
        {
            if (!await tokenService.ValidateToken())
            {
                notifier.SendInformationalNotification("You're not logged in or your session has expired");
                NavigationManager.NavigateTo("/login");
            }

            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            

            if (!changeGameName.IsValueInitial)
            {
                var newName = changeGameName.CurrentValue;
                Dictionary<string, string> content = new Dictionary<string, string>
                {
                    { "newName", newName! }
                };

                var request = await client.PostAsJsonAsync(nameRequestUrl, content);
                var body = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    notifier.SendSuccessNotification("Game name updated successfully!");
                }
                else
                {
                    notifier.SendErrorNotification(body);
                }
            }
            

            //await changeGameName.OnClick();
            await SendGameInfo();
            NavigationManager.NavigateTo("/managegames");
        }

        void OnChangeRadio(object value)
        {
            if (value != null)
            {
                string selectedValue = value.ToString();
                winnerTeamName = selectedValue; // Update the selected value

                if (selectedValue == teams[0])
                {
                    highlightedLeft = true;
                    highlightedRight = false;
                    teamPoints[0] = maxTeamPoints;
                    teamPoints[1] = teamPoints[1] < 10 ? teamPoints[1] : 9;
                }
                else if (selectedValue == teams[1])
                {
                    highlightedLeft = false;
                    highlightedRight = true;
                    teamPoints[1] = maxTeamPoints;
                    teamPoints[0] = teamPoints[0] < 10 ? teamPoints[0] : 9;
                }
            }
            isValueInitial = changeGameName.IsValueInitial
                && (winnerTeamName == null)
                && (initialGameStatus == currentGameStatus || currentGameStatus == null)
                && (teamPoints[0] == team1InitialPoints && teamPoints[1] == team2InitialPoints);

            StateHasChanged();
        }

        void OnChangeDropDown(object args)
        {
            if (args.ToString() == "Finished")
            {
                radioButtonList.Change.InvokeAsync(teams[0]);
            }

            isValueInitial = changeGameName.IsValueInitial
                && (winnerTeamName == null)
                && (initialGameStatus == currentGameStatus || currentGameStatus == null)
                && (teamPoints[0] == team1InitialPoints && teamPoints[1] == team2InitialPoints);

            StateHasChanged();
        }

        void OnChangeNumeric(int args, int teamIndex)
        {
            //Check if input equals 10 when the team is not selected as winner
            if (args >= 10 && (radioButtonList.Value != null && teams[teamIndex] != radioButtonList.Value))
            {
                teamPoints[teamIndex] = 9;
                radzenNumerics[teamIndex].Value = 9;
            }

            //Check if input is different than 10 when the team is selected as winner
            if (args < 10 && teams[teamIndex] == radioButtonList.Value)
            {
                radioButtonList.Change.InvokeAsync(arg:null);
            }

            //Check if input is 10 and the winner is not set
            if (args == 10 && radioButtonList.Value == null)
            {
                radioButtonList.Change.InvokeAsync(teams[teamIndex]);
            }

            isValueInitial = changeGameName.IsValueInitial
                && (winnerTeamName == null)
                && (initialGameStatus == currentGameStatus || currentGameStatus == null)
                && (teamPoints[0] == team1InitialPoints && teamPoints[1] == team2InitialPoints);

            StateHasChanged();
        }

        async Task SendGameInfo()
        {
            GameDto gameInfo = new GameDto()
            {
                GameStatusName = currentGameStatus,
                WinnerName = winnerTeamName
            };

            if (currentGameStatus == null)
            {
                return;
            }

            if (!radioButtonList.Disabled)
            {
                //gameInfo.BluePoints = teamPoints[0];
                //gameInfo.RedPoints = teamPoints[1];
                gameInfo.BluePoints = radioButtonList.Value == teams[0] ? 10 : teamPoints[0];
                gameInfo.RedPoints = radioButtonList.Value == teams[1] ? 10 : teamPoints[1];
            }

            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = await client.PutAsJsonAsync($"/api/Game?gameId={id}", gameInfo);

            if (request.IsSuccessStatusCode)
            {
                notifier.SendSuccessNotification("Game details updated successfully!");
            }
            else
            {
                notifier.SendErrorNotification("Something went wrong!");
            }
        }
    }
}
