using AutoMapper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SqlServer.Server;
using System.Reflection;

namespace Backend_portafolio.Controllers
{
	public class PostsController : Controller
	{
		private readonly IUsersService _usersService;
		private readonly IRepositoryCategorias _repositoryCategorias;
		private readonly IRepositoryFormat _repositoryFormat;
		private readonly IRepositoryPosts _repositoryPosts;
		private readonly IMapper _mapper;

		public PostsController(
			IUsersService usersService,
			IRepositoryCategorias repositoryCategorias, 
			IRepositoryFormat repositoryFormat,
			IRepositoryPosts repositoryPosts,
			IMapper mapper)
        {
			_usersService = usersService;
			_repositoryCategorias = repositoryCategorias;
			_repositoryFormat = repositoryFormat;
			_repositoryPosts = repositoryPosts;
			_mapper = mapper;
		}

		[HttpGet]

		public async Task<IActionResult> Index(string format)
		{
			var posts = await _repositoryPosts.ObtenerPorFormato(format);
			var model = posts
				.GroupBy(p => p.formatName)
				.Select(p => new ListPostViewModel()
				{
					format = p.Key,
					posts = p.AsEnumerable(),

				}).ToList();

			//Formato para retornar al listado correspondiente
			ViewBag.Format = format;

			return View(model);
		}

        [HttpGet]
        public async Task <IActionResult> Crear(string format)
        {
			var usuarioID = _usersService.ObtenerUsuario();
	
			var model = new PostViewModel();

			model.user_id = usuarioID;
			model.categories = await ObtenerCategorias();
			model.formatName = format;

			var formats = await _repositoryFormat.Obtener();
            model.format_id = formats.Where(f => f.name == format).Select(f => f.id).FirstOrDefault();
            model.formatName = formats.Where(f => f.name == format).Select(f => f.name).FirstOrDefault();

            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> Crear(PostViewModel viewModel)
		{
			//verificamos que el model state sea valido antes de continuar
			if(!ModelState.IsValid)
			{
				viewModel.categories = await ObtenerCategorias();
				//viewModel.formats = await ObtenerCategorias();

                return View(viewModel);
			}

			//Verificamos que la categoria que nos mandan exista
			var categoria = await _repositoryCategorias.ObtenerPorId(viewModel.category_id);

			if(categoria is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			//Verificamos que el formato que nos mandan exista
			var Formato = await _repositoryFormat.ObtenerPorId(viewModel.format_id);

			if (Formato is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			//Colocamos fecha actual
			viewModel.created_at = DateTime.Today;

			await _repositoryPosts.Crear(viewModel);

            return RedirectToAction("Index", "Posts", new { format = viewModel.formatName });
        }

		[HttpGet]
		public async Task<IActionResult> Editar(int id, string format)
		{
			var model = await _repositoryPosts.ObtenerPorId(id);

			//Auto Mapper
			var modelView = _mapper.Map<PostViewModel>(model);

			modelView.user_id = _usersService.ObtenerUsuario();
			modelView.categories = await ObtenerCategorias();
            //modelView.formats = await ObtenerFormatos();

            var formats = await _repositoryFormat.Obtener();
            modelView.format_id = formats.Where(f => f.name == format).Select(f => f.id).FirstOrDefault();
			modelView.formatName = formats.Where(f => f.name == format).Select(f => f.name).FirstOrDefault();

            return View(modelView);
		}

		[HttpPost]
		public async Task<IActionResult>Editar(PostViewModel viewModel)
		{
			if(!ModelState.IsValid)
			{
				viewModel.categories = await ObtenerCategorias();
				viewModel.formats = await ObtenerCategorias();
				return View(viewModel);
			}

			//Verificamos que la categoria que nos mandan exista
			var categoria = await _repositoryCategorias.ObtenerPorId(viewModel.category_id);

			if (categoria is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			//Verificamos que el formato que nos mandan exista
			var Formato = await _repositoryFormat.ObtenerPorId(viewModel.format_id);

			if (Formato is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			await _repositoryPosts.Editar(viewModel);

            return RedirectToAction("Index", "Posts", new { format = viewModel.formatName });
        }

        [HttpPost]
		public async Task<IActionResult>Borrar(int id)
		{
			var post = await _repositoryPosts.ObtenerPorId(id);

			if(post is null)
				return View("NoEncontrado", "Home");

			await _repositoryPosts.Borrar(id);

			return RedirectToAction("Index");
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerCategorias()
		{
			var categories = await _repositoryCategorias.Obtener();
			return categories.Select(category => new SelectListItem(category.name, category.id.ToString()));
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerFormatos()
		{
			var formats = await _repositoryFormat.Obtener();
			return formats.Select(format => new SelectListItem(format.name, format.id.ToString()));
		}
	}
}
