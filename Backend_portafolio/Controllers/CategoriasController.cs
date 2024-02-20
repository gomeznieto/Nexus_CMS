using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Controllers
{
	public class CategoriasController : Controller
	{
		private readonly IRepositoryCategorias _repositoryCategorias;

		public CategoriasController(IRepositoryCategorias repositoryCategorias)
        {
			_repositoryCategorias = repositoryCategorias;
		}

        [HttpGet]
		public IActionResult Crear()
		{
			return View() ;
		}

		[HttpPost]
		public async Task <IActionResult> Crear(Categoria categoria)
		{
			//Validamos los datos que nos llegan del formulario
			if(!ModelState.IsValid)
			{
				return View(categoria);
			}

			//Validar que la categoria no exista en la base de datos
			var existe = await _repositoryCategorias.Existe(categoria);

			if(existe)
			{
				ModelState.AddModelError("", "La categoria ya existe");
				return View(categoria);
			}

			categoria.name = categoria.name.Trim();

			//Crea la categoria
			await _repositoryCategorias.Crear(categoria);

			return View();
		}
	}
}
