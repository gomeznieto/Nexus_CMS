using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Controllers
{
	public class PostsController : Controller
	{
		private readonly IRepositoryCategorias _repositoryCategorias;
		private readonly IRepositoryFormat _repositoryFormat;

		public PostsController(IRepositoryCategorias repositoryCategorias, IRepositoryFormat repositoryFormat)
        {
			_repositoryCategorias = repositoryCategorias;
			_repositoryFormat = repositoryFormat;
		}

        [HttpGet]
        public async Task <IActionResult> Crear()
        {
			var usuarioID = 1; //TODO: CAMBIAR EN SERVICIOS
			var formats = await _repositoryFormat.Obtener();
			var categories = await _repositoryCategorias.Obtener();

			var model = new PostViewModel();
			model.user_id = usuarioID;
			model.categories = categories.Select(category => new SelectListItem(category.name, category.id.ToString()) );
			model.formats = formats.Select(format => new SelectListItem(format.name, format.id.ToString()) );

			return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> Crear(PostViewModel viewModel)
		{

			return RedirectToAction("Index");
		}
    }
}
