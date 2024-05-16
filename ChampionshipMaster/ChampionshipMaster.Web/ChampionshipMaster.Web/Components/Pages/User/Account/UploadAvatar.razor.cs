using ChampionshipMaster.SHARED.DTO;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen.Blazor;
using Radzen;
using System.IdentityModel.Tokens.Jwt;

namespace ChampionshipMaster.Web.Components.Pages.User.Account
{
    public partial class UploadAvatar : ComponentBase
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
        [Inject]
        IImageService imageService { get; set; } = default!;

        const long FileSizeLimit = 40 * 1024;
        bool disableSaveButton = true;
        string imagePath = string.Empty;
        string newImagePath = string.Empty;
        string base64EncodedImageData = string.Empty;
        string base64EncodedNewImageData = string.Empty;

        Radzen.FileInfo? uploadedImage = null;
        RadzenUpload upload = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!await tokenService.ValidateToken())
                {
                    NavigationManager.NavigateTo("/login");
                }

                var tokenString = await tokenService.GetToken();
                if (tokenString != null)
                {
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
                    try
                    {
                        var imageResult = await _localStorage.GetAsync<string>("playerAvatar");
                        if (imageResult.Success && !string.IsNullOrEmpty(imageResult.Value))
                        {
                            base64EncodedImageData = imageResult.Value!;
                            var imageType = imageService.GetImageFileType(base64EncodedImageData);
                            imagePath = $"data:image/{imageType};base64,{base64EncodedImageData}";
                        }
                        else
                        {
                            imagePath = "images/defaultAvatar.png";
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    StateHasChanged();
                }
            }
        }

        async Task OnProgress(UploadProgressArgs args)
        {
        }

        async Task OnChange(UploadChangeEventArgs args)
        {
            if (!args.Files.Any())
            {
                disableSaveButton = true;
                newImagePath = string.Empty;
                StateHasChanged();
                return;
            }

            if (args.Files.First().Size > FileSizeLimit)
            {
                notifier.SendErrorNotification($"The image you selected exceeds the size limit of {FileSizeLimit / 1024}KB", 5);
                await upload.ClearFiles();
                disableSaveButton = true;
                newImagePath = string.Empty;
                StateHasChanged();
                return;
            }

            uploadedImage = args.Files.First();
            var newImage = await imageService.ConvertToBase64String(uploadedImage);
            newImagePath = $"data:{uploadedImage.ContentType};base64,{newImage}";
            disableSaveButton = false;
            StateHasChanged();
        }

        async Task UploadImage()
        {
            var token = await tokenService.GetToken();
            using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var imageBase64 = await imageService.ConvertToBase64String(uploadedImage!);
            var content = new ProfileDto { Avatar = Convert.FromBase64String(imageBase64) };
            var response = await client.PostAsJsonAsync($"api/Player/changeAvatar/", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    await _localStorage.SetAsync("playerAvatar", imageBase64);
                    var body = await response.Content.ReadAsStringAsync();
                    notifier.SendSuccessNotification(body);
                    imagePath = $"data:image/{uploadedImage!.ContentType};base64,{imageBase64}";
                    newImagePath = string.Empty;
                    disableSaveButton = true;
                    await upload.ClearFiles();
                    StateHasChanged();
                    NavigationManager.NavigateTo("/profile", forceLoad: true);
                }
                catch
                {
                    notifier.SendErrorNotification("Something went wrong");
                }
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                notifier.SendErrorNotification(body);
            }
        }
    }
}
