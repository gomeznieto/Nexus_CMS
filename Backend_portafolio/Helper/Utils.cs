using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace Backend_portafolio.Helper
{
    public static class Utils
    {
        // Método para generar slug a partir de un título
        public static string GenerateSlug(string title)
        {
            string slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, @"\s+", "-");                 // espacios → guiones
            slug = Regex.Replace(slug, @"[^\w\-]", "");              // quitar símbolos raros
            slug = Regex.Replace(slug, @"\-{2,}", "-").Trim('-');    // múltiples guiones
            return slug;
        }

        // Método para transformar SVG a HTML sin estilos
        public async static Task<string> SvgHtmlConverter(string icon, IWebHostEnvironment _webHostEnvironment)
        {
            string svgContent = null;

            // Construir la ruta física del archivo SVG
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, icon.TrimStart('/'));

            if (File.Exists(filePath))
            {
                svgContent = await System.IO.File.ReadAllTextAsync(filePath);

                svgContent = svgContent.Replace("fill=\"#0F0F0F\"", "fill=\"currentColor\"");
                svgContent = svgContent.Replace("fill=\"none\"", "fill=\"currentColor\"");
                svgContent = svgContent.Replace("fill=\"#000000\"", "fill=\"currentColor\"");
                svgContent = svgContent.Replace("fill=\"#000\"", "fill=\"currentColor\"");
                svgContent = svgContent.Replace("fill=\"#fff\"", "fill=\"currentColor\"");
                svgContent = svgContent.Replace("width=\"800px\"", "");
                svgContent = svgContent.Replace("height=\"800px\"", "");

                if (!svgContent.Contains("fill=\"currentColor\"") && svgContent.Contains("<svg"))
                {
                    svgContent = svgContent.Replace("<svg", "<svg fill=\"currentColor\"");
                }
            }

            return svgContent;
        }
    }

}
