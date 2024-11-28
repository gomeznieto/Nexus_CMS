using Backend_portafolio.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
	public class ModalsController:Controller
	{

		public IActionResult DeleteErrorModal()
		{
			try
			{
				Session.DeleteErrorSession(HttpContext);

			}
			catch (Exception)
			{
				return Json(false);
			}

			return Json(true);
		}

        public IActionResult DeleteSuccessModal()
        {
            try
            {
                Session.DeleteSuccessSession(HttpContext);

            }
            catch (Exception)
            {
                return Json(false);
            }

            return Json(true);
        }

        public IActionResult ChangeNumberPosts(int cantidad)
		{
			try
			{
				Session.CantidadPostsSession(HttpContext, cantidad);

			}
			catch (Exception)
			{
				return Json(false);
			}

			return Json(true);
		}

	}
}
