﻿using ChampionshipMaster.Web.Components.Shared;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Diagnostics.Tracing;
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
        IImageService imageService { get; set; } = default!;
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        ProtectedLocalStorage _localStorage { get; set; } = default!;

        ImageUpload playerAvatar;
        ChangeName changeUsername;
        bool loggedIn = false;
        string username = string.Empty;
        string userId = string.Empty;
        string email = string.Empty;
        string requestUrl = "/api/Player";
        string? base64EncodedImageData;
        bool isValueInitial = true;
        PlayerDto editedPlayer = new();

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
                userId = token.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
                requestUrl += $"?playerId={userId}";

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

            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = await client.PutAsJsonAsync(requestUrl, editedPlayer);
            var body = await request.Content.ReadAsStringAsync();

            if (request.IsSuccessStatusCode)
            {
                try
                {
                    if (editedPlayer.Name != null)
                    {
                        var responseValue = JsonSerializer.Deserialize<Dictionary<string, string>>(body)!["jwtToken"];
                        await _localStorage.SetAsync("jwtToken", responseValue);
                    }

                    if (editedPlayer.Avatar != null)
                    {
                        var responseValue = JsonSerializer.Deserialize<Dictionary<string, string>>(body)!["image"];
                        await _localStorage.SetAsync("playerAvatar", responseValue);
                    }

                    NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                }
                catch
                {
                    notifier.SendErrorNotification("Something went wrong! You might have to log out and log back in again", 10);
                }
            }
            else
            {
                notifier.SendErrorNotification(body);
            }
        }

        public async Task CheckButtonState()
        {
            isValueInitial = playerAvatar.IsValueInitial && changeUsername.IsValueInitial;

            editedPlayer.Name = changeUsername.IsValueInitial ? null : changeUsername.CurrentValue;
            editedPlayer.Avatar = playerAvatar.IsValueInitial ? null : Convert.FromBase64String(await imageService.ConvertToBase64String(playerAvatar.UploadedImage!));
        }
    }
}
