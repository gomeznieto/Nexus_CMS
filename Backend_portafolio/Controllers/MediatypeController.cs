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
            var mediTypes = await _repositoryMediatype.Obtener();

            return View(mediTypes);
        }

        /***************/
        /*   CREAR     */
        /***************/

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Crear(MediaType mediaType)
		{
            return RedirectToAction("Index");
		}

        /***************/
        /*   EDITAR    */
        /***************/

        [HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
            var mediaType = await _repositoryMediatype.ObtenerPorId(id);

            if(mediaType == null)
            {
                return RedirectToAction("NoExiste", "Home");
            }

            return View(mediaType);
		}

		[HttpPost]
		public async Task<IActionResult> Editar(MediaType mediaType)
		{
            //Verificamos model state
            if(!ModelState.IsValid)
            {
                return View(mediaType);
            }
            
            //Verificamos si existe
            var existe = await _repositoryMediatype.ObtenerPorId(mediaType.id);

            if(existe is null)
            {
                return RedirectToAction("NoExiste", "Home");
            }

            //Crear
            await _repositoryMediatype.Editar(mediaType);

			return RedirectToAction("Index");
		}

		/***************/
		/*   BORRAR    */
		/***************/

        public async Task<IActionResult>Borrar(int id)
        {
            // Verificar si hay links con este tipo de media antes de borrar
            //Si hay, no borrar. Emitir cartel que indique que primero debe borrar los links

			//Verificamos si existe
			var existe = await _repositoryMediatype.ObtenerPorId(id);

			if (existe is null)
			{
				return RedirectToAction("NoExiste", "Home");
			}

            await _repositoryMediatype.Borrar(id);

            return RedirectToAction("NoExiste", "Home");
		}

		/***************/
		/*  FUNCIONES  */
		/***************/

		[HttpGet]
		public async Task<IActionResult> VerificarExisteCategoria(string name)
		{
			var existeCategoria = await _repositoryMediatype.Existe(name);

			if (existeCategoria)
				return Json($"El nombre {name} ya existe!");

			return Json(true);

		}
	}
}
