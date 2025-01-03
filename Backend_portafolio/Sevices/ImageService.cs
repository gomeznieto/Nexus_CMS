using Backend_portafolio.Models;

namespace Backend_portafolio.Sevices
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, User user, string subfolder = "images");
    }
    public class ImageService : IImageService
    {
        //Guarda un archivo que le pasemos en la carpeta del Usuario
        public async Task<string> UploadImageAsync(IFormFile imageFile, User user, string subfolder)
        {
            // Carpeta raíz
            string root = "img";

            // Verifica que el archivo no sea nulo o vacío
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("El archivo no es válido.");
            }

            // Define la carpeta de subcarpetas y crea si no existe
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", root, subfolder, user.email);
            Directory.CreateDirectory(uploadsFolder);

            // Verifica la extensión del archivo
            string fileNameWithExtension = imageFile.FileName;
            string extension = Path.GetExtension(fileNameWithExtension).ToLower();

            if (extension != ".jpg" && extension != ".png")
            {
                throw new ArgumentException("El tipo de archivo no es válido. Solo se permiten .jpg y .png.");
            }

            // Define el nombre único del archivo
            string uniqueFileName = $"{Path.GetRandomFileName()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Verifica el tamaño del archivo (por ejemplo, 2MB)
            const int maxFileSizeInBytes = 1 * 1024 * 1024; // 2 MB

            if (imageFile.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException($"No pudo guardarse la imagen.\nEl archivo excede el límite de tamaño permitido (1MB).");
            }

            // Borrar anterior
            string img_anterior = user.img;

            // Guardamos imagen
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);

                BorrarArchivo(user.img);
            }

            return $"/{root}/{subfolder}/{user.email}/{uniqueFileName}";
        }

        // Borra el archivo de la ruta que le indicamos
        void BorrarArchivo(string path)
        {
            if (path != null)
            {
                string path_anterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
                if (File.Exists(path_anterior))
                {
                    File.Delete(path_anterior);
                }
            }

        }
    }
}
