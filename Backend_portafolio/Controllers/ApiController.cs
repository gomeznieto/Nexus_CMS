using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
	public class ApiController : Controller
	{
        private readonly IRepositoryCategorias _repositoryCateogorias;
		private readonly IRepositoryPosts _repositoryPosts;
		private readonly IRepositoryLink _repositoryLink;
		private readonly IRepositoryMedia _repositoryMedia;
		private readonly IRepositoryFormat _repositoryFormat;

		public ApiController(
			IRepositoryCategorias repositoryCateogorias,
			IRepositoryPosts repositoryPost,
			IRepositoryLink repositoryLink,
			IRepositoryMedia repositoryMedia,
			IRepositoryFormat repositoryFormat
			)
        {
            _repositoryCateogorias = repositoryCateogorias;
			_repositoryPosts = repositoryPost;
			_repositoryLink = repositoryLink;
			_repositoryMedia = repositoryMedia;
			_repositoryFormat = repositoryFormat;
		}

        [HttpGet]
        public async Task<IActionResult> Categoria()
        {
            //TODO: Validar Token


            var categorias = await _repositoryCateogorias.Obtener();

            return Json(categorias);
        }

		public async Task<IActionResult> Formato()
		{
			//TODO: Validar Token


			var formatos = await _repositoryFormat.Obtener();

			return Json(formatos);
		}

		[HttpGet]
		public async Task<IActionResult> Usuario()
		{
			//TODO: Validar Token


			var categorias = await _repositoryCateogorias.Obtener();

			return Json(categorias);
		}

		[HttpGet]
		public async Task<IActionResult> Entrada()
		{
			//TODO: Validar Token

			var posts = await _repositoryPosts.Obtener();

			List<PostApiModel> postsApiModels = new List<PostApiModel>();

			foreach ( var post in posts )
			{
				PostApiModel aux = new PostApiModel();
				aux.id = post.id;
				aux.title = post.title;
				aux.description = post.description;
				aux.cover = post.cover;
				aux.Format = post.format;
				aux.category = post.category;

				var imagenes = await _repositoryMedia.ObtenerPorPost(aux.id);
				aux.images = imagenes.Select(i => new ApiMediaModel { url = i.url, mediaType = i.name}).ToList();

				var links = await _repositoryLink.ObtenerPorPost(aux.id);
				aux.links = links.Select(l => new ApiLinkModel { url = l.url, source = l.name, icon = l.icon }).ToList();

				postsApiModels.Add(aux);
			}

			return Json(postsApiModels);
		}
	}
}
