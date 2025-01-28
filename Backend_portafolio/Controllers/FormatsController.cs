using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;
using Backend_portafolio.Helper;

namespace Backend_portafolio.Controllers
{
    public class FormatsController : Controller
    {
        private readonly IFormatService _formatService;


        public FormatsController(
            IFormatService formatService
        )
        {
            _formatService = formatService;
        }


        //****************************************************
        //*********************** INDEX **********************
        //****************************************************

        public async Task<IActionResult> Index()
        {
            try
            {
                var format = await _formatService.GetAllFormat();
                return View(format);
            }
            catch (Exception)
            {
                Session.CrearModalError("Se ha producido un error", "Error", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        [HttpGet]
        public IActionResult Crear()
        {
            var viewModel = _formatService.GetFormatViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Format viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                await _formatService.CreateFormat(viewModel);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session.CrearModalError("Se ha producido un error", "Error", HttpContext);
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
                var format = await _formatService.GetFormatById(id);
                return View(format);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Error", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Format model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _formatService.EditFormat(model);
                Session.CrearModalSuccess("El formato se ha editado correctamente", "Formats", HttpContext);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Error", HttpContext);
                return RedirectToAction("Index");
            }
        }

        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                await _formatService.DeleteFormat(id);
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
        public async Task<IActionResult> VerificarExisteFormato(string format)
        {
            try
            {
                var existeCategoria = await _formatService.Existe(format);

                if (existeCategoria)
                    throw new Exception($"El nombre {format} ya existe!");

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
    }
}
