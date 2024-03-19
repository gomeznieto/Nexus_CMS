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
			catch (Exception ex)
			{
				return Json(false);
			}

			return Json(true);
		}

	}
}
