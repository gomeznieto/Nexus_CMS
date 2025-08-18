using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Microsoft.IdentityModel.Tokens;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Sevices
{
    public interface IPostService
    {
        Task CreatePost(PostViewModel viewModel);
        Task CreatePost(Post viewModel);
        Task EditPost(PostViewModel viewModel);
        Task DeletePost(int id);
        Task<List<PostViewModel>> GetAllPosts(int userID = 0);
        Task<List<PostViewModel>> GetAllPosts(string format, int pagina);
        Task<PostViewModel> GetPostById(int id, int user_id = 0);
        Task<PostViewModel> GetPostViewModel(string format, PostViewModel v = null);
        Task<PostViewModel> PrepareEditPostViewModel(int id);
        Task<PostViewModel> PrepareViewModel(PostViewModel viewModel);
        Task<List<PostViewModel>> SearchAllPost(string format, string buscar, int page);
        Task<int> GetCountPostByFormat(string format);
        Task EditarBorrador(int id, bool draft);
        Task<int> GetCountPostByUser(int user_id);

        Task<List<ApiHomeSectionPostViewModel>> GetPostsGroupedBySectionAsync(int homeSectionId, UserViewModel user);
    }

    public class PostService : IPostService
    {
        private readonly IUsersService _usersService;
        private readonly IRepositoryCategorias _repositoryCategorias;
        private readonly IRepositoryPosts _repositoryPosts;
        private readonly IRepositoryMedia _repositoryMedia;
        private readonly IRepositoryLink _repositoryLink;
        private readonly ICategoriaService _categoriaService;
        private readonly IFormatService _formatService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IMediaService _mediaService;
        private readonly ILinkService _linkService;
        private readonly IEncryptionService _encryptionService;
        private readonly IImageService _imageService;
        private readonly IRepositoryHomeSection _repositoryHomeSection;
        private readonly IHomeSectionService _homeSectionService;
        private readonly IHomeSectionPostService _homeSectionPostService;
        private readonly ISourceService _sourceService;
        private readonly HttpContext _httpContext;
        private readonly IMapper _mapper;

        public PostService(
            IUsersService usersService,
            IRepositoryCategorias repositoryCategorias,
            IRepositoryPosts repositoryPosts,
            IRepositoryMedia repositoryMedia,
            IRepositoryLink repositoryLink,
            ICategoriaService categoriaService,
            IFormatService formatService,
            ISourceService sourceService,
            IMediaTypeService mediaTypeService,
            IMediaService mediaService,
            ILinkService linkService,
            IHttpContextAccessor httpContextAccessor,
            IEncryptionService encryptionService,
            IImageService imageService,
            IRepositoryHomeSection repositoryHomeSection,
            IHomeSectionService homeSectionService,
            IHomeSectionPostService homeSectionPostService,
            IMapper mapper)
        {
            _repositoryCategorias = repositoryCategorias;
            _repositoryPosts = repositoryPosts;
            _repositoryMedia = repositoryMedia;
            _repositoryLink = repositoryLink;
            _categoriaService = categoriaService;
            _formatService = formatService;
            _mediaTypeService = mediaTypeService;
            _mediaService = mediaService;
            _linkService = linkService;
            _encryptionService = encryptionService;
            _imageService = imageService;
            _repositoryHomeSection = repositoryHomeSection;
            _homeSectionService = homeSectionService;
            _homeSectionPostService = homeSectionPostService;
            _sourceService = sourceService;
            _usersService = usersService;
            _httpContext = httpContextAccessor.HttpContext;
            _mapper = mapper;
        }


        //****************************************************
        //******************* OBTENER POSTS ******************
        //****************************************************

        public async Task<List<PostViewModel>> GetAllPosts(int userID = 0)
        {
            try
            {
                if (userID == 0)
                    userID = _usersService.ObtenerUsuario();

                return _mapper.Map<List<PostViewModel>>((await _repositoryPosts.Obtener(userID)).ToList());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PostViewModel>> GetAllPosts(string format, int pagina)
        {

            try
            {
                // Usuario registrado
                var usuarioID = _usersService.ObtenerUsuario();

                //Averiguar si el formato existe
                var formato = await _formatService.Existe(format);

                if (!formato)
                    throw new Exception("¡El formato no existe!");

                //crear session de cantidad de post en caso de no haber sido ya creada
                if (Session.GetCantidadPostsSession(_httpContext) == -1)
                {
                    Session.CantidadPostsSession(_httpContext, 10);
                }

                //Obtener cantidades para generar paginación
                var cantidadPorPagina = Session.GetCantidadPostsSession(_httpContext);
                IEnumerable<PostViewModel> posts = _mapper.Map<IEnumerable<PostViewModel>>(await _repositoryPosts.ObtenerPorFormato(format, cantidadPorPagina, pagina, usuarioID));

                //Obtenemos categorias y seccion del home para mostrar en lista
                foreach (var post in posts)
                {
                    post.categoryList = await _repositoryCategorias.ObtenerCategoriaPostPorId(post.id);
                    post.HomeSectionPost = await _homeSectionPostService.GetByPostIdAsync(post.id) ?? new HomeSectionPostModel
                    {
                        Id = null,
                        PostId = post.id,
                        Order = 0,
                        HomeSectionId = 0,
                        Name = "-"
                    };
                }

                return posts.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public async Task<PostViewModel> GetPostById(int id, int user_id = 0)
        {
            try
            {
                if (user_id == 0)
                    user_id = _usersService.ObtenerUsuario();

                var post = await _repositoryPosts.ObtenerPorId(id, user_id);

                if (post is null)
                    throw new Exception("¡El post no existe!");

                return _mapper.Map<PostViewModel>(post);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //******************* BUSCAR POSTS *******************
        //****************************************************

        public async Task<List<PostViewModel>> SearchAllPost(string format, string buscar, int page)
        {
            // Usuario registrado
            var usuarioID = _usersService.ObtenerUsuario();

            var cantidadPorPagina = Session.GetCantidadPostsSession(_httpContext);
            IEnumerable<Post> posts;

            // En caso de que el formato sea nulo, obtenemos todas las entradas
            if (format is null)
            {
                posts = await _repositoryPosts.Obtener(usuarioID);
            }
            else
            {
                posts = await _repositoryPosts.ObtenerPorFormato(format, cantidadPorPagina, page, usuarioID);
            }

            if (!buscar.IsNullOrEmpty())
            {
                posts = posts.Where(p => p.title.ToUpper().Contains(buscar.ToUpper()));
            }

            return _mapper.Map<List<PostViewModel>>(posts.ToList());
        }

        //****************************************************
        //******************** VIEW MODELS *******************
        //****************************************************

        public async Task<PostViewModel> PrepareEditPostViewModel(int id)
        {
            var userID = _usersService.ObtenerUsuario();

            // Obtenemos el post por id
            var model = await GetPostById(id, userID);

            //Mapeamos de Post a PostViewModel
            var modelView = _mapper.Map<PostViewModel>(model);

            // Armamos el view model para editar el post
            modelView = await GetPostViewModel(modelView.format, modelView);

            return modelView;
        }

        // Obtener PostViewModel para crear post
        //** Completamos el view model con las listas de categorias, formatos, media types y sources
        public async Task<PostViewModel> GetPostViewModel(string format, PostViewModel viewModel = null)
        {

            try
            {
                // Si no se manda un view model, se crea uno nuevo
                if (viewModel == null)
                    viewModel = new PostViewModel();

                var usuarioID = viewModel.user_id;

                if (usuarioID == 0)
                {
                    usuarioID = _usersService.ObtenerUsuario();
                    viewModel.user_id = usuarioID;
                }

                // FORMATOS
                viewModel.formats = await ObtenerFormatos();
                viewModel.format_id = int.Parse(viewModel.formats.Where(f => f.Text == format).Select(f => f.Value).FirstOrDefault());
                viewModel.format = format;

                if (viewModel.format_id == 0)
                    throw new Exception("¡El formato no existe!");

                // HOME SECTION
                viewModel.HomeSectionList = await _homeSectionService.GetByUserAsync(usuarioID);

                // HOME SECTION POST
                viewModel.HomeSectionPost = await _homeSectionPostService.GetByPostIdAsync(viewModel.id)
                    ?? new HomeSectionPostModel
                    {
                        Id = null,
                        PostId = viewModel.id,
                        Order = 0,
                        HomeSectionId = 0
                    };

                // CATEGORIAS
                if (viewModel.id != 0)
                {
                    viewModel.mediaList = await _mediaService.GetMediaByPost(viewModel.id);
                    viewModel.linkList = await _linkService.GetLinkByPost(viewModel.id);
                    viewModel.categoryList = await _categoriaService.GetCategoriasByPost(viewModel.id);
                }

                viewModel = await PrepareSelectViewModel(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception("¡Se ha producido un error. Intente más tarde!", ex);
            }
        }

        // Prepara el view model para editar un post
        public async Task<PostViewModel> PrepareViewModel(PostViewModel viewModel)
        {
            // Obtener el viewModel con las listas de categorias, media types y sources
            viewModel = await GetPostViewModel(viewModel.format, viewModel);

            // Recueperar las categorias que se han seleccionado
            if (!viewModel.categoryListString.IsNullOrEmpty())
            {
                viewModel.categoryList = await _categoriaService.SerealizarJsonCategoryPost(viewModel.categoryListString);
            }

            // Recueperar las fuentes que se han seleccionado
            if (!viewModel.sourceListString.IsNullOrEmpty())
            {
                viewModel.linkList = _linkService.SerealizarJsonLink(viewModel.sourceListString);
            }

            // Recueperar los medios que se han seleccionado
            if (!viewModel.mediaListString.IsNullOrEmpty())
            {
                viewModel.mediaList = _mediaService.SerealizarJsonMedia(viewModel.mediaListString);
            }

            return viewModel;
        }

        public async Task<PostViewModel> PrepareSelectViewModel(PostViewModel viewModel)
        {
            //Obtenemos Categorias Select List parar mostrar en la vista
            viewModel.categories = await ObtenerCategorias();

            //Obtenemos Media Types Select List parar mostrar en la vista
            viewModel.mediaTypes = await ObtenerMediaTypes();

            //Obtener fuente Select List para mostrar en la vista
            viewModel.sources = await ObtenerSource();

            //Obtener formatos para mostrar en la vista
            viewModel.formats = await ObtenerFormatos();

            return viewModel;
        }

        //****************************************************
        //********************* CREAR POST *******************
        //****************************************************

        // ** Validar que el formato exista
        // ** Se guardan Categoria, Media, MediaType y Source que haya completado el usuario
        public async Task CreatePost(Post post)
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();

                post.user_id = userId;
                post.created_at = DateTime.Now;
                await _repositoryPosts.Crear(post);

            }
            catch (Exception ex)
            {
                throw new Exception("¡Se ha producido un error. Intente más tarde!", ex);
            }
        }

        public async Task CreatePost(PostViewModel viewModel)
        {
            try
            {
                // Usuario registrado
                var userID = _usersService.ObtenerUsuario();
                var currentUser = await _usersService.GetDataUser();

                if (userID != viewModel.user_id)
                {
                    throw new Exception("¡No tienes permisos para realizar esta acción!");
                }

                // Validar que el formato exista
                var Formato = await _formatService.GetFormatById(viewModel.format_id);

                if (Formato is null)
                {
                    throw new Exception("¡El formato no existe!");
                }

                // Cargamos la fecha de creación
                viewModel.created_at = DateTime.Now;

                // Creamos el post
                var createPost = _mapper.Map<Post>(viewModel);
                await _repositoryPosts.Crear(createPost);
                viewModel.id = createPost.id;

                //GUARDAR IMAGEN
                if (viewModel.ImageFile != null)
                {
                    viewModel.cover = await _imageService.UploadImageAsync(viewModel.ImageFile, currentUser, "posts", viewModel.id.ToString());
                    await _repositoryPosts.GuardarImagen(viewModel.id, viewModel.cover);

                }

                // SUBIR MEDIA
                if (!viewModel.mediaListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<MediaForm> mediaForms = _mediaService.SerealizarJsonMediaForm(viewModel.mediaListString);

                    //Mappeamos de MediaForm a Media
                    List<Media> medias = _mapper.Map<List<Media>>(mediaForms);

                    //Agregamos el numero de post creado a cada Media
                    foreach (var media in medias)
                    {
                        media.post_id = viewModel.id;
                    }

                    //Subimos MeiaLinks
                    await _mediaService.Create(medias);
                }

                //SUBIR LINKS
                if (!viewModel.sourceListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<LinkForm> linksForms = _linkService.SerealizarJsonLinkForm(viewModel.sourceListString);

                    //Mappeamos de MediaForm a Media
                    List<Link> links = _mapper.Map<List<Link>>(linksForms);

                    //Agregamos el numero de post creado a cada Media
                    foreach (var link in links)
                    {
                        link.post_id = viewModel.id;
                    }

                    //Subimos MeiaLinks
                    await _linkService.CreateLink(links);
                }

                //SUBIR CATEGORIAS
                if (!viewModel.categoryListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<CategoryForm> categoriesForms = _categoriaService.SerealizarJsonCategoryForm(viewModel.categoryListString);
                    await _categoriaService.CreateCategoriesForm(viewModel.id, categoriesForms);
                }

                // Guardar Seccion
                if (viewModel.HomeSectionPost.HomeSectionId != 0)
                {
                    var homeSectionModel = _mapper.Map<HomeSectionPostModel>(viewModel.HomeSectionPost);
                    //homeSectionModel.PostId = viewModel.id;
                    await _homeSectionPostService.CreateAsync(homeSectionModel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("¡Se ha producido un error. Intente más tarde!", ex);
            }

        }


        //****************************************************
        //******************** EDITAR POST *******************
        //****************************************************

        public async Task EditPost(PostViewModel viewModel)
        {
            try
            {
                // Usuario registrado
                var userID = _usersService.ObtenerUsuario();
                var currentUser = await _usersService.GetDataUser();

                if (userID != viewModel.user_id)
                {
                    throw new Exception("¡No tienes permisos para realizar esta acción!");
                }

                // Validar que el formato exista
                var format = await _formatService.GetFormatById(viewModel.format_id);

                if (format is null)
                {
                    throw new Exception("¡El formato no existe!");
                }

                var postExist = await GetPostById(viewModel.id);

                if (postExist == null)
                {
                    throw new Exception("¡El post que intenta modificar no existe!");
                }

                // Iamgen
                if (viewModel.ImageFile != null)
                {
                    viewModel.cover = await _imageService.UploadImageAsync(viewModel.ImageFile, currentUser, "posts", viewModel.id.ToString());
                }
                else
                {
                    viewModel.cover = postExist.cover;
                }

                // Cargamos la fecha de creación
                viewModel.modify_at = DateTime.Now;

                // Creamos el post
                await _repositoryPosts.Editar(_mapper.Map<Post>(viewModel));

                // Verificamos si se modificaron los datos de media
                if (!viewModel.mediaListString.IsNullOrEmpty() && viewModel.mediaListString != "[]")
                {
                    //Deserializamos string de media
                    List<MediaForm> mediaForms = _mediaService.SerealizarJsonMediaForm(viewModel.mediaListString);
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
                        else if (aux?.id is not 0 && aux?.url is not null)
                        {
                            //ACTUALIZAMOS
                            await _repositoryMedia.Editar(aux);
                        }
                        else
                        {
                            //ELIMINAMOS AL ESTAR URL NULL
                            await _repositoryMedia.Borrar(aux.id);
                        }
                    }

                    //Subimos MeiaLinks
                    await _repositoryMedia.Crear(medias);
                }

                /***** LINKS *****/
                if (!viewModel.sourceListString.IsNullOrEmpty() && viewModel.sourceListString != "[]")
                {
                    //Deserializamos string de media
                    List<LinkForm> linkForms = _linkService.SerealizarJsonLinkForm(viewModel.sourceListString);
                    List<Link> links = new List<Link>();

                    //Verificamos si entre los datos tenemos que actualizar algunos
                    foreach (var linkForm in linkForms)
                    {
                        Link aux = _mapper.Map<Link>(linkForm);

                        aux.post_id = viewModel.id;

                        if (aux?.id is 0)
                        {
                            //NUEVO
                            links.Add(aux);
                        }
                        else if (aux?.id is not 0 && aux?.url is not null)
                        {
                            //ACTUALIZAMOS
                            await _repositoryLink.Editar(aux);
                        }
                        else
                        {
                            //ELIMINAMOS AL ESTAR URL NULL
                            await _repositoryLink.Borrar(aux.id);
                        }
                    }

                    //Subimos Links
                    await _repositoryLink.Crear(links);
                }

                // CATEGOIRIAS
                //SUBIR CATEGORIAS
                if (!viewModel.categoryListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<CategoryForm> categoriesForms = _categoriaService.SerealizarJsonCategoryForm(viewModel.categoryListString);
                    List<Category_Post> categoriesPosts = (await _repositoryCategorias.ObtenerCategoriaPostPorId(viewModel.id)).ToList();

                    foreach (var categoryForm in categoriesForms)
                    {
                        // Validar que cada categoría exista
                        var categorySearched = _repositoryCategorias.ObtenerPorId(categoryForm.category_id);

                        if (categorySearched == null)
                        {
                            //Crear mensaje de error para modal
                            throw new Exception("La categoría no existe");
                        }


                        //Si la categoria no se encuentra en el listado ya subido, se agrega
                        if (categoriesPosts.All(x => x.Categoria.id != categoryForm.category_id))
                        {
                            await _repositoryCategorias.CrearCategoriaPorPost(categoryForm);
                        }
                    }

                    //Buscamos entre las categorias que ya están subidas por aquellas que no están en las que nos llegan
                    foreach (var categoryPost in categoriesPosts)
                    {
                        if (categoriesForms.All(x => x.category_id != categoryPost.Categoria.id))
                        {
                            await _repositoryCategorias.BorrarCatergoriaPorPost(categoryPost.post_id, categoryPost.Categoria.id);
                        }
                    }
                }

                // HOME SECTION POST
                // PUEDE VENIR SIN ID POR TRATARSE DE UNA NUEVA SECCION
                if (viewModel.HomeSectionPost is not null && viewModel.HomeSectionPost.HomeSectionId != 0)
                {
                    // Verificamos si la sección ya existe
                    var homeSectionPost = await _homeSectionPostService.GetByPostIdAsync(viewModel.id);

                    if (homeSectionPost is null)
                    {
                        // Creamos una nueva sección
                        var homeSectionModel = _mapper.Map<HomeSectionPostModel>(viewModel.HomeSectionPost);
                        await _homeSectionPostService.CreateAsync(homeSectionModel);
                    }
                    else
                    {
                        // Actualizamos la sección existente
                        homeSectionPost.Order = viewModel.HomeSectionPost.Order;
                        homeSectionPost.HomeSectionId = viewModel.HomeSectionPost.HomeSectionId;
                        await _homeSectionPostService.UpdateAsync(homeSectionPost);
                    }
                }
                else
                {
                    // Borramos la sección del post si no se ha seleccionado una sección
                    if (viewModel.HomeSectionPost.HomeSectionId == 0)
                    {
                        var homeSectionPost = await _homeSectionPostService.GetByPostIdAsync(viewModel.id);

                        if (homeSectionPost is not null)
                        {
                            await _homeSectionPostService.DeleteAsync(homeSectionPost);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception("¡Se ha producido un error. Intente más tarde!", ex);
            }

        }


        //****************************************************
        //******************** BORRAR POST *******************
        //****************************************************

        public async Task DeletePost(int id)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                var post = await GetPostById(id, userID);

                if (post is null)
                    throw new Exception("¡El post no existe!");

                await _repositoryPosts.Borrar(id);
            }
            catch (Exception ex)
            {
                throw new Exception("La entrada no se pudo borrar.\n¡Se ha producido un error!", ex);
            }
        }


        //****************************************************
        //******************* BORRADOR POST ******************
        //****************************************************

        public async Task EditarBorrador(int id, bool draft)
        {
            try
            {
                await _repositoryPosts.EditarBorrador(id, draft);
            }
            catch (Exception ex)
            {
                throw new Exception("¡Se ha producido un error. Intente más tarde!", ex);
            }
        }



        //****************************************************
        //************ CANTIDAD POST POR FORMATO *************
        //****************************************************

        public async Task<int> GetCountPostByFormat(string format)
        {
            try
            {
                return await _repositoryPosts.ObtenerCantidadPorFormato(format);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetCountPostByUser(int user_id)
        {
            try
            {
                return await _repositoryPosts.ObtenerCantidadPorUsuario(user_id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        //****************************************************
        //***************** METODOS PRIVADOS *****************
        //****************************************************

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias()
        {
            var categories = await _categoriaService.GetAllCategorias();
            return categories.Select(category => new SelectListItem(category.name, category.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerFormatos()
        {
            var formats = await _formatService.GetAllFormat();
            return formats.Select(format => new SelectListItem(format.name, format.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerMediaTypes()
        {
            var mediaTypes = await _mediaTypeService.GetAllMediaType();
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerSource()
        {
            var mediaTypes = await _sourceService.GetAllSource();
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
        }

        //****************************************************
        //************** POST POR HOME SECTION ***************
        //****************************************************

        public async Task<List<ApiHomeSectionPostViewModel>> GetPostsGroupedBySectionAsync(int homeSectionId, UserViewModel user)
        {
            try
            {
                var homeSectionPosts = await _homeSectionPostService.GetByHomeSectionIdAsync(homeSectionId);

                var posts = new List<ApiHomeSectionPostViewModel>();

                foreach (var homeSectionPost in homeSectionPosts)
                {
                    var post = await GetPostById(homeSectionPost.PostId, user.id);
                    var categories = await _categoriaService.GetCategoriasByPost(post.id);

                    if (post != null && homeSectionPost.HomeSectionId == homeSectionId)
                    {
                        var postViewModel = new ApiHomeSectionPostViewModel
                        {
                            Id = post.id,
                            Title = post.title,
                            Cover = post.cover, 
                            CategoriesList = categories.ToList(),
                            Slug = Utils.GenerateSlug(post.title),
                            Format = post.format
                        };

                        posts.Add(postViewModel);
                    }
                }

                return posts.ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
