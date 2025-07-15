using Backend_portafolio.Entities;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using AspNetCoreGeneratedDocument;

namespace Backend_portafolio.Controllers
{
    public class MediatypeController : Controller
    {
        private readonly IMediaTypeService _mediaTypeService;

        public MediatypeController(
            IMediaTypeService mediaTypeService
        )
        {
            _mediaTypeService = mediaTypeService;
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
        public IActionResult Crear()
        {
            var viewModel = _mediaTypeService.GetMediaTypeViewModel();
            viewModel.MediaTypeDefaults = _mediaTypeService.GetMediaTypeDefault().ToList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(MediaTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                await _mediaTypeService.CreateMediaType(viewModel);
                Session.CrearModalSuccess("El MediaType ha sido creado exitosamente", "Index", HttpContext);
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
        public async Task<IActionResult> Editar(MediaTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                await _mediaTypeService.EditMediaType(viewModel);
                Session.CrearModalSuccess("El MediaType ha sido modificada exitosamente", "Index", HttpContext);
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
        public async Task<IActionResult> Borrar(int id)
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
        public async Task<IActionResult> VerificarExisteMediaType(string name)
        {
            try
            {
                await _mediaTypeService.ExisteMediaType(name);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
