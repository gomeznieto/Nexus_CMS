using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.SqlServer.Server;
using System.Text.Json;

namespace Backend_portafolio.Helper
{
    public static class Session
    {
        public static async Task UpdateSession(HttpContext httpContext, IRepositoryFormat repositoryFormat, int user_id)
        {
			try
			{
				var formats = await repositoryFormat.Obtener(user_id);
				var formatsJson = JsonSerializer.Serialize(formats.ToList());
				httpContext.Session.SetString("Formats", formatsJson);
			}
			catch (Exception)
			{
				throw new Exception("Se ha producido un error");
			}
        }

		public static void ErrorSession(HttpContext httpContext, ModalViewModel modal)
		{
            var formatsJson = JsonSerializer.Serialize(modal);
            httpContext.Session.SetString("Error", formatsJson);
        }

        public static void SuccessSession(HttpContext httpContext, ModalViewModel modal)
        {
            var formatsJson = JsonSerializer.Serialize(modal);
            httpContext.Session.SetString("Exito", formatsJson);
        }

        public static ModalViewModel GetErrorSession(HttpContext httpContext)
		{
            var errorModal = httpContext.Session.GetString("Error");
			if (errorModal != null)
			{
				return JsonSerializer.Deserialize<ModalViewModel>(errorModal);
			}

			return null;
        }

        public static ModalViewModel GetSuccessSession(HttpContext httpContext)
        {
            var successModal = httpContext.Session.GetString("Exito");
            if (successModal != null)
            {
                return JsonSerializer.Deserialize<ModalViewModel>(successModal);
            }

            return null;
        }

        public static void DeleteErrorSession(HttpContext httpContext)
        {
			httpContext.Session.Remove("Error");
        }

        public static void DeleteSuccessSession(HttpContext httpContext)
        {
            httpContext.Session.Remove("Exito");
        }

        public static void CantidadPostsSession(HttpContext httpContext, int cantidad)
        {
            var formatsJson = JsonSerializer.Serialize(cantidad);
            httpContext.Session.SetString("CantidadPosts", formatsJson);
        }

        public static int GetCantidadPostsSession(HttpContext httpContext)
        {
            var errorModal = httpContext.Session.GetString("CantidadPosts");

            if (errorModal != null)
            {
                return JsonSerializer.Deserialize<int>(errorModal);
            }

            return -1;
        }
    }
}
