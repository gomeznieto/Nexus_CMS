using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;

namespace Backend_portafolio.Services
{
    public interface IHomeService
    {
        Task CreatePostFromHomeView(HomeViewModel viewModel);
        Task<HomeViewModel> GetHomeViewModel(HomeViewModel viewModel = null);
    }

    public class HomeService : IHomeService
    {
        private readonly IUsersService _usersService;
        private readonly IPostService _postService;
        private readonly IFormatService _formatService;
        private readonly HttpContext _httpContext;

        public HomeService(
            IUsersService usersService,
            IHttpContextAccessor httpContextAccessor,
            IPostService postService,
            IFormatService formatService
        )
        {
            _usersService = usersService;
            _postService = postService;
            _formatService = formatService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        public async Task<HomeViewModel> GetHomeViewModel(HomeViewModel viewModel = null)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                await Session.UpdateSession(_httpContext, _formatService, userID);

                if(viewModel == null)
                    viewModel = new HomeViewModel();

                viewModel.formatList = await _formatService.GetAllFormat();
                viewModel.ultimosPosts = await _postService.GetAllPosts();
                viewModel.user_id = userID;

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreatePostFromHomeView(HomeViewModel viewModel)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                var format = await _formatService.GetFormatById(viewModel.format_id);

                if(format == null)
                    throw new ApplicationException("El formato seleccionado no existe");

                viewModel.draft = true;

                await _postService.CreatePost(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}
