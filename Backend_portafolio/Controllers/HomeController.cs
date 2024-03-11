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

        public HomeController(ILogger<HomeController> logger, IRepositoryFormat repositoriyFormat)
		{
			_logger = logger;
            _repositoriyFormat = repositoriyFormat;
        }

		public async Task<IActionResult> Index()
		{
			try
			{
				await Helper.Session.UpdateSession(HttpContext, _repositoriyFormat);

				return View();
			}
			catch (Exception)
			{

				return View();
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
