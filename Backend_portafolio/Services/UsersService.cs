using Backend_portafolio.Datos;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using System.Data;
using Backend_portafolio.Helper;
using Backend_portafolio.Entities;


namespace Backend_portafolio.Services
{
    public interface IUsersService
    {
        int ObtenerUsuario();
        Task ChangePassword(PasswordViewModel viewModel);
        Task CreateUser(RegisterViewModel viewModel);
        Task EditUser(UserViewModel viewModel);
        Task LoginUser(LoginViewModel viewModel);
        Task LogoutUser();
        Task VerifyEmail(string email);
        Task verifyNewPassword(string newPass, string repeatNewPass);
        Task<bool> verifyPassword(string pass);
        Task<RegisterViewModel> GetRegisterViewModel(RegisterViewModel viewModel = null);
        Task<UserViewModel> GetDataUser();
        Task<UserViewModel> GetUserByApiKey(string apiKey);
        Task<UserViewModel> GetUserByUser(string username);
        Task<UserViewModel> GetUserViewModel(UserViewModel userViewModel = null);
        Task<int> GetTotalCountUsers();
        Task<UserDataListViewModel> SearchUser(string search, int role = 0, int page = 1);
        Task<List<UserViewModel>> ObtenerUsuarios(int page);
        Task<List<UserViewModel>> ObtenerUsuarios();
        Task<UserDataListViewModel> GetUserDataList(int page = 1);
        Task EditUserByAdmin(UserDataListViewModel viewModel);
        Task CreateAdminUser();
        Task<PasswordViewModel> GetPasswordViewModel();
    }

    public class UsersService : IUsersService
    {
        private readonly HttpContext _httpContext;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IRepositoryRole _repositoryRole;
        private readonly IRepositoryUsers _repositoryUsers;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<UserViewModel> _signInManager;
        private readonly UserManager<UserViewModel> _userManager;

