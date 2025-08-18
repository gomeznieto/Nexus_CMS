using AutoMapper;
using Backend_portafolio.Constants;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        Task<List<ApiHomeSectionViewModel>> GetHomeSection(string apiKey);
        Task<ApiResponse<List<ApiLayoutHomeModel>>> GetHomeLayout(string apiKey);
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHomeSectionService _homeSectionService;
        private readonly ILayoutService _layoutService;
        private readonly IHomeSectionPostService _homeSectionPost;

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
            IUsersService usersService,
            IWebHostEnvironment webHostEnvironment,
            IHomeSectionService homeSectionService,
            ILayoutService layoutService,
            IHomeSectionPostService homeSectionPost
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
            _webHostEnvironment = webHostEnvironment;
            _homeSectionService = homeSectionService;
            _layoutService = layoutService;
            _homeSectionPost = homeSectionPost;
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
                var networks = await _networkService.GetSocialNetworksByUserId(user.id);

                foreach (var network in networks)
                {
                    string svgContent = null;
                    // Construir la ruta física del archivo SVG
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, network.icon.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        svgContent = await System.IO.File.ReadAllTextAsync(filePath);
                        svgContent = svgContent.Replace("fill=\"#0F0F0F\"", "fill=\"currentColor\"");
                        svgContent = svgContent.Replace("fill=\"#000000\"", "fill=\"currentColor\"");
                        svgContent = svgContent.Replace("fill=\"#000\"", "fill=\"currentColor\"");
                        svgContent = svgContent.Replace("fill=\"#fff\"", "fill=\"currentColor\"");
                        svgContent = svgContent.Replace("width=\"800px\"", "");
                        svgContent = svgContent.Replace("height=\"800px\"", "");

                        if (!svgContent.Contains("fill=\"currentColor\"") && svgContent.Contains("<svg"))
                        {
                            svgContent = svgContent.Replace("<svg", "<svg fill=\"currentColor\"");
                        }
                    }

                    network.icon = svgContent;

                }

                userApiViewModel.Networks = networks;

                // Pasar iconos a SVG

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
        //*********************** HOME ***********************
        //****************************************************
        public async Task<List<ApiHomeSectionViewModel>> GetHomeSection(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);

                if (user is null)
                {
                    throw new Exception("No tiene autorización");
                }

                var homeSections = await _homeSectionService.GetByUserAsync(user.id);

                var homeSectionViewModels = homeSections.Select(async el =>
                {
                    var postListBySection = await _postService.GetPostsGroupedBySectionAsync((int)el.Id, user);

                    return new ApiHomeSectionViewModel()
                    {
                        HomeSectionId = (int)el.Id,
                        HomeSectionName = el.Name,
                        Posts = postListBySection,
                    };
                });

                return (await Task.WhenAll(homeSectionViewModels)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //*********************** POSTS **********************
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

                //Pasar SVG a HTML
                foreach (var el in postsApiModels)
                {
                    foreach (var link in el.links)
                    {
                        string svgContent = null;
                        // Construir la ruta física del archivo SVG
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, link.icon.TrimStart('/'));

                        if (System.IO.File.Exists(filePath))
                        {
                            svgContent = await System.IO.File.ReadAllTextAsync(filePath);
                            svgContent = svgContent.Replace("fill=\"#0F0F0F\"", "fill=\"currentColor\"");
                            svgContent = svgContent.Replace("fill=\"none\"", "fill=\"currentColor\"");
                            svgContent = svgContent.Replace("fill=\"#000000\"", "fill=\"currentColor\"");
                            svgContent = svgContent.Replace("fill=\"#000\"", "fill=\"currentColor\"");
                            svgContent = svgContent.Replace("fill=\"#fff\"", "fill=\"currentColor\"");
                            svgContent = svgContent.Replace("width=\"800px\"", "");
                            svgContent = svgContent.Replace("height=\"800px\"", "");

                            if (!svgContent.Contains("fill=\"currentColor\"") && svgContent.Contains("<svg"))
                            {
                                svgContent = svgContent.Replace("<svg", "<svg fill=\"currentColor\"");
                            }
                        }

                        link.icon = svgContent;
                    }

                }

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

        //****************************************************
        //*********************** LAYOOUT ********************
        //****************************************************
        public async Task<ApiResponse<List<ApiLayoutHomeModel>>> GetHomeLayout(string apiKey)
        {
            try
            {
                // API response model
                var apiLayoutResponse = new List<ApiLayoutHomeModel>();

                //User
                var user = await _usersService.GetUserByApiKey(apiKey);

                // Obtenemos cada una de las secciones del Layout
                var layoutHomeSections = await _layoutService.GetLayoutForm(user.id);

                foreach (var section in layoutHomeSections.Sections)
                {

                    var option = section.SectionType;

                    var layoutSection = new ApiLayoutHomeModel()
                    {
                        Order = section.DisplayOrder,
                        Type = section.SectionType,
                    };

                    switch (option)
                    {
                        case SectionTypesNames.UserAbout:
                            layoutSection.Data = new
                            {
                                Name = user.name,
                                Headline = user.headline,
                                About = user.about,
                                ProfileImage = user.img,
                                Mail = user.email,
                            };

                            break;
                        case SectionTypesNames.Bio:
                            layoutSection.Data = new
                            {
                                Bios = (await _bioService.GetAllBio(user.id)).OrderBy(x => x.year).ToList(),
                            };
                            break;
                        case SectionTypesNames.UserHobbies:
                            layoutSection.Data = new
                            {
                                Hobbies = user.hobbies
                            };

                            break;
                        case SectionTypesNames.SocialNetworks:
                            var socialNetworks = await _networkService.GetSocialNetworksByUserId(user.id);

                            foreach (var network in socialNetworks)
                            {
                                string svgContent = null;
                                // Construir la ruta física del archivo SVG
                                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, network.icon.TrimStart('/'));

                                if (System.IO.File.Exists(filePath))
                                {
                                    svgContent = await System.IO.File.ReadAllTextAsync(filePath);
                                    svgContent = svgContent.Replace("fill=\"#0F0F0F\"", "fill=\"currentColor\"");
                                    svgContent = svgContent.Replace("fill=\"#000000\"", "fill=\"currentColor\"");
                                    svgContent = svgContent.Replace("fill=\"#000\"", "fill=\"currentColor\"");
                                    svgContent = svgContent.Replace("fill=\"#fff\"", "fill=\"currentColor\"");
                                    svgContent = svgContent.Replace("width=\"800px\"", "");
                                    svgContent = svgContent.Replace("height=\"800px\"", "");

                                    if (!svgContent.Contains("fill=\"currentColor\"") && svgContent.Contains("<svg"))
                                    {
                                        svgContent = svgContent.Replace("<svg", "<svg fill=\"currentColor\"");
                                    }
                                }

                                network.icon = svgContent;

                            }

                            layoutSection.Data = new
                            {
                                SocialNetwork = socialNetworks,
                            };
                            break;
                        case SectionTypesNames.HomeSection:

                            var homeSection = await _homeSectionService.GetByIdAsync((int)section.SectionId, user.id);

                            layoutSection.Name = homeSection.Name;

                            layoutSection.Data = new
                            {
                                posts = await _postService.GetPostsGroupedBySectionAsync((int)section.SectionId, user)
                            };
                            break;

                    }

                    apiLayoutResponse.Add(layoutSection);

                }

                return new ApiResponse<List<ApiLayoutHomeModel>>()
                {
                    Success = true,
                    Message = "",
                    Data = apiLayoutResponse,
                };

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
