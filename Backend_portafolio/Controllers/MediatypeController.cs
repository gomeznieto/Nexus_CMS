using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class MediatypeController : Controller
    {
        private readonly IRepositoryMediatype _repositoryMediatype;
		private readonly IUsersService _usersService;

		public MediatypeController(IRepositoryMediatype repositoryMediatype, IUsersService usersService)
        {
            _repositoryMediatype = repositoryMediatype;
			_usersService = usersService;
		}


        /***************/
        /*    INDEX    */
        /***************/
        public async Task<IActionResult> Index()
        {
            try
            {
				var userID = _usersService.ObtenerUsuario();

				var mediTypes = await _repositoryMediatype.Obtener(userID);

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
			var userID = _usersService.ObtenerUsuario();

			var viewModel = new MediaType();
			viewModel.user_id = userID;

			return View(viewModel);
        }

		[HttpPost]
		public async Task<IActionResult> Crear(MediaType viewModel)
		{
            try
            {
				var userID = _usersService.ObtenerUsuario();

				//Validar Model
				if (!ModelState.IsValid || viewModel.user_id != userID)
				{
					return View(viewModel);
				}

				//Validar que exista
				var existe = await _repositoryMediatype.Existe(viewModel.name, userID);

				if (existe)
				{
					ModelState.AddModelError(null, "El media type ya existe");
					return View(viewModel);
				}

				await _repositoryMediatype.Crear(viewModel);

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
		public async Task<IActionResult> Editar(MediaType viewModel)
		{
			try
			{
				var userID = _usersService.ObtenerUsuario();
				//Verificamos model state
				if (!ModelState.IsValid || userID != viewModel.user_id)
				{
					return View(viewModel);
				}

				//Verificamos si existe
				var existe = await _repositoryMediatype.ObtenerPorId(viewModel.id);

				if (existe is null)
				{
					return RedirectToAction("NoExiste", "Home");
				}

				//Crear
				await _repositoryMediatype.Editar(viewModel);

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
				var userID = _usersService.ObtenerUsuario();

				var existeCategoria = await _repositoryMediatype.Existe(name, userID);

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
