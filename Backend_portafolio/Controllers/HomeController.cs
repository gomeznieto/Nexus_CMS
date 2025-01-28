using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Backend_portafolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IFormatService _formatService;

        public HomeController(
            IHomeService homeService,
            IFormatService formatService
        )
        {
            _homeService = homeService;
            _formatService = formatService;
        }

        //****************************************************
        //*********************** INDEX **********************
        //****************************************************
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var formatos = (await _formatService.GetAllFormat()).Count();

                if(formatos < 1)
                    throw new Exception(
                        "Antes de comenzar a crear \"Entradas\" es necesario crear un \"Formato\"" +
                        "</br><a class=\"btn btn-action mt-5\" href='/formats' onclick='handleLinkClickCloseModal(event, \"/formats\")'>Formatos</a>" +
                        "");

                var viewModel = await _homeService.GetHomeViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Home", HttpContext);
                var viewModel = await _homeService.GetHomeViewModel();
                return View(viewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _homeService.GetHomeViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                await _homeService.CreatePostFromHomeView(viewModel);
                Session.CrearModalSuccess("Post creado correctamente", "Home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Home", HttpContext);
                return RedirectToAction("Index", "Posts");
            }
        }


        //****************************************************
        //************************ 404 ***********************
        //****************************************************
        public IActionResult NoEncontrado()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
