using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Backend_portafolio.Helper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;

namespace Backend_portafolio.Services
{
    public interface IUsersService
    {
        Task ChangePassword(UserViewModel viewModel);
        Task CreateUser(RegisterViewModel viewModel);
        Task EditUser(UserViewModel viewModel);
        Task<RegisterViewModel> GetRegisterViewModel(RegisterViewModel viewModel = null);
        Task<UserViewModel> GetUserViewModel();
        Task LoginUser(LoginViewModel viewModel);
        Task LogoutUser();
        int ObtenerUsuario();
    }

    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryRole _repositoryRole;
        private readonly IRepositoryUsers _repositoryUsers;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly HttpContext _httpContext;

        public UsersService(
            UserManager<User> userManager,
            IRepositoryRole repositoryRole,
            IRepositoryUsers repositoryUsers,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<User> MySignInManager,
            IMapper mapper,
            IImageService imageService
            )
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userManager = userManager;
            _repositoryRole = repositoryRole;
            _repositoryUsers = repositoryUsers;
            _signInManager = MySignInManager;
            _mapper = mapper;
            _imageService = imageService;
        }


        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        // Obtener el usuario autenticado
        public int ObtenerUsuario()
        {
            if (_httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = _httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }
        }

        // Obtener los datos del usuario
        public async Task<User> GetDataUser()
        {
            try
            {
                //var userID = ObtenerUsuario();
                //var user = await _repositoryUsers.BuscarPorId(userID);

                User user = await _signInManager.UserManager.GetUserAsync(_httpContext.User);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Obtener el viewmodel para el registro
        public async Task<RegisterViewModel> GetRegisterViewModel(RegisterViewModel viewModel = null)
        {
            try
            {
                viewModel = viewModel ?? new RegisterViewModel();
                viewModel.roles = (await _repositoryRole.Obtener()).ToList();

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<UserViewModel> GetUserViewModel()
        {
            try
            {
                var user = await GetDataUser();
                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.RoleName = (await _repositoryRole.Obtener()).Where(x => x.id == user.role).Select(x => x.name).FirstOrDefault();
                return userViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreateUser(RegisterViewModel viewModel)
        {
            try
            {

                //Obtenemos el usuario autenticado que está creando al nuevo usuario
                User currentUser = await _signInManager.UserManager.GetUserAsync(_httpContext.User);

                Role adminRole = (await _repositoryRole.Obtener()).FirstOrDefault(x => x.name == "admin");

                if (adminRole != null && currentUser.role != adminRole.id)
                    throw new ApplicationException("No tienes permisos para registrar un nuevo usuario");

                var newUSer = new User() { email = viewModel.Email, name = viewModel.Name, role = viewModel.role };
                newUSer.apiKey = ApiKey.GenerateApiKey();

                var result = await _userManager.CreateAsync(newUSer, password: viewModel.Password);

                if (!result.Succeeded)
                    throw new ApplicationException("Error al registrar el usuario");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditUser(UserViewModel viewModel)
        {
            try
            {
                User currentUser = await _signInManager.UserManager.GetUserAsync(_httpContext.User);
                User userEdit = _mapper.Map<User>(viewModel);

                userEdit.img = viewModel.ImageFile != null
                    ? await _imageService.UploadImageAsync(viewModel.ImageFile, currentUser, "profile-images")
                    : currentUser.img;

                var result = await _userManager.UpdateAsync(userEdit);

                if (!result.Succeeded)
                    throw new ApplicationException("Error al editar el usuario");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************** LOGIN ***********************
        //****************************************************

        public async Task LoginUser(LoginViewModel viewModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    throw new ApplicationException("Nombre del usuario o password incorrectos");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** LOGOUT **********************
        //****************************************************

        public async Task LogoutUser()
        {
            try
            {
                await _httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                _httpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //*********************** PASS ***********************
        //****************************************************

        public async Task ChangePassword(UserViewModel viewModel)
        {
            try
            {
                User currentUser = await _signInManager.UserManager.GetUserAsync(_httpContext.User);

                // Validar que el usuario exista
                if (currentUser == null)
                    throw new ApplicationException("Usuario no encontrado");

                // Validar la contraseña actual
                var isValidPassword = await _userManager.CheckPasswordAsync(currentUser, viewModel.password);

                if (!isValidPassword)
                    throw new ApplicationException("Contraseña incorrecta");

                // Editamso la pass
                bool result = await _repositoryUsers.EditarPass(currentUser, viewModel.passwordNuevo);

                // Validar que se haya moficado
                if (!result)
                    throw new ApplicationException("Error al cambiar la contraseña");

                //Validar que la constraseña se haya guardado correctamente
                currentUser = await _userManager.FindByIdAsync(currentUser.id.ToString());
                var isPasswordChanged = await _userManager.CheckPasswordAsync(currentUser, viewModel.passwordNuevo);
                
                if (!isPasswordChanged)
                    throw new ApplicationException("Error al cambiar la contraseña");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }

}
