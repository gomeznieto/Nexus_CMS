using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{

    public class ApiController : ControllerBase
    {
        private readonly IRepositoryCategorias _repositoryCateogorias;
        private readonly IRepositoryPosts _repositoryPosts;
        private readonly IRepositoryLink _repositoryLink;
        private readonly IRepositoryMedia _repositoryMedia;
        private readonly IRepositoryFormat _repositoryFormat;
        private readonly IRepositoryUsers _repositoryUser;
        private readonly IApiService _apiService;

        public ApiController(
            IRepositoryCategorias repositoryCateogorias,
            IRepositoryPosts repositoryPost,
            IRepositoryLink repositoryLink,
            IRepositoryMedia repositoryMedia,
            IRepositoryFormat repositoryFormat,
            IRepositoryUsers repositoryUser,
            IHttpContextAccessor httpContextAccessor,
            IUsersService usersService,
            ITokenService tokenService,
            IApiService apiService
            )
        {
            _repositoryCateogorias = repositoryCateogorias;
            _repositoryPosts = repositoryPost;
            _repositoryLink = repositoryLink;
            _repositoryMedia = repositoryMedia;
            _repositoryFormat = repositoryFormat;
            _repositoryUser = repositoryUser;
            _apiService = apiService;
        }

        //****************************************************
        //******************** CATEGOIAS *********************
        //****************************************************

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Categoria([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
               var categories = await _apiService.GetCategories(apiKey);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //****************************************************
        //******************** FORMATOS **********************
        //****************************************************
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Formato([FromHeader(Name = "X-Api-Key")] string apiKey)
        {

            try
            {
                var formats = await _apiService.GetFormats(apiKey);
                return Ok(formats);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //****************************************************
        //********************* USUARIOS *********************
        //****************************************************

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Usuario([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                var user = await _apiService.GetUser(apiKey);
                return Ok(user);
            }
            catch (Exception ex)
            {
                    return BadRequest(ex.Message);
            }
        }

        //****************************************************
        //******************** ENTRADAS **********************
        //****************************************************

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Entrada([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // Validar el securityStamp
            var tokenValido = await _repositoryUser.ValidarApiKey(apiKey);

            if (!tokenValido)
                return BadRequest("Token inválido.");

            // Obtener el usuario
            var usuario = await _repositoryUser.ObtenerUsuarioPorApiKey(apiKey);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var posts = await _repositoryPosts.Obtener(usuario.id);

            if (posts is null)
                return NotFound();

            List<ApiPostModel> postsApiModels = new List<ApiPostModel>();

            foreach (var post in posts)
            {
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

                var categories = await _repositoryCateogorias.ObtenerPorPost(aux.id);
                aux.categories = categories.Select(l => new ApiCategoryModel { id = l.id, category = l.name }).ToList();

                postsApiModels.Add(aux);
            }

            return Ok(postsApiModels);
        }

        [AllowAnonymous]
        [HttpGet("{controller}/{action}/{id}")]
        public async Task<IActionResult> Entrada([FromHeader(Name = "X-Api-Key")] int id)
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
