using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Backend_portafolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(
            IHomeService homeService
        )
        {
            _homeService = homeService;
        }

        //****************************************************
        //*********************** INDEX **********************
        //****************************************************
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await _homeService.GetHomeViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Home", HttpContext);
                return View();
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
