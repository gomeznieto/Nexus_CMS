using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Backend_portafolio.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly IRepositoryFormat _repositoriyFormat;
        private readonly IRepositoryPosts _repositoriyPosts;

        public HomeController(ILogger<HomeController> logger, IRepositoryFormat repositoriyFormat, IRepositoryPosts repositoryPosts)
		{
			_logger = logger;
            _repositoriyFormat = repositoriyFormat;
            _repositoriyPosts = repositoryPosts;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
		{
			try
			{
				HomeViewModel viewModel = new HomeViewModel();
				await Session.UpdateSession(HttpContext, _repositoriyFormat);
                viewModel.formatList = (await _repositoriyFormat.Obtener()).ToList();
				viewModel.ultimosPosts = (await _repositoriyPosts.Obtener()).ToList();

                return View(viewModel);
			}
			catch (Exception)
			{

				return View();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Index(Categoria viewModel)
		{
			return View();
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
