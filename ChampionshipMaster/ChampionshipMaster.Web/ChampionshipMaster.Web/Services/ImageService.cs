
namespace ChampionshipMaster.Web.Services
{
    public class ImageService : IImageService
    {
        public byte[][] fileSignatures = {
            [0x89, 0x50, 0x4E, 0x47], // PNG
            [0xFF, 0xD8, 0xFF], // JPEG
            [0xFF, 0xD8, 0xFF, 0xE0], // JPG (JPEG with EXIF)
            [0xFF, 0xD8, 0xFF, 0xE1] // JPG (JPEG with APP1)
        };

        public async Task<string> ConvertToBase64String(Radzen.FileInfo file)
        {
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
            var imageBase64 = Convert.ToBase64String(memoryStream.ToArray());
            return imageBase64;
        }

        public string? GetImageFileType(string imageString)
        {
            byte[] bytes = Convert.FromBase64String(imageString);

            // Compare the first few bytes of the decoded byte array with the magic numbers
            foreach (var signature in fileSignatures)
            {
                if (StartsWith(bytes, signature))
                {
                    return GetImageFileTypeFromSignature(signature);
                }
            }

            // If no match is found, return null or throw an exception
            return null;
        }

        public string GetImageFileTypeFromSignature(byte[] signature)
        {
            if (signature.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }))
                return "png";
            else if (signature.SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }))
                return "jpeg";
            else
                return "jpg";
        }

        public bool StartsWith(byte[] imageBytes, byte[] signature)
        {
            if (imageBytes.Length < signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (imageBytes[i] != signature[i])
                    return false;
            }
            return true;
        }
    }
}
