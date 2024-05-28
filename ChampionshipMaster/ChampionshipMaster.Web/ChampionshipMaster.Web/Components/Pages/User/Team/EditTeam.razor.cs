﻿using ChampionshipMaster.Web.Services;

namespace ChampionshipMaster.Web.Components.Pages.User.Team
{
    public partial class EditTeam : ComponentBase
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
        IImageService imageService { get; set; } = default!;
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        [Parameter] public string id { get; set; }

        bool isValueInitial = true;
        bool isLogged = false;
        ChangeTeamMembers changeTeamMembers;
        ChangeName changeTeamName;
        ImageUpload changeTeamLogo;

        string nameRequestUrl = "api/Teams/changeTeamName";
        string logoRequestUrl = "api/Teams/changeTeamLogo";
        string teamMembersRequestUrl = "api/Teams/setPlayers";

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

                var team = await client.GetFromJsonAsync<TeamDto>($"api/Teams/{id}");
                if (team != null)
                {
                    changeTeamName.SetInitialValue(team.Name!);
                    nameRequestUrl += $"?teamId={id}";

                    changeTeamLogo.UpdateDisplayedImagePath(Convert.ToBase64String(team.Logo ?? new byte[0]));
                    logoRequestUrl += $"?teamId={id}";

                    teamMembersRequestUrl += $"?teamId={id}";
                    StateHasChanged();
                }
            }
        }

        public void CheckButtonState()
        {
            isValueInitial = changeTeamName.IsValueInitial && changeTeamLogo.IsValueInitial && changeTeamMembers.isValueInitial;
        }

        public async Task OnClick()
        {
            await SubmitNewTeamName();
            await SubmitNewTeamLogo();
            await changeTeamMembers.SetTeamMembers();
            NavigationManager.NavigateTo("/manageteams");
        }

        public async Task SubmitNewTeamName()
        {
            if (!await tokenService.ValidateToken())
            {
                notifier.SendInformationalNotification("You're not logged in or your session has expired");
                NavigationManager.NavigateTo("/login");
            }

            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");



            if (!changeTeamName.IsValueInitial)
            {
                var newName = changeTeamName.CurrentValue;
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
        }

        public async Task SubmitNewTeamLogo()
        {
            if (!changeTeamLogo.IsValueInitial)
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendInformationalNotification("You're not logged in or your session has expired");
                    NavigationManager.NavigateTo("/login");
                }

                var token = await tokenService.GetToken();
                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var imageBase64 = await imageService.ConvertToBase64String(changeTeamLogo.UploadedImage!);
                Dictionary<string, string> content = new Dictionary<string, string>
                {
                    { "newImage", imageBase64 }
                };

                var response = await client.PostAsJsonAsync(logoRequestUrl, content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    notifier.SendSuccessNotification(body);
                }
                else
                {
                    notifier.SendErrorNotification(body);
                }
            }
        }
    }
}
