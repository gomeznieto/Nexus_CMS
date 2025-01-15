using AutoMapper;
using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;

namespace Backend_portafolio.Controllers
{
    public class PostsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IPostService _postService;
        private readonly ICategoriaService _categoriaService;
        private readonly IFormatService _formatService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IMediaService _mediaService;
        private readonly ISourceService _sourceService;
        private readonly ILinkService _linkService;
        private readonly IMapper _mapper;

        public PostsController(
            IUsersService usersService,
            IPostService postService,
            ICategoriaService categoriaService,
            IFormatService formatService,
            ISourceService sourceService,
            ILinkService linkService,
            IMediaTypeService mediaTypeService,
            IMediaService mediaService,
            IMapper mapper)
        {
            _postService = postService;
            _categoriaService = categoriaService;
            _formatService = formatService;
            _mediaTypeService = mediaTypeService;
            _mediaService = mediaService;
            _sourceService = sourceService;
            _linkService = linkService;
            _usersService = usersService;
            _mapper = mapper;
        }


        //****************************************************
        //******************** LISTA POST ********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Index(string format, int page = 1)
        {
            try
            {
                // Obtener todos los posts
                IEnumerable<Post> posts = await _postService.GetAllPosts(format, page);

                //Salida de la vista
                ViewBag.Format = format;
                ViewBag.Cantidad = await _postService.GetCountPostByFormat(format);
                ViewBag.Message = "No hay entradas para mostrar";

                return View(posts);
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                Session.CrearModalError("Ha surgido un error. ¡Intente más tarde!", "Home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //******************* BUSCAR POST ********************
        //****************************************************

        // Buscado de posts
        [HttpPost]
        public async Task<IActionResult> Index(string format, string buscar, int page = 1)
        {
            try
            {
                // Usuario registrado
                var posts = await _postService.SearchAllPost(format, buscar, page);

                //Formato para retornar al listado correspondiente
                ViewBag.Format = format;
                ViewBag.Cantidad = posts.Count();
                ViewBag.Message = $"Sin resultados para \"{buscar}\".";

                return View(posts);
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Se ha producido un error. Intente más tarde!", type = true, path = "Posts" });
                return RedirectToAction("Index", "Posts", new { format = format });
            }
        }


        //****************************************************
        //******************** CREAR POST ********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Crear(string format)
        {
            try
            {
                var model = await _postService.GetPostViewModel(format);
                return View(model);
            }
            catch (Exception ex)
            {
                //Crear mensaje de error para modal
                Session.ErrorSession(HttpContext, new ModalViewModel { message = ex.Message, type = true, path = "Posts" });
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear(PostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _postService.PrepareViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                await _postService.CreatePost(viewModel);
                return RedirectToAction("Index", "Posts", new { format = viewModel.format });
            }
            catch (Exception ex)
            {
                //Crear mensaje de error para modal
                Session.CrearModalError(ex.Message, "Home", HttpContext);

                //Redirect a la página anterior
                return RedirectToAction("Index", "Posts", new { format = viewModel.format });

            }
        }


        //****************************************************
        //******************* EDITAR POST ********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string format)
        {
            try
            {
                var modelView = await _postService.PrepareEditPostViewModel(id);
                return View(modelView);
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
                Session.ErrorSession(HttpContext, errorModal);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost] //TODO: Refactorizar con Service
        public async Task<IActionResult> Editar(PostViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _postService.PrepareViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                await _postService.EditPost(viewModel);
                return RedirectToAction("Index", "Posts", new { format = viewModel.format });
            }
            catch (Exception ex)
            {

                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = ex.Message, type = true, path = "Posts" };
                Session.ErrorSession(HttpContext, errorModal);

                //Redirige a la página anterior
                return RedirectToAction("Index", "Posts", new { format = viewModel.format });
            }

        }

        //****************************************************
        //****************** EDITAR BORRADOR *****************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> EditarBorrador(int id, bool draft)
        {
            try
            {
                var mensaje = draft ? $"La entrada se ha colocado como borrador exitosamente" : $"La entrada se ha publicado exitosamente";

                await _postService.EditarBorrador(id, draft);

                return Json(new { error = false, mensaje = mensaje });
            }
            catch (Exception ex)
            {

                return Json(new { error = true, mensaje = ex.Message });
            }

        }


        //****************************************************
        //******************** BORRAR POST *******************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                await _postService.DeletePost(id);
                return Json(new { error = false, mensaje = "¡La entrada ha sido borrada correctamente!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });

            }
        }

    }
}
