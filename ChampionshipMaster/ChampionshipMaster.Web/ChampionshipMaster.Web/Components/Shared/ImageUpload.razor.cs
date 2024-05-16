using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class ImageUpload : ComponentBase
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
        IImageService imageService { get; set; } = default!;
        [Inject]
        ProtectedLocalStorage _localStorage { get; set; } = default!;

        [Parameter]
        public EventCallback StateChange { get; set; }
        [Parameter]
        public required string Title { get; set; }
        [Parameter]
        public string? ToLocalStorage { get; set; }
        [Parameter]
        public SetImageForOptions SetImageFor { get; set; }
        [Parameter]
        public required string RequestUrl { get; set; }
        [Parameter]
        public bool ReloadPageOnComplete { get; set; } = false;

        public enum SetImageForOptions
        {
            Avatar,
            Team,
            Championship
        }

        const long FileSizeLimit = 40 * 1024;
        public bool isValueInitial = true;
        string imagePath = string.Empty;
        string newImagePath = string.Empty;

        Radzen.FileInfo? uploadedImage = null;
        RadzenUpload upload = default!;

        public void UpdateDisplayedImagePath(string? DisplayedImageEncodedData)
        {
            if (!string.IsNullOrEmpty(DisplayedImageEncodedData))
            {
                var imageType = imageService.GetImageFileType(DisplayedImageEncodedData);
                imagePath = $"data:image/{imageType};base64,{DisplayedImageEncodedData}";
            }
            else
            {
                if (SetImageFor == SetImageForOptions.Avatar) { imagePath = "images/defaultAvatar.png"; }
                else if (SetImageFor == SetImageForOptions.Team) { imagePath = "images/defaultTeam.png"; }
                else if (SetImageFor == SetImageForOptions.Championship) { imagePath = "images/defaultChampionship.png"; }
                else { imagePath = "images/defaultLogo.png"; }
            }

            StateHasChanged();
        }

        async Task OnProgress(UploadProgressArgs args)
        {
        }

        async Task OnChange(UploadChangeEventArgs args)
        {
            if (!args.Files.Any())
            {
                isValueInitial = true;
                StateHasChanged();
                await StateChange.InvokeAsync();
                newImagePath = string.Empty;
                return;
            }

            if (args.Files.First().Size > FileSizeLimit)
            {
                notifier.SendErrorNotification($"The image you selected exceeds the size limit of {FileSizeLimit / 1024}KB", 5);
                await upload.ClearFiles();
                isValueInitial = true;
                newImagePath = string.Empty;
                StateHasChanged();
                await StateChange.InvokeAsync();
                return;
            }

            uploadedImage = args.Files.First();
            var newImage = await imageService.ConvertToBase64String(uploadedImage);
            newImagePath = $"data:{uploadedImage.ContentType};base64,{newImage}";
            isValueInitial = false;
            StateHasChanged();
            await StateChange.InvokeAsync();
        }

        public async Task UploadImage()
        {
            if (!isValueInitial)
            {
                if (!await tokenService.ValidateToken())
                {
                    notifier.SendInformationalNotification("You're not logged in or your session has expired");
                    NavigationManager.NavigateTo("/login");
                }

                var token = await tokenService.GetToken();
                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var imageBase64 = await imageService.ConvertToBase64String(uploadedImage!);
                Dictionary<string, string> content = new Dictionary<string, string>
                {
                    { "newImage", imageBase64 }
                };

                var response = await client.PostAsJsonAsync(RequestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        if (ToLocalStorage != null)
                        {
                            await _localStorage.SetAsync(ToLocalStorage, imageBase64);
                            var body = await response.Content.ReadAsStringAsync();
                            notifier.SendSuccessNotification(body);
                            imagePath = $"data:image/{uploadedImage!.ContentType};base64,{imageBase64}";
                            newImagePath = string.Empty;
                            await upload.ClearFiles();
                            isValueInitial = true;
                            StateHasChanged();
                            await StateChange.InvokeAsync();
                        }
                        else
                        {
                            var body = await response.Content.ReadAsStringAsync();
                            notifier.SendSuccessNotification(body);
                        }

                        if (ReloadPageOnComplete) { NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true); }
                    }
                    catch
                    {
                        notifier.SendErrorNotification("Something went wrong!");
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
}
