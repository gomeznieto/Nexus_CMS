using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<IActionResult> Index()
		{
			var categorias = await _repositoryCategorias.Obtener();
			
			return View(categorias.OrderBy(x => x.name).ToList());
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
			var existe = await _repositoryCategorias.Existe(categoria.name);

			if(existe)
			{
				ModelState.AddModelError(nameof(categoria.name), $"El nombre {categoria.name} ya existe!");
				return View(categoria);
			}

			categoria.name = categoria.name.Trim();

			//Crea la categoria
			await _repositoryCategorias.Crear(categoria);

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> VerificarExisteCategoria(string name)
		{
			var existeCategoria = await _repositoryCategorias.Existe(name);

			if(existeCategoria)
				return Json($"EL nombre {name} ya existe!");

			return Json(true);

		}

		[HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
			var categoriaCambiar = await _repositoryCategorias.ObtenerPorId(id);

			if (categoriaCambiar == null)
				return RedirectToAction("NoEncontrado", "Home");

			return View(categoriaCambiar);
		}

		[HttpPost]
		public async Task<IActionResult> Editar(Categoria categoria)
		{
			//Validar errores del Model
			if(!ModelState.IsValid)
				return View(categoria);

			//Validar que la categoria no exista en la base de datos
			var existe = await _repositoryCategorias.Existe(categoria.name);

			if (existe)
			{
				ModelState.AddModelError(nameof(categoria.name), $"El nombre {categoria.name} ya existe!");
				return View(categoria);
			}

			categoria.name = categoria.name.Trim();

			await _repositoryCategorias.Editar(categoria);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Borrar(int id)
		{
			var categoria = await _repositoryCategorias.ObtenerPorId(id);

			if (categoria == null)
				return View("NoEncontrado", "Home");

			await _repositoryCategorias.Borrar(id);

			return RedirectToAction("Index");
		}
	}
}
