using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class FormatsController : Controller
    {
        private readonly IRepositoryFormat _repositoryFormat;

        public FormatsController(IRepositoryFormat repositoryFormat)
        {
            _repositoryFormat = repositoryFormat;
        }

        public async Task<IActionResult> Index()
        {
            var formats = await _repositoryFormat.Obtener();

            return View(formats);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Format model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            await _repositoryFormat.Crear(model);

            //Actualizar Session de Formatos para barra de navegacion
            await Helper.Session.UpdateSession(HttpContext, _repositoryFormat);

            return RedirectToAction("Index");
        }

        [HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
			var format = await _repositoryFormat.ObtenerPorId(id);
            
            if(format is null)
				return RedirectToAction("NoEncontrado", "Home");

			return View(format);
		}

		[HttpPost]
        public async Task<IActionResult> Editar(Format model)
        {
			if (!ModelState.IsValid)
			{
				return View(model);
			}

            var existe = await _repositoryFormat.ObtenerPorId(model.id);

            if(existe is null)
				return RedirectToAction("NoEncontrado", "Home");

			await _repositoryFormat.Editar(model);

			return RedirectToAction("Index");
		}

		/***************/
		/*   BORRAR    */
		/***************/
		[HttpPost]
        public async Task<IActionResult>Borrar(int id)
		{

			var existe = await _repositoryFormat.ObtenerPorId(id);

			if (existe is null)
				return RedirectToAction("NoEncontrado", "Home");

            var borrar = await _repositoryFormat.sePuedeBorrar(id);

            if (!borrar)
				return Json(new { error = true, mensaje = "No se puede borrar porque el formato se encuentra en uso" });

			await _repositoryFormat.Borrar(id);

            //Actualizar Session de Formatos para barra de navegacion
            await Helper.Session.UpdateSession(HttpContext, _repositoryFormat);

			return Json(new { error = false, mensaje = "Borrado con Éxito" });
		}


		/***************/
		/*  FUNCIONES  */
		/***************/

		[HttpGet]
		public async Task<IActionResult> VerificarExisteFormato(string name)
		{
			var existeCategoria = await _repositoryFormat.Existe(name);

			if (existeCategoria)
				return Json($"El nombre {name} ya existe!");

			return Json(true);

		}

        [HttpGet]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> ObtenerJSON()
        {
            var formatos = await _repositoryFormat.Obtener();
            return Json(formatos);
        }
    }
}