        public UsersService(
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService,
            IMapper mapper,
            IRepositoryRole repositoryRole,
            IRepositoryUsers repositoryUsers,
            ITokenService tokenService,
            SignInManager<UserViewModel> MySignInManager,
            UserManager<UserViewModel> userManager
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

        public async Task<List<UserViewModel>> ObtenerUsuarios(int page)
        {
            try
            {
                await VerifyAdmin();

                //crear session de cantidad de post en caso de no haber sido ya creada
                if (Session.GetCantidadUsersSession(_httpContext) == -1)
                {
                    Session.CantidadUsersSession(_httpContext, 10);
                }

                var cantidadPorPagina = Session.GetCantidadUsersSession(_httpContext);

                var users = await _repositoryUsers.GetUsers(page, cantidadPorPagina);

                return _mapper.Map<List<UserViewModel>>(users);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserViewModel>> ObtenerUsuarios()
        {
            try
            {
                await VerifyAdmin();

                //crear session de cantidad de post en caso de no haber sido ya creada
                if (Session.GetCantidadUsersSession(_httpContext) == -1)
                {
                    Session.CantidadUsersSession(_httpContext, 10);
                }

                var cantidadUsuarios = await _repositoryUsers.CountAllUsers();
                var cantidadPorPagina = Session.GetCantidadUsersSession(_httpContext);

                var users = _mapper.Map<List<UserViewModel>>(await _repositoryUsers.GetUsers(1, cantidadUsuarios));
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
        public async Task<UserViewModel> GetDataUser()
        {
            try
            {
                UserViewModel user = (await _signInManager.UserManager.GetUserAsync(_httpContext.User));
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
                var roles = _mapper.Map<List<RoleViewModel>>((await _repositoryRole.Obtener()).ToList());
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
        public async Task<UserViewModel> GetUserViewModel(UserViewModel userViewModel = null)
        {
            try
            {
                var user = await GetDataUser();
                //Mappear el usuario
                if (userViewModel == null)
                    userViewModel = _mapper.Map<UserViewModel>(user);

                //Obtener roles
                var roles = (await _repositoryRole.Obtener()).ToList();

                userViewModel.RoleName = roles.Where(x => x.id == user.role).Select(x => x.name).FirstOrDefault();
                userViewModel.RolesName = roles;

                return userViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDataListViewModel> GetUserDataList(int page = 1)
        {
            try
            {

                var userDataListViewModel = new UserDataListViewModel();
                userDataListViewModel.totalUserList = (await ObtenerUsuarios()).Select(x => _mapper.Map<UserViewModel>(x)).ToList();
                var cantidadPorPagina = Session.GetCantidadUsersSession(_httpContext);
                userDataListViewModel.countUsers = userDataListViewModel.totalUserList.Count;
                userDataListViewModel.usersList = userDataListViewModel.totalUserList.Skip(cantidadPorPagina * (page - 1)).Take(cantidadPorPagina).ToList();

                var roles = (await _repositoryRole.Obtener()).ToList();

                userDataListViewModel.roles = roles;

                return userDataListViewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PasswordViewModel> GetPasswordViewModel()
        {
            try
            {
                var userID = ObtenerUsuario();
                var viewModel = new PasswordViewModel()
                {
                    id = userID
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDataListViewModel> SearchUser(string search, int role = 0, int page = 1)
        {
            try
            {
                var cantidadPorPagina = Session.GetCantidadUsersSession(_httpContext);

                var roles = (await _repositoryRole.Obtener()).ToList();
                var allUsers = await ObtenerUsuarios();

                var userViewModel = new UserDataListViewModel();
                userViewModel.roles = roles;
                userViewModel.totalUserList = allUsers;

                // Usuario por Palabra
                if (search != null)
                {
                    var usersSearched = userViewModel.totalUserList
                        .Where(
                        x => x.username.ToLower().Contains(search.ToLower()) ||
                                x.name.ToLower().Contains(search.ToLower()) ||
                                x.email.ToLower().Contains(search.ToLower())
                        )
                        .ToList();
                    userViewModel.totalUserList = usersSearched;
                }

                //Usuario por Role
                if (role != 0)
                {
                    var usersSearched = userViewModel.totalUserList.Where(x => x.role == role).ToList();
                    userViewModel.totalUserList = usersSearched;
                }

                userViewModel.countUsers = userViewModel.totalUserList.Count;
                userViewModel.usersList = userViewModel.totalUserList.Skip(cantidadPorPagina * (page - 1)).Take(cantidadPorPagina).ToList();

                return userViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserViewModel> GetUserByUser(string username)
        {
            try
            {
                var user = await _repositoryUsers.BuscarUsuarioPorUsername(username);
                return _mapper.Map<UserViewModel>(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalCountUsers()
        {
            try
            {
                await VerifyAdmin();
                var count = await _repositoryUsers.CountAllUsers();
                return count;
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
                UserViewModel currentUser = await GetDataUser();

                // Validar que el usuario tenga permisos para poder crear un usuario. Solos Admin pueden crear
                RoleViewModel adminRole = _mapper.Map<RoleViewModel>((await _repositoryRole.Obtener()).FirstOrDefault(x => x.name == "admin"));

                if (adminRole != null && currentUser.role != adminRole.id)
                    throw new ApplicationException("No tienes permisos para registrar un nuevo usuario");

                // Creamos el nuevo usuario
                var newUser = new UserViewModel() { username = viewModel.Username, email = viewModel.Email, name = viewModel.Name, role = viewModel.role };

                // generamos ApiKey para que poder acceder a la API
                newUser.apiKey = _tokenService.GenerateApiKey();

                var result = await _userManager.CreateAsync(newUser, password: viewModel.Username + ".pass"); //Variable de entorno

                // SI no se pudo crear el usuario
                if (!result.Succeeded)
                    throw new ApplicationException("Error al registrar el usuario");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateAdminUser()
        {
            var provisoryPassword = "123456"; //Variable de entorno

            try
            {
                var countUsers = await _repositoryUsers.CountAllUsers();
                var adminRole = await _repositoryRole.BuscarPorNombre("admin");

                if (countUsers > 0)
                    return;

                var newUser = new UserViewModel();
                newUser.username = "admin";
                newUser.email = "";
                newUser.name = "admin";
                newUser.role = adminRole.id;
                var result = await _userManager.CreateAsync(newUser, password: provisoryPassword);
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
                UserViewModel currentUser = await GetDataUser();
                UserViewModel userEdit = _mapper.Map<UserViewModel>(viewModel);

                userEdit.img = viewModel.ImageFile != null
                    ? await _imageService.UploadImageAsync(viewModel.ImageFile, currentUser, "profile-images")
                    : currentUser.img;

                userEdit.passwordHash = userEdit.passwordHash == null ? currentUser.passwordHash : userEdit.passwordHash;


                var result = await _userManager.UpdateAsync(userEdit);

                if (!result.Succeeded)
                    throw new ApplicationException("Error al editar el usuario");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task EditUserByAdmin(UserDataListViewModel viewModel)
        {
            try
            {
                var user = await _repositoryUsers.BuscarPorId(viewModel.id);

                // Editar Role
                if(user.role != viewModel.role)
                {
                    user.role = viewModel.role;
                    await _repositoryUsers.EditarUsuario(user);
                }

                //Reestablecer la contraseña
                if(viewModel.recoveryPass)
                {
                    var newPassword = _userManager.PasswordHasher.HashPassword(_mapper.Map<UserViewModel>(user), user.username + ".pass");
                    user.passwordHash = newPassword;
                    await _repositoryUsers.EditarUsuario(user);
                }

                //Reestablecer la ApiKey
                if(viewModel.recoveryApikey)
                {
                    user.apiKey = _tokenService.GenerateApiKey();
                    await _repositoryUsers.EditarUsuario(user);
                }


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
        public async Task ChangePassword(PasswordViewModel viewModel)
        {
            try
            {
                UserViewModel currentUser = await GetDataUser();

                // Validar que el usuario exista
                if (currentUser == null)
                    throw new ApplicationException("Usuario no encontrado");

                if(viewModel.id != currentUser.id)
                    throw new ApplicationException("No puede modificar la contraseña de otro usuario");

                // Validar la contraseña actual
                var isValidPassword = await _userManager.CheckPasswordAsync(currentUser, viewModel.password);

                if (!isValidPassword)
                    throw new ApplicationException("Contraseña incorrecta");

                // Editamso la pass
                bool result = await _repositoryUsers.EditarPass(_mapper.Map<User>(currentUser), viewModel.passwordNuevo);

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
        //*********************** API ************************
        //****************************************************

        public async Task<UserViewModel> GetUserByApiKey(string apiKey)
        {
            try
            {
                var user = await _repositoryUsers.ObtenerUsuarioPorApiKey(apiKey);

                if (user == null)
                    throw new ApplicationException("Usuario no encontrado");

                return _mapper.Map<UserViewModel>(user);

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
         * Pasa a mayúsculas los nombres de los roles
         * @param roles Lista de roles
         */
        private void PasarMayusculas(ref List<RoleViewModel> roles)
        {
            foreach (var rol in roles)
            {
                rol.name = rol.name.ToUpper();
            }
        }

        /**
         * Verifica si un email ya existe
         * @param email Email
         */
        public async Task VerifyEmail(string email)
        {
            try
            {
                var user = await GetDataUser();

                if (user.emailNormalizado == email.ToUpper())
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
        public async Task<bool> verifyPassword(string pass)
        {
            try
            {
                var userData = await GetDataUser();
                return await _userManager.CheckPasswordAsync(userData, pass);

                
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

        private async Task VerifyAdmin()
        {
            try
            {
                var user = await GetDataUser();
                var roles = await _repositoryRole.BuscarPorId(user.role);

                if (roles.name.ToLower() != "admin")
                    throw new ApplicationException("No se pudo obtener el usuario");
            }
            catch (Exception)
            {

                throw new ApplicationException("No se pudo obtener el usuario");
            }
        }

    }

}
