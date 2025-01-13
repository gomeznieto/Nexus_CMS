using AutoMapper;
using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IRepositorySource _repositorySource;
        private readonly IRepositoryLink _repositoryLink;
        private readonly IMapper _mapper;

        public PostsController(
            IUsersService usersService,
            IRepositoryCategorias repositoryCategorias,
            IRepositoryFormat repositoryFormat,
            IRepositoryPosts repositoryPosts,
            IRepositoryMedia repositoryMedia,
            IRepositoryMediatype repositoryMediatype,
            IRepositorySource repositorySource,
            IRepositoryLink repositoryLink,
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
            _mapper = mapper;
        }


        //--------------------------------------
        // INDEX
        //--------------------------------------

        [HttpGet]

        public async Task<IActionResult> Index(string format, int page = 1)
        {
            try
            {
                // Usuario registrado
                var usuarioID = _usersService.ObtenerUsuario();

                //crear session de cantidad de post en caso de no haber sido ya creada
                if (Session.GetCantidadPostsSession(HttpContext) == -1)
                {
                    Session.CantidadPostsSession(HttpContext, 10);
                }

                //Obtener cantidades para generar paginación
                var cantidadPorPagina = Session.GetCantidadPostsSession(HttpContext);
                IEnumerable<Post> posts = await _repositoryPosts.ObtenerPorFormato(format, cantidadPorPagina, page, usuarioID);

                //Obtenemos categorias para mostrar en lista
                foreach (var post in posts)
                {
                    post.categoryList = await _repositoryCategorias.ObtenerCategoriaPostPorId(post.id);
                }

                //Formato para retornar al listado correspondiente
                ViewBag.Format = format;
                ViewBag.Cantidad = await _repositoryPosts.ObtenerCantidadPorFormato(format);
                ViewBag.Message = "No hay entradas para mostrar";

                return View(posts);
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
                Session.ErrorSession(HttpContext, errorModal);

                return RedirectToAction("Index", "Home");
            }
        }


        // Lógica para el buscador
        [HttpPost]
        public async Task<IActionResult> Index(string format, string buscar, int page = 1)
        {
            try
            {

                // Usuario registrado
                var usuarioID = _usersService.ObtenerUsuario();

                var cantidadPorPagina = Session.GetCantidadPostsSession(HttpContext);

                IEnumerable<Post> posts = await _repositoryPosts.ObtenerPorFormato(format, cantidadPorPagina, page, usuarioID);

                if (!buscar.IsNullOrEmpty())
                {
                    posts = posts.Where(p => p.title.ToUpper().Contains(buscar.ToUpper()));
                }

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


        //--------------------------------------
        /// CREATE
        //--------------------------------------

        [HttpGet]
        public async Task<IActionResult> Crear(string format)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();

                var model = new PostViewModel();

                model.user_id = usuarioID;
                model.format = format;

                //Obtenemos el formato de la entrada creada
                var formats = await _repositoryFormat.Obtener(usuarioID);
                model.format_id = formats.Where(f => f.name == format).Select(f => f.id).FirstOrDefault();
                model.format = formats.Where(f => f.name == format).Select(f => f.name).FirstOrDefault();

                //Obtenemos Categorias Select List
                model.categories = await ObtenerCategorias(usuarioID);

                //Obtenemos Media Types Select List
                model.mediaTypes = await ObtenerMediaTypes(usuarioID);

                //Obtener fuente de los links
                model.sources = await ObtenerSource(usuarioID);

                return View(model);
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Home" };
                Session.ErrorSession(HttpContext, errorModal);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear(PostViewModel viewModel)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                //verificamos que el model state sea valido antes de continuar
                if (!ModelState.IsValid)
                {
                    viewModel.categories = await ObtenerCategorias(userID);
                    viewModel.mediaTypes = await ObtenerMediaTypes(userID);
                    viewModel.sources = await ObtenerSource(userID);

                    // Agregar categorias, link y multimedia
                    if (!viewModel.sourceListString.IsNullOrEmpty())
                    {
                        //Deserializamos string de media
                        List<CategoryForm> categoriesForms = JsonSerializer.Deserialize<List<CategoryForm>>(viewModel.categoryListString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        List<Category_Post> categoryPostList = new List<Category_Post>();

                        foreach (var category in categoriesForms)
                        {
                            // Validar que cada categoría exista
                            var categorySearched = await _repositoryCategorias.ObtenerPorId(category.category_id);
                            categorySearched.id = category.category_id;

                            if (categorySearched == null)
                            {
                                //Crear mensaje de error para modal
                                Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Error en uno de los datos ingresados!", type = true, path = "Posts" });
                                return RedirectToAction("Index", "Posts", new { format = viewModel.format });
                            }
                            else
                            {
                                Category_Post aux = new Category_Post();
                                aux.Categoria = categorySearched;
                                categoryPostList.Add(aux);

                            }

                        }

                        viewModel.categoryList = categoryPostList;
                    }

                    return View(viewModel);
                }

                //Verificamos que el formato que nos mandan exista
                var Formato = await _repositoryFormat.ObtenerPorId(viewModel.format_id);

                if (Formato is null)
                {
                    //Crear mensaje de error para modal
                    Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Error en uno de los datos ingresados!", type = true, path = "Posts" });
                    return RedirectToAction("Index", "Posts", new { format = viewModel.format });
                }

                //Colocamos fecha actual
                viewModel.created_at = DateTime.Now;

                await _repositoryPosts.Crear(viewModel);

                // SUBIR MEDIA
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

                //SUBIR LINKS
                if (!viewModel.sourceListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<LinkForm> linksForms = JsonSerializer.Deserialize<List<LinkForm>>(viewModel.sourceListString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    //Mappeamos de MediaForm a Media
                    List<Link> links = _mapper.Map<List<Link>>(linksForms);

                    //Agregamos el numero de post creado a cada Media
                    foreach (var link in links)
                    {
                        link.post_id = viewModel.id;
                    }

                    //Subimos MeiaLinks
                    await _repositoryLink.Crear(links);
                }

                //SUBIR CATEGORIAS
                if (!viewModel.sourceListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<CategoryForm> categoriesForms = JsonSerializer.Deserialize<List<CategoryForm>>(viewModel.categoryListString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    foreach (var category in categoriesForms)
                    {
                        // Validar que cada categoría exista
                        var categorySearched = _repositoryCategorias.ObtenerPorId(category.category_id);

                        if (categorySearched == null)
                        {
                            //Crear mensaje de error para modal
                            Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Error en uno de los datos ingresados!", type = true, path = "Posts" });
                            return RedirectToAction("Index", "Posts", new { format = viewModel.format });
                        }

                        // Agregarle el número del post a CategoriaForm
                        category.post_id = viewModel.id;
                    }

                    //Subimos Las categorias del Post
                    await _repositoryCategorias.CrearCategoriaPorPost(categoriesForms);
                }

                return RedirectToAction("Index", "Posts", new { format = viewModel.format });
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Posts" };
                Session.ErrorSession(HttpContext, errorModal);

                //Redirect a la página anterior
                return RedirectToAction("Index", "Posts", new { format = viewModel.format });

            }
        }


        //--------------------------------------
        // EDIT
        //--------------------------------------
        [HttpGet]
        public async Task<IActionResult> Editar(int id, string format)
        {
            try
            {
                var model = await _repositoryPosts.ObtenerPorId(id);

                //Mapeamos de Post a PostViewModel
                var modelView = _mapper.Map<PostViewModel>(model);
                var userID = _usersService.ObtenerUsuario();

                modelView.user_id = userID;
                modelView.categories = await ObtenerCategorias(userID);
                modelView.mediaTypes = await ObtenerMediaTypes(userID);
                modelView.sources = await ObtenerSource(userID);

                modelView.mediaList = await _repositoryMedia.ObtenerPorPost(modelView.id);
                modelView.linkList = await _repositoryLink.ObtenerPorPost(modelView.id);
                modelView.categoryList = await _repositoryCategorias.ObtenerCategoriaPostPorId(modelView.id);

                var formats = await _repositoryFormat.Obtener(userID);
                modelView.format_id = formats.Where(f => f.name == format).Select(f => f.id).FirstOrDefault();
                modelView.format = formats.Where(f => f.name == format).Select(f => f.name).FirstOrDefault();

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

        [HttpPost]
        public async Task<IActionResult> Editar(PostViewModel viewModel)
        {
            try
            {
                /***** POST *****/
                var userID = _usersService.ObtenerUsuario();

                if (!ModelState.IsValid || viewModel.user_id != userID)
                {
                    viewModel.categories = await ObtenerCategorias(userID);
                    viewModel.formats = await ObtenerFormatos();
                    return View(viewModel);
                }

                //Verificamos que el formato que nos mandan exista
                var Formato = await _repositoryFormat.ObtenerPorId(viewModel.format_id);

                if (Formato is null)
                {
                    //Crear mensaje de error para modal
                    Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Error en uno de los datos ingresados!", type = true, path = "Posts" });
                    return RedirectToAction("Index", "Posts", new { format = viewModel.format });
                }

                viewModel.modify_at = DateTime.Now;

                await _repositoryPosts.Editar(viewModel);


                /***** METAS *****/

                if (!viewModel.mediaListString.IsNullOrEmpty() && viewModel.mediaListString != "[]")
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
                    List<LinkForm> linkForms = JsonSerializer.Deserialize<List<LinkForm>>(viewModel.sourceListString);
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
                if (!viewModel.sourceListString.IsNullOrEmpty())
                {
                    //Deserializamos string de media
                    List<CategoryForm> categoriesForms = JsonSerializer.Deserialize<List<CategoryForm>>(viewModel.categoryListString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    List<Category_Post> categoriesPosts = (await _repositoryCategorias.ObtenerCategoriaPostPorId(viewModel.id)).ToList();

                    foreach (var categoryForm in categoriesForms)
                    {
                        // Validar que cada categoría exista
                        var categorySearched = _repositoryCategorias.ObtenerPorId(categoryForm.category_id);

                        if (categorySearched == null)
                        {
                            //Crear mensaje de error para modal
                            Session.ErrorSession(HttpContext, new ModalViewModel { message = "¡Error en uno de los datos ingresados!", type = true, path = "Posts" });
                            return RedirectToAction("Index", "Posts", new { format = viewModel.format });
                        }

                        // Agregarle el número del post a CategoriaForm
                        //categoryForm.post_id = viewModel.id;

                        //Si la categoria no se encuentra en el listado ya subido, se agrega
                        if (categoriesPosts.All(x => x.Categoria.id != categoryForm.category_id))
                        {
                            await _repositoryCategorias.CrearCategoriaPorPost(categoryForm);
                        }
                    }

                    //Buscamos entre las categorias que ya están subidas por aquellas que no están en las que nos llegan
                    foreach(var categoryPost in categoriesPosts)
                    {
                        if (categoriesForms.All(x => x.category_id != categoryPost.Categoria.id))
                        {
                            await _repositoryCategorias.BorrarCatergoriaPorPost(categoryPost.post_id, categoryPost.Categoria.id);
                        }
                    }

                    //Subimos Las categorias del Post
                }

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

        [HttpPost]
        public async Task<IActionResult> EditarBorrador(int id, bool draft)
        {
            try
            {
                var mensaje = draft ? $"La entrada se ha colocado como borrador exitosamente" : $"La entrada se ha publicado exitosamente";

                await _repositoryPosts.EditarBorrador(id, draft);

                return Json(new { error = false, mensaje = mensaje });
            }
            catch (Exception ex)
            {

                return Json(new { error = true, mensaje = ex.Message });
            }

        }


        //--------------------------------------
        // DELTE
        //--------------------------------------
        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                //Verificar si existe
                var post = await _repositoryPosts.ObtenerPorId(id);

                if (post is null)
                    return View("NoEncontrado", "Home");

                await _repositoryPosts.Borrar(id);

                return Json(new { error = false, mensaje = "¡La entrada ha sido borrada correctamente!" });
            }
            catch (Exception)
            {
                return Json(new { error = true, mensaje = "La entrada no se pudo borrar.\n¡Se ha producido un error!" });

            }
        }


        //--------------------------------------
        // FUNCTIONS
        //--------------------------------------
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int user_id)
        {
            var categories = await _repositoryCategorias.Obtener(user_id);
            return categories.Select(category => new SelectListItem(category.name, category.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerFormatos()
        {
            var userID = _usersService.ObtenerUsuario();
            var formats = await _repositoryFormat.Obtener(userID);
            return formats.Select(format => new SelectListItem(format.name, format.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerMediaTypes(int user_id)
        {
            var mediaTypes = await _repositoryMediatype.Obtener(user_id);
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerSource(int user_id)
        {
            var mediaTypes = await _repositorySource.Obtener(user_id);
            return mediaTypes.Select(mediatype => new SelectListItem(mediatype.name, mediatype.id.ToString()));
        }

    }
}
