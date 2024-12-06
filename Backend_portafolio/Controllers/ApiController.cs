using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;

namespace Backend_portafolio.Controllers
{
	
	public class ApiController : ControllerBase
	{
        private readonly IRepositoryCategorias _repositoryCateogorias;
		private readonly IRepositoryPosts _repositoryPosts;
		private readonly IRepositoryLink _repositoryLink;
		private readonly IRepositoryMedia _repositoryMedia;
		private readonly IRepositoryFormat _repositoryFormat;
        private readonly IUsersService _usersService;

        public ApiController(
			IRepositoryCategorias repositoryCateogorias,
			IRepositoryPosts repositoryPost,
			IRepositoryLink repositoryLink,
			IRepositoryMedia repositoryMedia,
			IRepositoryFormat repositoryFormat,
            IUsersService usersService
			)
        {
            _repositoryCateogorias = repositoryCateogorias;
			_repositoryPosts = repositoryPost;
			_repositoryLink = repositoryLink;
			_repositoryMedia = repositoryMedia;
			_repositoryFormat = repositoryFormat;
           _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Categoria()
        {
            //TODO: Validar Token


            var categorias = await _repositoryCateogorias.Obtener();

            return Ok(categorias);
        }

		public async Task<IActionResult> Formato()
		{
			//TODO: Validar Token

			var userID = _usersService.ObtenerUsuario();
			var formatos = await _repositoryFormat.Obtener(userID);

			return Ok(formatos);
		}

		[HttpGet]
		public async Task<IActionResult> Usuario()
		{
			//TODO: Validar Token


			var categorias = await _repositoryCateogorias.Obtener();

			return Ok(categorias);
		}

		[HttpGet]
		public async Task<IActionResult> Entrada()
		{
            //TODO: Validar Token

            var usuarioID = _usersService.ObtenerUsuario();

            var posts = await _repositoryPosts.Obtener(usuarioID);

			if(posts is null)
				return NotFound();

			List<ApiPostModel> postsApiModels = new List<ApiPostModel>();

			foreach ( var post in posts) { 
                ApiPostModel aux = new ApiPostModel();
				aux.id = post.id;
				aux.title = post.title;
				aux.description = post.description;
				aux.cover = post.cover;
				aux.Format = post.format;

				var imagenes = await _repositoryMedia.ObtenerPorPost(aux.id);
				aux.images = imagenes.Select(i => new ApiMediaModel { url = i.url, mediaType = i.name}).ToList();

				var links = await _repositoryLink.ObtenerPorPost(aux.id);
				aux.links = links.Select(l => new ApiLinkModel { url = l.url, source = l.name, icon = l.icon }).ToList();

				var categories = await _repositoryCateogorias.ObtenerPorPost(aux.id);
				aux.categories = categories.Select(l => new ApiCategoryModel { id = l.id, category = l.name }).ToList();

				postsApiModels.Add(aux);
			}

			return Ok(postsApiModels);
		}

		[HttpGet("{controller}/{action}/{id}")]
		public async Task<IActionResult> Entrada(int id)
		{
			//TODO: Validar Token

			var post = await _repositoryPosts.ObtenerPorId(id);

			if (post is null)
				return NotFound();	

			ApiPostModel aux = new ApiPostModel();

			aux.id = post.id;
			aux.title = post.title;
			aux.description = post.description;
			aux.cover = post.cover;
			aux.Format = post.format;

			var imagenes = await _repositoryMedia.ObtenerPorPost(aux.id);
			aux.images = imagenes.Select(i => new ApiMediaModel { url = i.url, mediaType = i.name }).ToList();

			var links = await _repositoryLink.ObtenerPorPost(aux.id);
			aux.links = links.Select(l => new ApiLinkModel { url = l.url, source = l.name, icon = l.icon }).ToList();

            var categories = await _repositoryLink.ObtenerPorPost(aux.id);
            aux.categories = categories.Select(l => new ApiCategoryModel { id = l.id, category = l.name }).ToList();

            return Ok(aux);
		}
	}
}
