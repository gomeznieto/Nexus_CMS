using Backend_portafolio.Sevices;
using System.Text.Json;

namespace Backend_portafolio.Helper
{
    public static class Session
    {
        public static async Task UpdateSession(HttpContext httpContext, IRepositoryFormat repositoryFormat)
        {
			try
			{
				var formats = await repositoryFormat.Obtener();
				var formatsJson = JsonSerializer.Serialize(formats.ToList());
				httpContext.Session.SetString("Formats", formatsJson);
			}
			catch (Exception)
			{
				throw new Exception("Se ha producido un error");
			}
        }
    }
}
