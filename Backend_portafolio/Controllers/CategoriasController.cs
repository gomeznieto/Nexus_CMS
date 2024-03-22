using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend_portafolio.Controllers
{
	public class CategoriasController : Controller
	{
		private readonly IRepositoryCategorias _repositoryCategorias;

		public CategoriasController(IRepositoryCategorias repositoryCategorias)
        {
			_repositoryCategorias = repositoryCategorias;
		}

		/************/
		/*  INDEX  */
		/************/
		[HttpGet]
		public async Task<IActionResult> Index(int page = 1)
		{
			try
			{
				var categorias = await _repositoryCategorias.Obtener();

				ViewBag.Cantidad = categorias.Count();
				ViewBag.Message = $"No hay categorias para mostrar.";

				return View(categorias.OrderBy(x => x.name).ToList());
			} 
			catch (Exception)
			{
				//Crear mensaje de error para modal
				var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
				Session.ErrorSession(HttpContext, errorModal);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Index(string buscar)
		{
			try
			{
				var categorias = await _repositoryCategorias.Obtener();

				if (!buscar.IsNullOrEmpty())
				{
					categorias = categorias.Where(p => p.name.ToUpper().Contains(buscar.ToUpper()));
				}

				ViewBag.Cantidad = categorias.Count();
				ViewBag.Message = $"Sin resultados para \"{buscar}\".";

				return View(categorias.OrderBy(x => x.name).ToList());
			}
			catch (Exception)
			{

				//Crear mensaje de error para modal
				var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
				Session.ErrorSession(HttpContext, errorModal);

				return RedirectToAction("Index", "Home");
			}
		}


		/************/
		/*  CREAR   */
		/************/

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


		/************/
		/*  EDITAR  */
		/************/

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


		/************/
		/*  BORRAR  */
		/************/

		[HttpPost]
		public async Task<IActionResult> Borrar(int id)
		{
			//Verificar si existe
			var categoria = await _repositoryCategorias.ObtenerPorId(id);

			if (categoria == null)
				return View("NoEncontrado", "Home");


			//Verificar si no está en uso
			var borrar = await _repositoryCategorias.sePuedeBorrar(id);

			if(!borrar)
				return Json(new { error = true, mensaje = "No se puede borrar porque se encuentra en uso" });

			//Borrar
			await _repositoryCategorias.Borrar(id);

			return Json(new { error = false, mensaje = "Borrado con Éxito" });
		}

		/***************/
		/*  FUNCIONES  */
		/***************/
		[HttpGet]
		public async Task<IActionResult> VerificarExisteCategoria(string name)
		{
			var existeCategoria = await _repositoryCategorias.Existe(name);

			if (existeCategoria)
				return Json($"El nombre {name} ya existe!");

			return Json(true);

		}
	}
}
