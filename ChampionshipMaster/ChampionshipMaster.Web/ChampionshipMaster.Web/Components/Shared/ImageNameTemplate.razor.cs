using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.Web.Services;
using static ChampionshipMaster.Web.Components.Shared.ImageUpload;

namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class ImageNameTemplate : ComponentBase
    {
        [Inject] IImageService imageService { get; set; } = default!;
        [Parameter] public byte[]? Image { get; set; }
        [Parameter] public string? Name { get; set; }
        [Parameter] public ImageTypeOptions ImageType { get; set; }

        public enum ImageTypeOptions
        {
            Avatar,
            Team,
            Championship
        }

        string? imagePath;

        protected override void OnInitialized()
        {
            if (Image == null)
            {
                if (ImageType == ImageTypeOptions.Avatar) { imagePath = "images/defaultAvatar.png"; }
                else if (ImageType == ImageTypeOptions.Team) { imagePath = "images/defaultTeam.png"; }
                else if (ImageType == ImageTypeOptions.Championship) { imagePath = "images/defaultChampionship.png"; }
                else { imagePath = "images/defaultLogo.png"; }
            }
            else
            {
                string base64image = Convert.ToBase64String(Image);
                var imageType = imageService.GetImageFileType(base64image);
                imagePath = $"data:image/{imageType};base64,{base64image}";
            }
        }
    }
}
