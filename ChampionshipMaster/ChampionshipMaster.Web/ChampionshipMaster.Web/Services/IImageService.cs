namespace ChampionshipMaster.Web.Services
{
    public interface IImageService
    {
        string? GetImageFileType(string imageString);
        bool StartsWith(byte[] imageBytes, byte[] signature);
        string GetImageFileTypeFromSignature(byte[] signature);
        Task<string> ConvertToBase64String(Radzen.FileInfo file);
    }
}
