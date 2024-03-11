using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class MediatypeController : Controller
    {
        private readonly IRepositoryMediatype _repositoryMediatype;

        public MediatypeController(IRepositoryMediatype repositoryMediatype)
        {
            _repositoryMediatype = repositoryMediatype;
        }


        /***************/
        /*    INDEX    */
        /***************/
        public async Task<IActionResult> Index()
        {
            try
            {
				var mediTypes = await _repositoryMediatype.Obtener();

				return View(mediTypes);
			}
            catch (Exception)
            {

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
		public async Task<IActionResult> Crear(MediaType mediaType)
		{
            try
            {
				//Validar Model
				if (!ModelState.IsValid)
				{
					return View(mediaType);
				}

				//Validar que exista
				var existe = await _repositoryMediatype.Existe(mediaType.name);

				if (existe)
				{
					ModelState.AddModelError(null, "El media type ya existe");
					return View(mediaType);
				}

				await _repositoryMediatype.Crear(mediaType);

				return RedirectToAction("Index");
			}
            catch (Exception)
            {

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
				var mediaType = await _repositoryMediatype.ObtenerPorId(id);

				if (mediaType == null)
				{
					return RedirectToAction("NoExiste", "Home");
				}

				return View(mediaType);
			}
			catch (Exception)
			{
				return RedirectToAction("NoExiste", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Editar(MediaType mediaType)
		{
			try
			{
				//Verificamos model state
				if (!ModelState.IsValid)
				{
					return View(mediaType);
				}

				//Verificamos si existe
				var existe = await _repositoryMediatype.ObtenerPorId(mediaType.id);

				if (existe is null)
				{
					return RedirectToAction("NoExiste", "Home");
				}

				//Crear
				await _repositoryMediatype.Editar(mediaType);

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
				//Verificamos si existe
				var existe = await _repositoryMediatype.ObtenerPorId(id);

				if (existe is null)
					return RedirectToAction("NoExiste", "Home");

				//Verificar si se encuentra en uso
				var borrarTipo = await _repositoryMediatype.sePuedeBorrar(id);

				if (!borrarTipo)
					return Json(new { error = true, mensaje = "No se puede borrar porque el tipo de media se encuentra en uso" });

				//Borrar
				await _repositoryMediatype.Borrar(id);

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
				var existeCategoria = await _repositoryMediatype.Existe(name);

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
