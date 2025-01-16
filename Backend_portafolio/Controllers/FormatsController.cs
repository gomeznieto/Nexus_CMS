using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;
using Backend_portafolio.Helper;
using Microsoft.AspNetCore.Http;

namespace Backend_portafolio.Controllers
{
    public class FormatsController : Controller
    {
        private readonly IRepositoryFormat _repositoryFormat;
        private readonly IFormatService _formatService;
        private readonly IUsersService _usersService;


        public FormatsController(
            IRepositoryFormat repositoryFormat,
            IFormatService formatService,
            IUsersService usersService)
        {
            _repositoryFormat = repositoryFormat;
            _formatService = formatService;
            _usersService = usersService;
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
                await _formatService.Existe(format);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        //     [HttpGet]
        //     [Route("api/[controller]/get")]
        //     public async Task<IActionResult> apiJSON()
        //     {
        //try
        //{
        //             var userID = _usersService.ObtenerUsuario();

        //             var formatos = await _repositoryFormat.Obtener(userID);
        //	return Json(formatos);
        //}
        //catch (Exception)
        //{
        //	return Json(new { error = true, mensaje = "Se ha producido un error."});
        //}
        //     }
    }
}
