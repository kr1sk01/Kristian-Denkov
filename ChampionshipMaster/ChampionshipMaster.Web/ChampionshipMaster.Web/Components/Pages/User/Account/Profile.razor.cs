using ChampionshipMaster.Web.Components.Shared;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ChampionshipMaster.Web.Components.Pages.User.Account
{
    public partial class Profile : ComponentBase
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
        [Inject]
        ProtectedLocalStorage _localStorage { get; set; } = default!;

        ImageUpload playerAvatar;
        ChangeName changeUsername;
        bool loggedIn = false;
        string username = string.Empty;
        string email = string.Empty;
        string? base64EncodedImageData;
        bool isValueInitial = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!await tokenService.ValidateToken())
                {
                    NavigationManager.NavigateTo("/login");
                }

                loggedIn = true;
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());
                username = token.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value ?? "";
                changeUsername.SetInitialValue(username);
                email = token.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "";

                try
                {
                    var imageResult = await _localStorage.GetAsync<string>("playerAvatar");
                    if (imageResult.Success && !string.IsNullOrEmpty(imageResult.Value))
                    {
                        base64EncodedImageData = imageResult.Value!;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    playerAvatar.UpdateDisplayedImagePath(base64EncodedImageData);
                }

                StateHasChanged();
            }
        }

        public async Task OnClick()
        {
            if (!await tokenService.ValidateToken())
            {
                notifier.SendInformationalNotification("You're not logged in or your session has expired");
                NavigationManager.NavigateTo("/login");
            }

            //Submit New Username
            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            if (!changeUsername.IsValueInitial)
            {
                var newName = changeUsername.CurrentValue;
                Dictionary<string, string> content = new Dictionary<string, string>
                {
                    { "newName", newName! }
                };

                var request = await client.PostAsJsonAsync("api/Player/changeUsername", content);
                var body = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    try
                    {
                        var responseValue = JsonSerializer.Deserialize<Dictionary<string, string>>(body)!["jwtToken"];
                        await _localStorage.SetAsync("jwtToken", responseValue);

                        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                    }
                    catch (Exception ex)
                    {
                        notifier.SendErrorNotification("Something went wrong!");
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    notifier.SendErrorNotification(body);
                }
            }


            //await changeUsername.OnClick();
            await playerAvatar.UploadImage();
            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        }

        public async Task CheckButtonState()
        {
            isValueInitial = playerAvatar.isValueInitial && changeUsername.IsValueInitial;
        }
    }
}
