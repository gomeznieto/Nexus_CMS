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
            // Carpeta raíz
            string root = "img";

            // Verifica que el archivo no sea nulo o vacío
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("El archivo no es válido.");
            }

            // Define la carpeta de subcarpetas y crea si no existe
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", root, subfolder);
            Directory.CreateDirectory(uploadsFolder);

            // Verifica la extensión del archivo
            string fileNameWithExtension = imageFile.FileName;
            string extension = Path.GetExtension(fileNameWithExtension).ToLower();

            if (extension != ".jpg" && extension != ".png")
            {
                throw new ArgumentException("El tipo de archivo no es válido. Solo se permiten .jpg y .png.");
            }

            // Define el nombre único del archivo
            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(userName)}_{Path.GetRandomFileName()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Verifica el tamaño del archivo (por ejemplo, 2MB)
            const int maxFileSizeInBytes = 1 * 1024 * 1024; // 2 MB

            if (imageFile.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException($"No pudo guardarse la imagen.\nEl archivo excede el límite de tamaño permitido (1MB).");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/{root}/{subfolder}/{uniqueFileName}";
        }
    }
}
