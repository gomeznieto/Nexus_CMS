using Backend_portafolio.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
	public class SessionController:Controller
	{

		[AllowAnonymous]
		public IActionResult DeleteErrorModal()
		{
			try
			{
				Session.DeleteErrorSession(HttpContext);
				return Json(true);

			}
			catch (Exception)
			{
				return Json(false);
			}

		}

        public IActionResult DeleteSuccessModal()
        {
            try
            {
                Session.DeleteSuccessSession(HttpContext);
				return Json(true);

            }
            catch (Exception)
            {
                return Json(false);
            }

        }

        public IActionResult ChangeNumberPosts(int cantidad)
		{
			try
			{
				Session.CantidadPostsSession(HttpContext, cantidad);
				return Json(true);

			}
			catch (Exception)
			{
				return Json(false);
			}

		}

        public IActionResult ChangeNumberUsers(int cantidad)
        {
            try
            {
                Session.CantidadUsersSession(HttpContext, cantidad);
                return Json(true);

            }
            catch (Exception)
            {
                return Json(false);
            }

        }

    }
}
