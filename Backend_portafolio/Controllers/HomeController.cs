using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;


namespace Backend_portafolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepositoryFormat _repositoriyFormat;
        private readonly IRepositoryPosts _repositoriyPosts;
        private readonly IUsersService _usersService;
        private readonly IRepositoryFormat _repositoryFormat;
        private readonly IRepositoryPosts _repositoryPosts;

        public HomeController(
            ILogger<HomeController> logger,
            IRepositoryFormat repositoriyFormat,
            IRepositoryPosts repositoryPosts,
            IUsersService usersService,
            IRepositoryFormat repositoryFormat,
            IRepositoryPosts repositoryPosts1
        )
        {
            _logger = logger;
            _repositoriyFormat = repositoriyFormat;
            _repositoriyPosts = repositoryPosts;
            _usersService = usersService;
            _repositoryFormat = repositoryFormat;
            _repositoryPosts = repositoryPosts1;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                HomeViewModel viewModel = new HomeViewModel();
                await Session.UpdateSession(HttpContext, _repositoriyFormat, userID);

                var model = new PostViewModel();

                viewModel.formatList = (await _repositoriyFormat.Obtener(userID)).ToList();
                viewModel.ultimosPosts = (await _repositoriyPosts.Obtener(userID)).ToList();
                viewModel.user_id = userID;

                return View(viewModel);
            }
            catch (Exception)
            {

                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel viewModel)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                //verificamos que el model state sea valido antes de continuar
                if (!ModelState.IsValid)
                {
                    await Session.UpdateSession(HttpContext, _repositoriyFormat, userID);

                    var usuarioID = _usersService.ObtenerUsuario();
                    var model = new PostViewModel();

                    viewModel.formatList = (await _repositoriyFormat.Obtener(usuarioID)).ToList();
                    viewModel.ultimosPosts = (await _repositoriyPosts.Obtener(usuarioID)).ToList();
                    viewModel.user_id = usuarioID;

                    return View(viewModel);
                }

                //Verificamos que el formato que nos mandan exista
                var Formato = await _repositoryFormat.ObtenerPorId(viewModel.format_id);

                if (Formato is null)
                {
                    //Crear mensaje de error para modal
                    Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Error en uno de los datos ingresados!", type = true, path = "Posts" });
                    return RedirectToAction("Index", "Home");
                }

                //Colocamos fecha actual
                viewModel.created_at = DateTime.Now;

                // Seteamos borrador
                viewModel.draft = true;

                await _repositoryPosts.Crear(viewModel);

                var successModal = new ModalViewModel { message = "¡Se ha guardado conn éxito!", type = true, path = "Home" };
                Session.SuccessSession(HttpContext, successModal);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
                Session.ErrorSession(HttpContext, errorModal);

                //Redirect a la página anterior
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
