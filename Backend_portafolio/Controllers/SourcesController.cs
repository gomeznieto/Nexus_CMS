using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace Backend_portafolio.Controllers
{
    public class SourcesController : Controller
    {
        private readonly IRepositorySource _repositorySource;

        public SourcesController(IRepositorySource repositorySource)
        {
            _repositorySource = repositorySource;
        }


        /***************/
        /*    INDEX    */
        /***************/
        public async Task<IActionResult> Index()
        {
            try
            {
				var mediTypes = await _repositorySource.Obtener();

				return View(mediTypes);
			}
            catch (Exception)
            {
				//Crear mensaje de error para modal
				var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
				Session.ErrorSession(HttpContext, errorModal);

				return RedirectToAction("Index", "Home");
			}
        }


        /***************/
        /*   CREAR     */
        /***************/

        [HttpGet]
        public  IActionResult Crear()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Crear(Source mediaType)
		{
            try
            {
				//Validar Model
				if (!ModelState.IsValid)
				{
					return View(mediaType);
				}

				//Validar que exista
				var existe = await _repositorySource.Existe(mediaType.name);

				if (existe)
				{
					ModelState.AddModelError(null, "El media type ya existe");
					return View(mediaType);
				}

				await _repositorySource.Crear(mediaType);

				return RedirectToAction("Index");
			}
            catch (Exception)
            {
				//Crear mensaje de error para modal
				Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Se ha producido un error. Intente más tarde!", type = true, path = "Sources" });
				return RedirectToAction("Index");
			}
		}


        /***************/
        /*   EDITAR    */
        /***************/

        [HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
			try
			{
				var mediaType = await _repositorySource.ObtenerPorId(id);

				if (mediaType == null)
				{
					//Crear mensaje de error para modal
					Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡La Fuente que intenta editar no existe!", type = true, path = "Sources" });
					return RedirectToAction("Index");
				}

				return View(mediaType);
			}
			catch (Exception)
			{
				//Crear mensaje de error para modal
				Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Se ha producido un error. Intente más tarde!", type = true, path = "Sources" });
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Editar(Source mediaType)
		{
			try
			{
				//Verificamos model state
				if (!ModelState.IsValid)
				{
					return View(mediaType);
				}

				//Verificamos si existe
				var existe = await _repositorySource.ObtenerPorId(mediaType.id);

				if (existe is null)
				{
					//Crear mensaje de error para modal
					Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡La Fuente que intenta editar no existe!", type = true, path = "Sources" });
					return RedirectToAction("Index");
				}

				//Crear
				await _repositorySource.Editar(mediaType);

				return RedirectToAction("Index");
			}
			catch (Exception)
			{
				//Crear mensaje de error para modal
				Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Se ha producido un error. Intente más tarde!", type = true, path = "Sources" });
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
				//Verificamos si existe
				var existe = await _repositorySource.ObtenerPorId(id);

				if (existe is null)
				{
					//Crear mensaje de error para modal
					Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡La Fuente que intenta eliminar no existe!", type = true, path = "Sources" });
					return RedirectToAction("Index");
				}

				//Verificar si se encuentra en uso
				var borrarTipo = await _repositorySource.sePuedeBorrar(id);

				if (!borrarTipo)
					return Json(new { error = true, mensaje = "No se puede borrar porque el tipo de fuente se encuentra en uso" });

				//Borrar
				await _repositorySource.Borrar(id);

				return Json(new { error = false, mensaje = "Borrado con Éxito" });
			}
			catch (Exception)
			{
				return Json(new { error = true, mensaje = "Se ha producido un error. Intente nuevamente más tarde!" });
			}
		}


		/***************/
		/*  FUNCIONES  */
		/***************/

		[HttpGet]
		public async Task<IActionResult> VerificarExisteCategoria(string name)
		{
			try
			{
				var existeCategoria = await _repositorySource.Existe(name);

				if (existeCategoria)
					return Json($"El nombre {name} ya existe!");

				return Json(true);
			}
			catch (Exception)
			{
				return Json($"Se ha producido un error. Intente nuevamente más tarde!");
			}

		}
	}
}
