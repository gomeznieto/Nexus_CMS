using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class FormatsController : Controller
    {
        private readonly IRepositoryFormat _repositoryFormat;
        private readonly IUsersService _usersService;

        public FormatsController(IRepositoryFormat repositoryFormat, IUsersService usersService)
        {
            _repositoryFormat = repositoryFormat;
            _usersService = usersService;
        }

		/***********/
		/*  INDEX  */
		/***********/

		public async Task<IActionResult> Index()
        {
			try
			{
				var userID =  _usersService.ObtenerUsuario();
				var formats = await _repositoryFormat.Obtener(userID);

				return View(formats);
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}
        }


		/***********/
		/*  CREAR  */
		/***********/

		[HttpGet]
        public IActionResult Crear()
        {
			var viewModel = new Format();

            var userId = _usersService.ObtenerUsuario();

			viewModel.user_id = userId;
			
			return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Format model)
        {
			var userId = _usersService.ObtenerUsuario();

			try
			{
				//Validamos Model
				if (!ModelState.IsValid || model.user_id != userId) //Si el formato no corresponde o el usuario logueado no es el mismo que la información del formulario
				{
					return View(model);
				}

				//Creamos formato
				await _repositoryFormat.Crear(model);

				//Actualizar Session de Formatos para barra de navegacion
				await Helper.Session.UpdateSession(HttpContext, _repositoryFormat, userId);

				return RedirectToAction("Index");
			}
			catch (Exception)
			{
				return RedirectToAction("Index");
			}
		}

        [HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
			try
			{
				//verificamos si existe
				var format = await _repositoryFormat.ObtenerPorId(id);

				if (format is null)
					return RedirectToAction("NoEncontrado", "Home");

				return View(format);
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Home");
			}
		}


		/***********/
		/*  EDITAR */
		/***********/

		[HttpPost]
        public async Task<IActionResult> Editar(Format model)
        {
			try
			{
				var userID = _usersService.ObtenerUsuario();

				//Verificamos Model
				if (!ModelState.IsValid || userID != model.user_id)
					return View(model);

				//Verificamos si existe
				var existe = await _repositoryFormat.ObtenerPorId(model.id);

				if (existe is null)
					return RedirectToAction("NoEncontrado", "Home");

				//Editamos
				await _repositoryFormat.Editar(model);

				return RedirectToAction("Index");
			}
			catch (Exception)
			{
				return RedirectToAction("Index");
			}
		}

		/***************/
		/*   BORRAR    */
		/***************/
		[HttpPost]
        public async Task<IActionResult>Borrar(int id)
		{
			try
			{
				var userID = _usersService.ObtenerUsuario();

				var existe = await _repositoryFormat.ObtenerPorId(id);

				if (existe is null)
					return RedirectToAction("NoEncontrado", "Home");

				var borrar = await _repositoryFormat.sePuedeBorrar(id);

				if (!borrar)
					return Json(new { error = true, mensaje = "No se puede borrar porque el formato se encuentra en uso" });

				await _repositoryFormat.Borrar(id);

				//Actualizar Session de Formatos para barra de navegacion
				await Helper.Session.UpdateSession(HttpContext, _repositoryFormat, userID);

				return Json(new { error = false, mensaje = "Borrado con Éxito" });
			}
			catch (Exception)
			{
				return Json(new { error = true, mensaje = "Se producjo un error al momento de intentar borrar. Pruebe en otro momento!" });
			}
		}


		/***************/
		/*  FUNCIONES  */
		/***************/

		[HttpGet]
		public async Task<IActionResult> VerificarExisteFormato(string name)
		{
			try
			{
				var userID = _usersService.ObtenerUsuario();

				var existeCategoria = await _repositoryFormat.Existe(name, userID);

				if (existeCategoria)
					return Json($"El nombre {name} ya existe!");

				return Json(true);
			}
			catch (Exception)
			{

				return Json($"Se produjo un error al intentear validar {name}. Intente con otro nombre o en otro momento!");
			}

		}

        [HttpGet]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> apiJSON()
        {
			try
			{
                var userID = _usersService.ObtenerUsuario();

                var formatos = await _repositoryFormat.Obtener(userID);
				return Json(formatos);
			}
			catch (Exception)
			{
				return Json(new { error = true, mensaje = "Se ha producido un error."});
			}
        }
    }
}
