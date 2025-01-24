using Backend_portafolio.Models;
using System.Text.Json;
using Backend_portafolio.Sevices;
using Backend_portafolio.Entities;

namespace Backend_portafolio.Helper
{
    public static class Session
    {

        public static async Task UpdateSession(HttpContext httpContext, IFormatService formatService, int user_id)
        {
			try
			{
				var formats = await formatService.GetAllFormat();
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

        public static void CrearModalSuccess(string msg, string path, HttpContext httpContext)
        {
            //Crear mensaje de éxito para modal
            var successModal = new ModalViewModel { message = msg, type = true, path = path};
            SuccessSession(httpContext, successModal);
        }

        public static void CrearModalError(string msg, string path, HttpContext httpContext)
        {
            //Crear mensaje de éxito para modal
            var errorModal = new ModalViewModel { message = msg, type = true, path = path };
            ErrorSession(httpContext, errorModal);
        }

        public static void CantidadPostsSession(HttpContext httpContext, int cantidad)
        {
            var formatsJson = JsonSerializer.Serialize(cantidad);
            httpContext.Session.SetString("CantidadPosts", formatsJson);
        }

        public static void CantidadUsersSession(HttpContext httpContext, int cantidad)
        {
            var formatsJson = JsonSerializer.Serialize(cantidad);
            httpContext.Session.SetString("CantidadUsers", formatsJson);
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

        public static int GetCantidadUsersSession(HttpContext httpContext)
        {
            var errorModal = httpContext.Session.GetString("CantidadUsers");

            if (errorModal != null)
            {
                return JsonSerializer.Deserialize<int>(errorModal);
            }

            return -1;
        }

        public static List<User> GetSearchedUserList(HttpContext httpContext)
        {
            var searchedListUser = httpContext.Session.GetString("searchedUserList");

            if (searchedListUser != null)
            {
                return JsonSerializer.Deserialize<List<User>>(searchedListUser);
            }

            return null;
        }

        public static void SearchedUserList(HttpContext httpContext, List<User> userList)
        {
            var formatsJson = JsonSerializer.Serialize(userList);
            httpContext.Session.SetString("searchedUserList", formatsJson);
        }
    }
}
