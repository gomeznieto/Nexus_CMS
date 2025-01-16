using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;
using Backend_portafolio.Helper;

namespace Backend_portafolio.Controllers
{
    public class MediatypeController : Controller
    {
        private readonly IRepositoryMediatype _repositoryMediatype;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IUsersService _usersService;

		public MediatypeController(
			IRepositoryMediatype repositoryMediatype, 
			IMediaTypeService mediaTypeService,
			IUsersService usersService
			)
        {
            _repositoryMediatype = repositoryMediatype;
            _mediaTypeService = mediaTypeService;
            _usersService = usersService;
		}


        //****************************************************
        //*********************** INDEX **********************
        //****************************************************
        public async Task<IActionResult> Index()
        {
            try
            {
				var viewModel = await _mediaTypeService.GetAllMediaType();
                return View(viewModel);
			}
            catch (Exception ex)
            {
				Session.CrearModalError(ex.Message, "Home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        [HttpGet]
        public  IActionResult Crear()
        {
			var viewModel = _mediaTypeService.GetMediaTypeViewModel();
            return View(viewModel);
        }

		[HttpPost]
		public async Task<IActionResult> Crear(MediaType viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

            try
            {
				await _mediaTypeService.CreateMediaType(viewModel);
                return RedirectToAction("Index");
			}
            catch (Exception ex)
            {
				Session.CrearModalError(ex.Message, "Index", HttpContext);
                return RedirectToAction("Index");
			}
		}


        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        [HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
			try
			{
				var viewModel = await _mediaTypeService.GetMediaTypeById(id);
				return View(viewModel);
			}
			catch (Exception ex)
			{
				Session.CrearModalError(ex.Message, "Index", HttpContext);
                return RedirectToAction("NoExiste", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Editar(MediaType viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			try
			{
				await _mediaTypeService.EditMediaType(viewModel);
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
                Session.CrearModalError(ex.Message, "Index", HttpContext);
                return RedirectToAction("Index");
			}
		}


        //****************************************************
        //********************** DELETE **********************
        //****************************************************
        [HttpPost]
		public async Task<IActionResult>Borrar(int id)
        {
			try
			{
				await _mediaTypeService.DeleteMediaType(id);
                return Json(new { error = false, mensaje = "Borrado con Éxito" });
			}
			catch (Exception ex)
			{
                return Json(new { error = true, mensaje = ex.Message });
			}
		}


        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************

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
