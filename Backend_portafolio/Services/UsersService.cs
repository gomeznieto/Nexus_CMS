using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using System.Data;


namespace Backend_portafolio.Services
{
    public interface IUsersService
    {
        int ObtenerUsuario();
        Task ChangePassword(UserViewModel viewModel);
        Task CreateUser(RegisterViewModel viewModel);
        Task EditUser(UserViewModel viewModel);
        Task LoginUser(LoginViewModel viewModel);
        Task LogoutUser();
        Task VerifyEmail(string email);
        Task verifyNewPassword(string newPass, string repeatNewPass);
        Task verifyPassword(string pass);
        Task<RegisterViewModel> GetRegisterViewModel(RegisterViewModel viewModel = null);
        Task<User> GetUserByApiKey(string apiKey);
        Task<UserViewModel> GetUserViewModel();
        Task<User> GetUserByUser(string username);
        Task VerifyRole(string role);
        Task<RoleViewModel> GetRolesViewModel();
        Task CreateRole(RoleViewModel viewModel);
        Task DeleteRole(int id);
        Task EditRole(RoleViewModel viewModel);
        Task<Role> GetRoleById(int id);
    }

    public class UsersService : IUsersService
    {
        private readonly HttpContext _httpContext;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IRepositoryRole _repositoryRole;
        private readonly IRepositoryUsers _repositoryUsers;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UsersService(
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService,
            IMapper mapper,
            IRepositoryRole repositoryRole,
            IRepositoryUsers repositoryUsers,
            ITokenService tokenService,
            SignInManager<User> MySignInManager,
            UserManager<User> userManager
            )
        {
            _httpContext = httpContextAccessor.HttpContext;
            _imageService = imageService;
            _mapper = mapper;
            _repositoryRole = repositoryRole;
            _repositoryUsers = repositoryUsers;
            _signInManager = MySignInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }


        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        /**
         * Obtiene el id del usuario autenticado
         * @return id del usuario
         */
        public int ObtenerUsuario()
        {
            try
            {
                if (_httpContext.User.Identity.IsAuthenticated)
                {
                    var idClaim = _httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                    var id = int.Parse(idClaim.Value);
                    return id;
                }

                throw new ApplicationException("El usuario no está autenticado");

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /**
         * Obtiene los datos del usuario autenticado
         * @return Usuario
         */
        public async Task<User> GetDataUser()
        {
            try
            {
                User user = await _signInManager.UserManager.GetUserAsync(_httpContext.User);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Obtiene el modelo de vista de registro
         * @param viewModel Modelo de vista de registro
         * @return Modelo de vista de registro
         */
        public async Task<RegisterViewModel> GetRegisterViewModel(RegisterViewModel viewModel = null)
        {
            try
            {
                viewModel = viewModel ?? new RegisterViewModel();
                var roles = (await _repositoryRole.Obtener()).ToList();
                PasarMayusculas(ref roles);
                viewModel.roles = roles;

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Obtiene el modelo de vista de usuario
         * @return Modelo de vista de usuario
         */
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

        public async Task<User> GetUserByUser(string username)
        {
            try
            {
                var user = await _repositoryUsers.BuscarUsuarioPorUsername(username);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

            /**
             * Crea un nuevo usuario
             * @param viewModel Modelo de vista de registro
             */
        public async Task CreateUser(RegisterViewModel viewModel)
        {
            try
            {
                User currentUser = await GetDataUser();

                // Validar que el usuario tenga permisos para poder crear un usuario. Solos Admin pueden crear
                Role adminRole = (await _repositoryRole.Obtener()).FirstOrDefault(x => x.name == "admin");

                if (adminRole != null && currentUser.role != adminRole.id)
                    throw new ApplicationException("No tienes permisos para registrar un nuevo usuario");

                // Creamos el nuevo usuario
                var newUSer = new User() { username = viewModel.Username, email = viewModel.Email, name = viewModel.Name, role = viewModel.role };

                // generamos ApiKey para que poder acceder a la API
                newUSer.apiKey = _tokenService.GenerateApiKey();

                var result = await _userManager.CreateAsync(newUSer, password: viewModel.Password);

                // SI no se pudo crear el usuario
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

        /**
         * Edita un usuario
         * @param viewModel Modelo de vista de usuario
         */
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

        /**
         * Inicia sesión de un usuario
         * @param viewModel Modelo de vista de login
         */
        public async Task LoginUser(LoginViewModel viewModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);

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

        /**
         * Cierra la sesión de un usuario
         */
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

        /**
         * Cambia la contraseña de un usuario
         * @param viewModel Modelo de vista de usuario
         */
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

        //****************************************************
        //********************** ROLE ************************
        //****************************************************

        public async Task<RoleViewModel> GetRolesViewModel()
        {
            try
            {
                var roles = (await _repositoryRole.Obtener()).ToList();

                PasarMayusculas(ref roles);

                return new RoleViewModel()
                {
                    Roles = roles.ToList(),
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task CreateRole(RoleViewModel viewModel)
        {
            try
            {
                var user = await GetDataUser();

                if(user.role != 1)
                    throw new ApplicationException("No tienes permisos para crear un nuevo rol");

                var role = _mapper.Map<Role>(viewModel);

                role.name = role.name.ToLower();

                await _repositoryRole.Crear(role);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void PasarMayusculas(ref List<Role> roles)
        {
            foreach (var rol in roles)
            {
                rol.name = rol.name.ToUpper();
            }
        }

        public async Task DeleteRole(int id)
        {
            try
            {
                var user = await GetDataUser();
                if (user.role != 1)
                    throw new ApplicationException("No tienes permisos para eliminar un rol");

                var sePuedeBorrar = await _repositoryRole.SePuedeBorrar(id);

                if (!sePuedeBorrar)
                    throw new ApplicationException("No se puede eliminar el rol");

                await _repositoryRole.Eliminar(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task EditRole(RoleViewModel viewModel)
        {
            try
            {
                var user = await GetDataUser();
                if (user.role != 1)

                    throw new ApplicationException("No tienes permisos para editar un rol");

                var role = _mapper.Map<Role>(viewModel);

                await _repositoryRole.Editar(role);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Role> GetRoleById(int id)
        {
            try
            {
                var user = await GetDataUser();
                if (user.role != 1)
                    throw new ApplicationException("No tienes permisos para obtener un rol");

                var role = await _repositoryRole.BuscarPorId(id);

                if (role == null)
                    throw new ApplicationException("Rol no encontrado");

                return role;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task VerifyRole(string role)
        {
            try
            {
                var result = await _repositoryRole.Existe(role);

                if (result)
                    throw new ApplicationException("El rol ya existe. Intente con otro nombre.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //*********************** API ************************
        //****************************************************

        public async Task<User> GetUserByApiKey(string apiKey)
        {
            try
            {
                var user = await _repositoryUsers.ObtenerUsuarioPorApiKey(apiKey);

                if (user == null)
                    throw new ApplicationException("Usuario no encontrado");

                return user;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************

        /**
         * Verifica si un email ya existe
         * @param email Email
         */
        public async Task VerifyEmail(string email)
        {
            try
            {
                var user = await GetDataUser();

                if(user.emailNormalizado == email.ToUpper())
                    return;

                var existEmail = await _repositoryUsers.ExistEmail(email);

                if (existEmail)
                    throw new ApplicationException($"El email \"{email}\" ya existe!\nIntente con otro.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /**
         * Verifica la contraseña de un usuario
         * @param pass Contraseña
         */
        public async Task verifyPassword(string pass)
        {
            try
            {
                var userData = await GetDataUser();
                var existePass = await _userManager.CheckPasswordAsync(userData, pass);

                if (existePass)
                    throw new Exception("La contrasela es incorrecta");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Verifica la nueva contraseña de un usuario
         * @param newPass Nueva contraseña
         * @param repeatNewPass Repetir nueva contraseña
         */
        public async Task verifyNewPassword(string newPass, string repeatNewPass)
        {
            try
            {

                if (newPass != repeatNewPass)
                    throw new Exception("Las contraseñas no coinciden");

                var userData = await GetDataUser();

                var samePass = await _userManager.CheckPasswordAsync(userData, newPass);

                if (samePass)
                    throw new Exception("La nueva contraseña no puede ser igual a la anterior");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }

}
