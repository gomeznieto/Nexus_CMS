using System.Text.RegularExpressions;

namespace Backend_portafolio.Helper
{
    public static class Utils
    {
        public static string GenerateSlug(string title)
        {
            string slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, @"\s+", "-");                 // espacios → guiones
            slug = Regex.Replace(slug, @"[^\w\-]", "");              // quitar símbolos raros
            slug = Regex.Replace(slug, @"\-{2,}", "-").Trim('-');    // múltiples guiones
            return slug;
        }
    }
}
