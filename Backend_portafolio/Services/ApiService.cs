using AutoMapper;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;

namespace Backend_portafolio.Services
{
    public interface IApiService
    {
        Task<ApiResponse<List<CategoryViewModel>>> GetCategories(string apiKey);
        Task<ApiResponse<List<FormatViewModel>>> GetFormats(string apiKey);
        Task<ApiResponse<ApiResponsePosts<ApiPostViewModel>>> GetPostById(string apiKey, int post_id);
        Task<ApiResponse<ApiResponsePosts<List<ApiPostViewModel>>>> GetAllPosts(string apiKey);
        Task<ApiResponse<ApiResponsePosts<List<ApiPostViewModel>>>> GetPostsPagination(string apiKey, int pageNumber, int pageSize);
        Task<ApiResponse<ApiUserViewModel>> GetUser(string apiKey);
    }
    public class ApiService : IApiService
    {
        private readonly IBioService _bioService;
        private readonly ICategoriaService _categoriaService;
        private readonly IFormatService _formatService;
        private readonly IMapper _mapper;
        private readonly INetworkService _networkService;
        private readonly IPostService _postService;
        private readonly ITokenService _tokenService;
        private readonly IMediaService _mediaService;
        private readonly ILinkService _linkService;
        private readonly IUsersService _usersService;

        public ApiService(
            IBioService bioService,
            ICategoriaService categoriaService,
            IFormatService formatService,
            ILinkService linkService,
            IMapper mapper,
            IMediaService mediaService,
            INetworkService networkService,
            IPostService postService,
            ITokenService tokenService,
            IUsersService usersService
        )
        {
            _bioService = bioService;
            _categoriaService = categoriaService;
            _formatService = formatService;
            _linkService = linkService;
            _mapper = mapper;
            _mediaService = mediaService;
            _networkService = networkService;
            _postService = postService;
            _tokenService = tokenService;
            _usersService = usersService;
        }

        //****************************************************
        //************************ USER **********************
        //****************************************************

