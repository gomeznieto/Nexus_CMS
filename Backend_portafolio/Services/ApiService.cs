using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;

namespace Backend_portafolio.Services
{
    public interface IApiService
    {
        Task<List<Categoria>> GetCategories(string apiKey);
        Task<List<Format>> GetFormats(string apiKey);
        Task<UserApiViewModel> GetUser(string apiKey);
    }
    public class ApiService : IApiService
    {
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;
        private readonly ICategoriaService _categoriaService;
        private readonly IFormatService _formatService;
        private readonly IBioService _bioService;
        private readonly INetworkService _networkService;
        private readonly IMapper _mapper;

        public ApiService(
            IUsersService usersService,
            ITokenService tokenService,
            ICategoriaService categoriaService,
            IFormatService formatService,
            IBioService bioService,
            INetworkService networkService,
            IMapper mapper
        )
        {
            _usersService = usersService;
            _tokenService = tokenService;
            _categoriaService = categoriaService;
            _formatService = formatService;
            _bioService = bioService;
            _networkService = networkService;
            _mapper = mapper;
        }

        //****************************************************
        //************************ USER **********************
        //****************************************************

        public async Task <UserApiViewModel> GetUser(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var userApiViewModel = _mapper.Map<UserApiViewModel>(user);

                //Obtener Bio
                userApiViewModel.Bios = await _bioService.GetAllBio(user.id);

                //Obtener Redes
                userApiViewModel.Networks = await _networkService.GetSocialNetworksByUserId(user.id);

                return userApiViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //****************************************************
        //******************** CATEGORIES ********************
        //****************************************************

        public async Task<List<Categoria>> GetCategories(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var categories = await _categoriaService.GetAllCategorias(user.id);
                return categories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************** FORMATS *********************
        //****************************************************

        public async Task<List<Format>> GetFormats(string apiKey)
        {
            try
            {
                await _tokenService.ValidateApiKey(apiKey);
                var user = await _usersService.GetUserByApiKey(apiKey);
                var formats = await _formatService.GetAllFormat(user.id);
                return formats.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //TIPO DE ENTRADAS

        //ENTRADAS POR TIPO
        //// CON SU CATEGORIA
        //// CON SU TIPO DE ENTRADA

    }
}
