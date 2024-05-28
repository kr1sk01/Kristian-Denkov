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
        public SetImageForOptions SetImageFor { get; set; }
        [Parameter]
        public string? Class { get; set; }

        public enum SetImageForOptions
        {
            Avatar,
            Team,
            Championship
        }

        const long FileSizeLimit = 40 * 1024;
        public bool IsValueInitial = true;
        string imagePath = string.Empty;
        string newImagePath = string.Empty;

        public Radzen.FileInfo? UploadedImage = null;
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
                IsValueInitial = true;
                StateHasChanged();
                await StateChange.InvokeAsync();
                newImagePath = string.Empty;
                return;
            }

            if (args.Files.First().Size > FileSizeLimit)
            {
                notifier.SendErrorNotification($"The image you selected exceeds the size limit of {FileSizeLimit / 1024}KB", 5);
                await upload.ClearFiles();
                IsValueInitial = true;
                newImagePath = string.Empty;
                StateHasChanged();
                await StateChange.InvokeAsync();
                return;
            }

            UploadedImage = args.Files.First();
            var newImage = await imageService.ConvertToBase64String(UploadedImage);
            newImagePath = $"data:{UploadedImage.ContentType};base64,{newImage}";
            IsValueInitial = false;
            StateHasChanged();
            await StateChange.InvokeAsync();
        }
    }
}