        /**
         * Retorna un usuario por apiKey
         * @param apiKey Clave
        */
        public async Task<ApiResponse<ApiUserViewModel>> GetUser(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var userApiViewModel = _mapper.Map<ApiUserViewModel>(user);

                //Obtener Bio
                userApiViewModel.Bios = await _bioService.GetAllBio(user.id);

                //Obtener Redes
                userApiViewModel.Networks = await _networkService.GetSocialNetworksByUserId(user.id);

                var result = new ApiResponse<ApiUserViewModel>()
                {
                    Success = true,
                    Message = "",
                    Data = userApiViewModel
                };

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //****************************************************
        //******************** CATEGORIES ********************
        //****************************************************

        /**
         * Retorna una lista de categorías
         * @param apiKey Clave
        */
        public async Task<ApiResponse<List<CategoryViewModel>>> GetCategories(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var categories = await _categoriaService.GetAllCategorias(user.id);

                var result = new ApiResponse<List<CategoryViewModel>>()
                {
                    Success = true,
                    Message = "",
                    Data = categories.ToList()
                };


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************** FORMATS *********************
        //****************************************************

        /**
         * Retorna una lista de formatos
         * @param apiKey Clave
        */
        public async Task<ApiResponse<List<FormatViewModel>>> GetFormats(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var formats = await _formatService.GetAllFormat(user.id);
                
                var result = new ApiResponse<List<FormatViewModel>>()
                {
                    Success = true,
                    Message = "",
                    Data = formats.ToList()
                };

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //*********************** PSOTS **********************
        //****************************************************

        /**
         * Retorna una lista de publicaciones
         * @param apiKey Clave
        */
        public async Task<ApiResponse<ApiResponsePosts<List<ApiPostViewModel>>>> GetAllPosts(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                List<PostViewModel> posts = await _postService.GetAllPosts(user.id);

                List<ApiPostViewModel> postsApiModels = _mapper.Map<List<ApiPostViewModel>>(posts);
                //List<ApiPostViewModel> postsApiModels = new List<ApiPostViewModel>();

                //foreach (var post in posts)
                //{
                //    var addPost = new ApiPostViewModel()
                //    {
                //        id = post.id,
                //        title = post.description,
                //        description = post.description,
                //        cover = post.cover,
                //        format = post.format,
                //        created_at = post.created_at

                //    };

                //    postsApiModels.Add(addPost);
                //}

                foreach (var post in postsApiModels)
                {
                    post.media = _mapper.Map<List<ApiMediaViewModel>>
                        (await _mediaService.GetMediaByPost(post.id));

                    post.links = _mapper.Map<List<ApiLinkViewModel>>
                        (await _linkService.GetLinkByPost(post.id));

                    post.categories = _mapper.Map<List<ApiCategoryViewModel>>
                        ((await _categoriaService.GetCategoriasByPost(post.id))
                        .ToList()
                        .Select(c => c.Categoria)).ToList();
                }

                var data = new ApiResponsePosts<List<ApiPostViewModel>>()
                {
                    Items = postsApiModels,
                    TotalRecords = posts.Count()
                };

                return new ApiResponse<ApiResponsePosts<List<ApiPostViewModel>>>()
                {
                    Success = true,
                    Message = "",
                    Data = data,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Retorna una publicación por id
         * @param apiKey Clave
         * @param post_id Id de la publicación
        */
        public async Task<ApiResponse<ApiResponsePosts<ApiPostViewModel>>> GetPostById(string apiKey, int post_id)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var post = await _postService.GetPostById(post_id, user.id);

                var postApiModel = _mapper.Map<ApiPostViewModel>(post);

                postApiModel.media = _mapper.Map<List<ApiMediaViewModel>>
                    (await _mediaService.GetMediaByPost(post_id));

                postApiModel.links = _mapper.Map<List<ApiLinkViewModel>>
                    (await _linkService.GetLinkByPost(post_id));

                postApiModel.categories = _mapper.Map<List<ApiCategoryViewModel>>
                    ((await _categoriaService.GetCategoriasByPost(post_id))
                    .ToList()
                    .Select(c => c.Categoria));

                var data = new ApiResponsePosts<ApiPostViewModel>()
                {
                    Items = postApiModel,
                    TotalRecords = 1
                };

                return new ApiResponse<ApiResponsePosts<ApiPostViewModel>>()
                {
                    Success = true,
                    Message = "",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Retorna una lista de publicaciones paginadas
         * @param apiKey Clave
         * @param pageNumber Número de página
         * @param pageSize Tamaño de la página
        */
        public async Task<ApiResponse<ApiResponsePosts<List<ApiPostViewModel>>>> GetPostsPagination(string apiKey, int pageNumber, int pageSize)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var posts = await _postService.GetAllPosts(user.id);

                var postsApiModels = _mapper.Map<List<ApiPostViewModel>>(posts);

                foreach (var post in postsApiModels)
                {
                    post.media = _mapper.Map<List<ApiMediaViewModel>>
                        (await _mediaService.GetMediaByPost(post.id));

                    post.links = _mapper.Map<List<ApiLinkViewModel>>
                        (await _linkService.GetLinkByPost(post.id));

                    post.categories = _mapper.Map<List<ApiCategoryViewModel>>
                        ((await _categoriaService.GetCategoriasByPost(post.id))
                        .ToList()
                        .Select(c => c.Categoria));
                }

                if (pageNumber < 1)
                {
                    pageNumber = 1;
                }

                if (pageSize < 1)
                {
                    pageSize = 10;
                }

                int totalPages = (int)Math.Ceiling((double)posts.Count() / pageSize);

                if (pageNumber > totalPages)
                {
                    pageNumber = totalPages;
                }

                var postApiModelsPagination = postsApiModels.Skip(pageNumber).Take(pageSize).ToList();

                var data = new ApiResponsePosts<List<ApiPostViewModel>>()
                {
                    Items = postApiModelsPagination,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = posts.Count(),
                    TotalPages = totalPages
                };

                return new ApiResponse<ApiResponsePosts<List<ApiPostViewModel>>>()
                {
                    Success = true,
                    Message = "",
                    Data = data,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
