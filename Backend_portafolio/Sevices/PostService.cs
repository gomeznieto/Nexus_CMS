using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Backend_portafolio.Entities;
using Microsoft.SqlServer.Server;
using Backend_portafolio.Helper;
using Microsoft.IdentityModel.Tokens;
using Backend_portafolio.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace Backend_portafolio.Sevices
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPosts(string format, int pagina, HttpContext httpContext);
        Task<PostViewModel> GetPostViewModel(string format, PostViewModel v = null);
        Task<List<Post>> SearchAllPost(string format, string buscar, int page, HttpContext httpContext);
    }

    public class PostService : IPostService
    {
        private readonly IUsersService _usersService;
        private readonly IRepositoryCategorias _repositoryCategorias;
        private readonly IRepositoryFormat _repositoryFormat;
        private readonly IRepositoryPosts _repositoryPosts;
        private readonly IRepositoryMedia _repositoryMedia;
        private readonly IRepositoryMediatype _repositoryMediatype;
        private readonly IRepositorySource _repositorySource;
        private readonly IRepositoryLink _repositoryLink;
        private readonly ICategociaService _categoriaService;
        private readonly IFormatService _formatService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;

        public PostService(
            IUsersService usersService,
            IRepositoryCategorias repositoryCategorias,
            IRepositoryFormat repositoryFormat,
            IRepositoryPosts repositoryPosts,
            IRepositoryMedia repositoryMedia,
            IRepositoryMediatype repositoryMediatype,
            IRepositorySource repositorySource,
            IRepositoryLink repositoryLink,
            ICategociaService categoriaService,
            IFormatService formatService,
            ISourceService sourceService,
            IMediaTypeService mediaTypeService,
            IMapper mapper)
        {
            _repositoryCategorias = repositoryCategorias;
            _repositoryFormat = repositoryFormat;
            _repositoryPosts = repositoryPosts;
            _repositoryMedia = repositoryMedia;
            _repositoryMediatype = repositoryMediatype;
            _repositorySource = repositorySource;
            _repositoryLink = repositoryLink;
            _usersService = usersService;
            _categoriaService = categoriaService;
            _formatService = formatService;
            _mediaTypeService = mediaTypeService;
            _sourceService = sourceService;
            _mapper = mapper;
        }

        // Obtener
        public async Task<List<Post>> GetAllPosts(string format, int pagina, HttpContext httpContext)
        {

            // Usuario registrado
            var usuarioID = _usersService.ObtenerUsuario();

            //crear session de cantidad de post en caso de no haber sido ya creada
            if (Session.GetCantidadPostsSession(httpContext) == -1)
            {
                Session.CantidadPostsSession(httpContext, 10);
            }

            //Obtener cantidades para generar paginación
            var cantidadPorPagina = Session.GetCantidadPostsSession(httpContext);
            IEnumerable<Post> posts = await _repositoryPosts.ObtenerPorFormato(format, cantidadPorPagina, pagina, usuarioID);

            //Obtenemos categorias para mostrar en lista
            foreach (var post in posts)
            {
                post.categoryList = await _repositoryCategorias.ObtenerCategoriaPostPorId(post.id);
            }

            return posts.ToList();
        }

        public async Task<List<Post>> SearchAllPost(string format, string buscar, int page, HttpContext httpContext)
        {
            // Usuario registrado
            var usuarioID = _usersService.ObtenerUsuario();

            var cantidadPorPagina = Session.GetCantidadPostsSession(httpContext);

            IEnumerable<Post> posts = await _repositoryPosts.ObtenerPorFormato(format, cantidadPorPagina, page, usuarioID);

            if (!buscar.IsNullOrEmpty())
            {
                posts = posts.Where(p => p.title.ToUpper().Contains(buscar.ToUpper()));
            }

            return posts.ToList();
        }

        // Obtener PostViewModel para crear post
        //** Completamos el view model con las listas de categorias, formatos, media types y sources
        public async Task<PostViewModel> GetPostViewModel(string format, PostViewModel viewModel = null)
        {

            // Si no se manda un view model, se crea uno nuevo
            if (viewModel == null)
                viewModel = new PostViewModel();

            var usuarioID = viewModel.user_id;

            if(usuarioID == 0)
            {
                usuarioID = _usersService.ObtenerUsuario();
                viewModel.user_id = usuarioID;
            }

            if(viewModel.format_id == 0)
                viewModel.format = format;

            //Obtenemos Categorias Select List
            viewModel.categories = await ObtenerCategorias(usuarioID);

            //Obtenemos Media Types Select List
            viewModel.mediaTypes = await ObtenerMediaTypes(usuarioID);

            //Obtener fuente de los links
            viewModel.sources = await ObtenerSource(usuarioID);

            return viewModel;
        }

        // Crear Post
        // ** Se guardan Categoria, Media, MediaType y Source que haya completado el usuario
        public async Task CreatePost(PostViewModel viewModel)
        {
            await _repositoryPosts.Crear(viewModel);
        }

        //****************************************************
        //***************** METODOS PRIVADOS *****************
        //****************************************************

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int user_id)
        {
            var categories = await _categoriaService.GetAllCategorias(user_id);
            return categories.Select(category => new SelectListItem(category.name, category.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerFormatos(int user_id)
        {
            var formats = await _formatService.GetAllFormat(user_id);
            return formats.Select(format => new SelectListItem(format.name, format.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerMediaTypes(int user_id)
        {
            var mediaTypes = await _mediaTypeService.GetAllMediaType(user_id);
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerSource(int user_id)
        {
            var mediaTypes = await _sourceService.GetAllSource(user_id);
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
        }
    }
}
