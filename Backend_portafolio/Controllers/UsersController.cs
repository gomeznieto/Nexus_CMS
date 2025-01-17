using AutoMapper;
using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUsersService _usersService;
        private readonly IRepositoryUsers _repositoryUsers;
        private readonly IRepositorySocialNetwork _repositorySocialNetwork;
        private readonly IBioService _bioService;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<User> userManager,
            IRepositoryRole repositoryRole,
            IRepositoryBio repositoryBio,
            SignInManager<User> signInManager,
            IUsersService usersService,
            IRepositoryUsers repositoryUsers,
            IRepositorySocialNetwork repositorySocialNetwork,
            IImageService imageService,
            IBioService bioService,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _usersService = usersService;
            _repositoryUsers = repositoryUsers;
            _repositorySocialNetwork = repositorySocialNetwork;
            _bioService = bioService;
            _mapper = mapper;
        }


        //****************************************************
        //********************* REGISTER *********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var viewModel = await _usersService.GetRegisterViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _usersService.GetRegisterViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                await _usersService.CreateUser(viewModel);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        //****************************************************
        //********************** LOGIN ***********************
        //****************************************************

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                await _usersService.LoginUser(viewModel);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return View(viewModel);
            }
        }


        //****************************************************
        //********************** LOGOUT **********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _usersService.LogoutUser();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //********************* CONFIGURE ********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Configuracion()
        {
            try
            {
                var viewModel = await _usersService.GetUserViewModel();
                return View(viewModel);

            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }

        }


        //****************************************************
        //********************* PROFILE **********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                var viewModel = await _usersService.GetUserViewModel();
                return View(viewModel);

            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //*********************** BIO ************************
        //****************************************************
        [HttpGet]
        public async Task<IActionResult> Bio()
        {
            try
            {
                var viewModel = await _bioService.GetBioViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Bio(BioViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _bioService.GetBioViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                await _bioService.CreateBio(viewModel);
                Session.CrearModalSuccess("Se ha creado la bio con éxito", "Users", HttpContext);
                return RedirectToAction("Bio");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                await _bioService.DeleteBio(id);
                return Json(new { error = false, mensaje = "¡La bio ha sido borrada correctamente!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(BioViewModel viewmodel)
        {
            if (!ModelState.IsValid)
                return View(viewmodel);

            try
            {
                await _bioService.EditBio(viewmodel);
                Session.CrearModalSuccess("Se ha modificado la bio con éxito", "Users", HttpContext);
                return RedirectToAction("Bio");
            }
            catch (Exception)
            {
                Session.CrearModalError("Ha surgido un error. ¡Intente más tarde!", "Users", HttpContext);
                return RedirectToAction("Bio");
            }
        }


        //****************************************************
        //*********************** USER ***********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> EditarUser(UserViewModel ViewModel)
        {
            try
            {
                await _usersService.EditUser(ViewModel);
                Session.CrearModalSuccess("El perfil ha sido guardado exitosamente", "Users", HttpContext);
                return RedirectToAction("Perfil");
            }
            catch (Exception argument)
            {
                Session.CrearModalError(argument.Message, "Users", HttpContext);
                return RedirectToAction("Perfil");
            }
        }

        //****************************************************
        //*********************** PASS ***********************
        //****************************************************
        [HttpPost]
        public async Task<IActionResult> EditarPass(UserViewModel viewModel)
        {

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                await _usersService.ChangePassword(viewModel);
                Session.CrearModalSuccess("La contraseña ha sido cambiada exitosamente", "Users", HttpContext);
                return RedirectToAction("Configuracion");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Configuracion");
            }
        }

        //****************************************************
        //********************* NETWORKS *********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Redes()
        {
            //Usuario
            var usuarioID = _usersService.ObtenerUsuario();

            // Redes
            var viewModel = new SocialNetworkViewModel();
            viewModel.Networks = (await _repositorySocialNetwork.ObtenerPorUsuario(usuarioID)).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Redes(SocialNetworkViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var usuarioID = _usersService.ObtenerUsuario();

                viewModel.user_id = usuarioID;
                await _repositorySocialNetwork.Agregar(viewModel);

                Session.CrearModalSuccess("Se ha creado la Red Social con éxito", "Users", HttpContext);

                return RedirectToAction("Redes");
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                Session.CrearModalError("Ha surgido un error. ¡Intente más tarde!", "Users", HttpContext);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRedes(int id)
        {
            var usuarioID = _usersService.ObtenerUsuario();
            var bio = await _repositorySocialNetwork.ObtenerPorId(id, usuarioID);

            if (bio == null)
            {
                return Json(new { error = true, mensaje = "La bio no se pudo obtener.\n¡Se ha producido un error!" });
            }

            return Json(new { error = false, bio });

        }

        [HttpPost]
        public async Task<IActionResult> EditarRedes(SocialNetworkViewModel viewModel)
        {
            Entities.SocialNetwork socialNetwork = _mapper.Map(viewModel, new Entities.SocialNetwork());

            var editado = await _repositorySocialNetwork.Editar(socialNetwork);

            if (!editado)
            {
                Session.CrearModalError("La red social no pudo ser modificada. Intente más tarde!", "Users", HttpContext);
                return RedirectToAction("Redes");
            }
            Session.CrearModalSuccess("La red social ha sido modificada exitosamente", "Users", HttpContext);
            return RedirectToAction("Redes");
        }

        [HttpPost]
        public async Task<IActionResult> BorrarRedes(int id)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();

                var bio = await _repositorySocialNetwork.ObtenerPorId(id, usuarioID);

                if (bio == null || usuarioID != bio.user_id)
                {
                    return Json(new { error = true, mensaje = "La bio no se pudo borrar.\n¡Se ha producido un error!" });
                }

                await _repositorySocialNetwork.Borrar(id, usuarioID);

                return Json(new { error = false, mensaje = "¡La bio ha sido borrada correctamente!" });
            }
            catch (Exception)
            {
                return Json(new { error = true, mensaje = "La bio no se pudo borrar.\n¡Se ha producido un error!" });

            }
        }


        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************


        // Verificar si el Email existe
        [HttpGet]
        public async Task<IActionResult> VerificarExisteEmail(string email)
        {
            try
            {
                var existeEmail = await _repositoryUsers.Existe(email);

                if (existeEmail)
                    return Json($"El nombre {email} ya existe!");

                return Json(true);
            }
            catch (Exception)
            {

                return Json($"Se produjo un error al intentear validar {email}. Intente con otro nombre o en otro momento!");
            }

        }

        // Verificar si la contraseña actual es correcta
        [HttpGet]
        public async Task<IActionResult> VerficarExistePass(string password)
        {

            try
            {
                var usuarioID = _usersService.ObtenerUsuario();
                var usuario = await _repositoryUsers.BuscarPorId(usuarioID);

                var existePass = await _userManager.CheckPasswordAsync(usuario, password);

                if (!existePass)
                    return Json($"La contraseña es incorrecta");

                return Json(true);

            }
            catch (Exception)
            {
                return Json($"La contraseña no es correcta");
            }
        }

        // Verificar si la nueva contraseña es diferente a la actual
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerificarPassNuevo(string passwordNuevo, string repetirPasswordNuevo)
        {

            try
            {
                if (passwordNuevo is null || repetirPasswordNuevo is null)
                    return Json("Las constraseñas deben ser iguales");

                var usuarioID = _usersService.ObtenerUsuario();
                var usuario = await _repositoryUsers.BuscarPorId(usuarioID);

                // Verificamos que las constraseñas sean iguales
                if (passwordNuevo != repetirPasswordNuevo)
                    return Json($"La contraseña deben ser iguales");

                // Verificamos que la contraseña sea diferente a la actual
                var existePass = await _userManager.CheckPasswordAsync(usuario, passwordNuevo);
                if (existePass)
                    return Json($"La contraseña debe ser diferente a la actual");

                return Json(true);

            }
            catch (Exception)
            {
                return Json($"La contraseña no es correcta");
            }
        }
    }
}
