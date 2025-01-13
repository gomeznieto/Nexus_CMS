using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend_portafolio.Controllers
{
    public class CategoriasController : Controller
	{
		private readonly IRepositoryCategorias _repositoryCategorias;
		private readonly IUsersService _usersService;

		public CategoriasController(IRepositoryCategorias repositoryCategorias, IUsersService usersService)
        {
			_repositoryCategorias = repositoryCategorias;
			_usersService = usersService;
		}

		/************/
		/*  INDEX  */
		/************/
		[HttpGet]
		public async Task<IActionResult> Index(int page = 1)
		{
			try
			{
				var userID = _usersService.ObtenerUsuario();

				var categorias = await _repositoryCategorias.Obtener(userID);

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
				var userID = _usersService.ObtenerUsuario();
				var categorias = await _repositoryCategorias.Obtener(userID);

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
			var userID = _usersService.ObtenerUsuario();

			var viewModel = new Categoria();
			viewModel.user_id = userID;

			return View(viewModel);
		}

		[HttpPost]
		public async Task <IActionResult> Crear(Categoria categoria)
		{
			//Validamos los datos que nos llegan del formulario
			var userID = _usersService.ObtenerUsuario();

			if(!ModelState.IsValid || categoria.user_id != userID)
			{
				return View(categoria);
			}

			//Validar que la categoria no exista en la base de datos
			var existe = await _repositoryCategorias.Existe(categoria.name, userID);

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
			var userID = _usersService.ObtenerUsuario();

			//Validar errores del Model
			if(!ModelState.IsValid || userID != categoria.user_id)
				return View(categoria);

			//Validar que la categoria no exista en la base de datos
			var existe = await _repositoryCategorias.Existe(categoria.name, userID);

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
		public async Task<IActionResult> VerificarExisteCategoria(string name, int userID)
		{
			var existeCategoria = await _repositoryCategorias.Existe(name, userID);

			if (existeCategoria)
				return Json($"El nombre {name} ya existe!");

			return Json(true);

		}
	}
}
