using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;

namespace Backend_portafolio.Controllers
{
    public class SourcesController : Controller
    {
        private readonly ISourceService _sourceService;

        public SourcesController(
			ISourceService sourceService
		)
        {
            _sourceService = sourceService;
        }


        //****************************************************
        //*********************** INDEX **********************
        //****************************************************
        public async Task<IActionResult> Index()
        {
            try
            {
				var mediTypes = await _sourceService.GetAllSource();
				return View(mediTypes);
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
			var viewModel = _sourceService.GetSourceViewModel();
            return View(viewModel);
        }

		[HttpPost]
		public async Task<IActionResult> Crear(Source viewModel)
		{
			if (!ModelState.IsValid)
				return View(viewModel);

            try
            {
				await _sourceService.CreateSource(viewModel);
                return RedirectToAction("Index");
			}
            catch (Exception ex)
            {
				Session.CrearModalError(ex.Message, "Sources", HttpContext);
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
				var viewModel = await _sourceService.GetSourceById(id);
                return View(viewModel);
			}
			catch (Exception ex)
			{
				Session.CrearModalError(ex.Message, "source", HttpContext);
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Editar(Source viewModel)
		{
			if (!ModelState.IsValid)
				return View(viewModel);

			try
			{
				await _sourceService.EditSource(viewModel);
                return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				Session.CrearModalError(ex.Message, "Sources", HttpContext);
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
				await _sourceService.DeleteSource(id);
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
		public async Task<IActionResult> VerificarExisteCategoria(string name, int user_id)
		{
			try
			{
				await _sourceService.Existe(name);
				return Json(true);
			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}

		}
	}
}
