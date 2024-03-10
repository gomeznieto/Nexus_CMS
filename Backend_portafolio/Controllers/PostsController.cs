using AutoMapper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text.Json;

namespace Backend_portafolio.Controllers
{
	public class PostsController : Controller
	{
		private readonly IUsersService _usersService;
		private readonly IRepositoryCategorias _repositoryCategorias;
		private readonly IRepositoryFormat _repositoryFormat;
		private readonly IRepositoryPosts _repositoryPosts;
        private readonly IRepositoryMedia _repositoryMedia;
        private readonly IRepositoryMediatype _repositoryMediatype;
        private readonly IMapper _mapper;

		public PostsController(
			IUsersService usersService,
			IRepositoryCategorias repositoryCategorias, 
			IRepositoryFormat repositoryFormat,
			IRepositoryPosts repositoryPosts,
			IRepositoryMedia repositoryMedia,
			IRepositoryMediatype repositoryMediatype,
			IMapper mapper)
        {
			_repositoryCategorias = repositoryCategorias;
			_repositoryFormat = repositoryFormat;
			_repositoryPosts = repositoryPosts;
            _repositoryMedia = repositoryMedia;
            _repositoryMediatype = repositoryMediatype;
            _usersService = usersService;
            _mapper = mapper;
		}


        /* ---------------- */
        /*      INDEX      */
        /*    =========     */
        /* ---------------- */
        [HttpGet]

		public async Task<IActionResult> Index(string format)
		{
			var posts = await _repositoryPosts.ObtenerPorFormato(format);
			var model = posts
				.GroupBy(p => p.format)
				.Select(p => new ListPostViewModel()
				{
					format = p.Key,
					posts = p.AsEnumerable(),

				}).ToList();

			//Formato para retornar al listado correspondiente
			ViewBag.Format = format;

			return View(model);
		}


        /* ---------------- */
        /*      CREAR      */
        /*    =========     */
        /* ---------------- */
        [HttpGet]
        public async Task <IActionResult> Crear(string format)
        {
			var usuarioID = _usersService.ObtenerUsuario();
	
			var model = new PostViewModel();

			model.user_id = usuarioID;
			model.format = format;

			//Obtenemos el formato de la entrada creada
			var formats = await _repositoryFormat.Obtener();
            model.format_id = formats.Where(f => f.name == format).Select(f => f.id).FirstOrDefault();
            model.format = formats.Where(f => f.name == format).Select(f => f.name).FirstOrDefault();

			//Obtenemos Categorias Select List
			model.categories = await ObtenerCategorias();

			//Obtenemos Media Types Select List
			model.mediaTypes = await ObtenerMediaTypes();

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

			//Subir Media
			if (!viewModel.mediaListString.IsNullOrEmpty())
			{
				//Deserializamos string de media
                List<MediaForm> mediaForms = JsonSerializer.Deserialize<List<MediaForm>>(viewModel.mediaListString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                //Mappeamos de MediaForm a Media
                List<Media> medias = _mapper.Map<List<Media>>(mediaForms);

				//Agregamos el numero de post creado a cada Media
                foreach (var media in medias)
				{
					media.post_id = viewModel.id;
				}

				//Subimos MeiaLinks
				await _repositoryMedia.Crear(medias);
			}

            return RedirectToAction("Index", "Posts", new { format = viewModel.format });
        }


        /* ---------------- */
        /*      EDITAR      */
        /*    =========     */
        /* ---------------- */
        [HttpGet]
		public async Task<IActionResult> Editar(int id, string format)
		{
			var model = await _repositoryPosts.ObtenerPorId(id);

			//Mapeamos de Post a PostViewModel
			var modelView = _mapper.Map<PostViewModel>(model);

			modelView.user_id = _usersService.ObtenerUsuario();
			modelView.categories = await ObtenerCategorias();
			modelView.mediaTypes = await ObtenerMediaTypes();
			modelView.mediaList = await _repositoryMedia.ObtenerPorPost(modelView.id);

            var formats = await _repositoryFormat.Obtener();
            modelView.format_id = formats.Where(f => f.name == format).Select(f => f.id).FirstOrDefault();
			modelView.format = formats.Where(f => f.name == format).Select(f => f.name).FirstOrDefault();

            return View(modelView);
		}

		[HttpPost]
		public async Task<IActionResult>Editar(PostViewModel viewModel)
		{

            /***** POST *****/

            if (!ModelState.IsValid)
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


			/***** METAS *****/

            if (!viewModel.mediaListString.IsNullOrEmpty())
            {
                //Deserializamos string de media
                List<MediaForm> mediaForms = JsonSerializer.Deserialize<List<MediaForm>>(viewModel.mediaListString);
				List<Media> medias = new List<Media>();

				//Verificamos si entre los datos tenemos que actualizar algunos
				foreach (var mediaForm in mediaForms)
				{
                    Media aux = _mapper.Map<Media>(mediaForm);
                    aux.post_id = viewModel.id;

                    if (aux?.id is 0)
					{
						//NUEVO
						medias.Add(aux);
                    }
					else if(aux?.id is not 0 && aux?.url is not null )
					{
                        //ACTUALIZAMOS
                        await _repositoryMedia.Editar(aux);
					} 
					else
					{
                        //ELIMINAMOS
                        await _repositoryMedia.Borrar(aux.id);
                    }
                }

				//Subimos MeiaLinks
				await _repositoryMedia.Crear(medias);
			}


            /***** LINKS *****/

			if(!viewModel.linkListString.IsNullOrEmpty())
			{

			}


            return RedirectToAction("Index", "Posts", new { format = viewModel.format });
        }


        /* ---------------- */
        /*      BORRAR      */
        /*    =========     */
        /* ---------------- */
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

        private async Task<IEnumerable<SelectListItem>> ObtenerMediaTypes()
        {
            var mediaTypes = await _repositoryMediatype.Obtener();
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
		}


        /*
		 *  API
		 *  ===
		 */

        //API - TODO: HEADERS CON TOKEN

        [HttpGet]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> ObtenerJSON()
        {
            var posts = await _repositoryPosts.Obtener();
            return Json(posts);
        }
    }
}
