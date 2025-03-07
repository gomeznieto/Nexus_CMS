using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend_portafolio.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, UserViewModel user, string subfolder, string source = "");
    }
    public class ImageService : IImageService
    {
        //Guarda un archivo que le pasemos en la carpeta del Usuario
        public async Task<string> UploadImageAsync(IFormFile imageFile, UserViewModel user, string subfolder, string source)
        {
            // Carpeta raíz
            string root = "img";

            // Verifica que el archivo no sea nulo o esté vacío
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("El archivo no es válido.");
            }

            // Define la ruta de la carpeta
            string uploadsFolder;
            string savedPath;

            if (source.IsNullOrEmpty())
            {
                uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", root, subfolder, user.username);
                savedPath = Path.Combine(root, subfolder, user.username);
            }
            else
            {
                uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", root, subfolder, user.username, source);
                savedPath = Path.Combine(root, subfolder, user.username, source);
            }

            // Creamos el directorio
            Directory.CreateDirectory(uploadsFolder);

            // Verifica la extensión del archivo
            string fileNameWithExtension = imageFile.FileName;
            string extension = Path.GetExtension(fileNameWithExtension).ToLower();

            if (extension != ".jpg" && extension != ".png" && extension != ".svg")
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
            BorrarArchivo(uploadsFolder);

            // Guardamos imagen
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/{savedPath}/{uniqueFileName}";
        }


        // Borra el archivo de la ruta que le indicamos
        void BorrarArchivo(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

        }
    }
}
