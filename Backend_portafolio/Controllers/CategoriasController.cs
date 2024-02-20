using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
	public class CategoriasController : Controller
	{
		[HttpGet]
		public IActionResult Crear()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Crear(Categoria categoria)
		{
			if(!ModelState.IsValid)
			{
				return View(categoria);
			}

			return View();
		}
	}
}
