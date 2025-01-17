using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace Backend_portafolio.Services
{
    public interface IBioService
    {
        Task CreateBio(BioViewModel viewModel);
        Task DeleteBio(int id);
        Task EditBio(BioViewModel viewModel);
        Task<BioViewModel> GetBioViewModel(BioViewModel viewModel = null);
    }
    public class BioService : IBioService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryRole _repositoryRole;
        private readonly IRepositoryUsers _repositoryUsers;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IUsersService _usersService;
        private readonly IRepositoryBio _repositoryBio;
        private readonly IImageService _imageService;
        private readonly HttpContext _httpContext;

        public BioService(
            UserManager<User> userManager,
            IRepositoryRole repositoryRole,
            IRepositoryUsers repositoryUsers,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<User> MySignInManager,
            IMapper mapper,
            IUsersService usersService,
            IRepositoryBio repositoryBio,
            IImageService imageService
        )
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userManager = userManager;
            _repositoryRole = repositoryRole;
            _repositoryUsers = repositoryUsers;
            _signInManager = MySignInManager;
            _mapper = mapper;
            _usersService = usersService;
            _repositoryBio = repositoryBio;
            _imageService = imageService;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        public async Task<BioViewModel> GetBioViewModel(BioViewModel viewModel = null)
        {
            try
            {
                viewModel = viewModel ?? new BioViewModel();
                var usuarioID = _usersService.ObtenerUsuario();
                viewModel.user_id = usuarioID;
                viewModel.Bios = (await _repositoryBio.Obtener(usuarioID)).OrderByDescending(x => x.year).ToList();
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

        public async Task CreateBio(BioViewModel viewModel)
        {
            try
            {
                // Validar usuario
                var userID = _usersService.ObtenerUsuario();
                if (userID != viewModel.user_id)
                    throw new ApplicationException("No puedes crear una bio para otro usuario");

                await _repositoryBio.Agregar(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        public async Task DeleteBio(int id)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();
                var bio = await _repositoryBio.ObtenerPorId(id, usuarioID);

                // Validar que la bio exista
                if (bio == null)
                    throw new ApplicationException("Bio no encontrada");

                // Validar que el usuario sea el dueño de la bio
                if (bio.user_id != usuarioID)
                    throw new ApplicationException("No puedes eliminar una bio de otro usuario");

                await _repositoryBio.Borrar(id, usuarioID);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditBio(BioViewModel viewModel)
        {
            try
            {
                //Validar usuario
                var usuarioID = _usersService.ObtenerUsuario();
                if (usuarioID != viewModel.user_id)
                    throw new ApplicationException("No puedes editar una bio de otro usuario");

                //Validar que la bio exista
                var bio = await _repositoryBio.ObtenerPorId(viewModel.id, usuarioID);
                if (bio == null)
                    throw new ApplicationException("Bio no encontrada");

                await _repositoryBio.Editar(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
