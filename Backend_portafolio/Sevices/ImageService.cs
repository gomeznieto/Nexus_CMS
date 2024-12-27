namespace Backend_portafolio.Sevices
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string userName, string subfolder = "images");
    }
    public class ImageService : IImageService
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string userName, string subfolder)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("El archivo no es válido.");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", subfolder);
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(userName)}_{Path.GetRandomFileName()}.jpg";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/img/{subfolder}/{uniqueFileName}";
        }
    }
}
