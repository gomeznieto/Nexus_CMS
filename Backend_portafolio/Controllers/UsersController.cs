using AutoMapper;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryRole _repositoryRole;
        private readonly IRepositoryBio _repositoryBio;
        private readonly SignInManager<User> _signInManager;
        private readonly IUsersService _usersService;
        private readonly IRepositoryUsers _repositoryUsers;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<User> userManager,
            IRepositoryRole repositoryRole,
            IRepositoryBio repositoryBio,
            SignInManager<User> signInManager,
            IUsersService usersService,
            IRepositoryUsers repositoryUsers,
            IImageService imageService,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _repositoryRole = repositoryRole;
            _repositoryBio = repositoryBio;
            _signInManager = signInManager;
            _usersService = usersService;
            _repositoryUsers = repositoryUsers;
            _imageService = imageService;
            _mapper = mapper;
        }

        /*
         ========================================

        = REGISTRO DE USUARIO

        Unicamente el admin tiene la autorización para poder crear nuevos usuarios

         ========================================
         */

        [HttpGet]
        public async Task<IActionResult> Register()
        {

            RegisterViewModel viewModel = new RegisterViewModel();

            // Obtener roles
            viewModel.roles = (await _repositoryRole.Obtener()).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.roles = (await _repositoryRole.Obtener()).ToList();

                return View(viewModel);
            }

            User user = await _signInManager.UserManager.GetUserAsync(User);
            Role adminRole = (await _repositoryRole.Obtener()).FirstOrDefault(x => x.name == "admin");

            // Verificamos que el Rol sea de Admin para que pueda registrar un nuevo cliente
            if (adminRole != null && user.role != adminRole.id)
            {
                viewModel.roles = (await _repositoryRole.Obtener()).ToList();

                return View(viewModel);
            }

            var usuario = new User() { email = viewModel.Email, name = viewModel.Name, role = viewModel.role };

            var result = await _userManager.CreateAsync(usuario, password: viewModel.Password);

            if (result.Succeeded)
            {
                //await _signInManager.SignInAsync(usuario, isPersistent: true); //Línea de código para loggearse con el user recién creado. Al ser creado solo por el admin esto no serí necesario
                return RedirectToAction("index", "home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(viewModel);
            }

        }

        /*
         ========================================

        = LOGIN

         ========================================
         */

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
            {
                return View(viewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("index", "home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre del usuario o password incorrectos");
                return View(viewModel);
            }
        }


        /*
         ========================================

        = LOGOUT

        Método para salir del usuario

         ========================================
         */

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }


        /*
         ========================================

        = CONFIGURACION

         ========================================
         */

        [HttpGet]
        public async Task<IActionResult> Configuracion()
        {
            User user = await _signInManager.UserManager.GetUserAsync(User);


            UserViewModel viewModel = _mapper.Map<UserViewModel>(user);

            viewModel.RoleName = (await _repositoryRole.Obtener()).Where(x => x.id == user.role).Select(x => x.name).FirstOrDefault();

            return View(viewModel);
        }


        /*
         ========================================

        = PERFIL

         ========================================
         */
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            User user = await _signInManager.UserManager.GetUserAsync(User);
            UserViewModel viewModel = _mapper.Map<User, UserViewModel>(user);
            viewModel.RoleName = (await _repositoryRole.Obtener()).Where(x => x.id == user.role).Select(x => x.name).FirstOrDefault();

            return View(viewModel);
        }


        /*
         ========================================

        = BIO

         ========================================
         */
        [HttpGet]
        public async Task<IActionResult> Bio()
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();
                BioViewModel viewModel = new BioViewModel();
                viewModel.Bios = (await _repositoryBio.Obtener(usuarioID)).OrderByDescending(x => x.year).ToList();

                return View(viewModel);
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Users" };
                Session.ErrorSession(HttpContext, errorModal);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Bio(Bio bio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(bio);
                }

                var usuarioID = _usersService.ObtenerUsuario();

                bio.user_id = usuarioID;
                await _repositoryBio.Agregar(bio);

                Session.CrearModalSuccess("Se ha creado la bio con éxito", "Users", HttpContext);

                return RedirectToAction("Bio");
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Users" };
                Session.ErrorSession(HttpContext, errorModal);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();

                var bio = await _repositoryBio.ObtenerPorId(id, usuarioID);

                if (bio == null || usuarioID != bio.user_id)
                {
                    return Json(new { error = true, mensaje = "La bio no se pudo borrar.\n¡Se ha producido un error!" });
                }

                await _repositoryBio.Borrar(id, usuarioID);

                return Json(new { error = false, mensaje = "¡La bio ha sido borrada correctamente!" });
            }
            catch (Exception)
            {
                return Json(new { error = true, mensaje = "La bio no se pudo borrar.\n¡Se ha producido un error!" });

            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Bio bio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(bio);
                }

                var usuarioID = _usersService.ObtenerUsuario();

                if (usuarioID != bio.user_id)
                {
                    return RedirectToAction("Bio");
                }

                await _repositoryBio.Editar(bio);

                Session.CrearModalSuccess("Se ha modificado la bio con éxito", "Users", HttpContext);

                return RedirectToAction("Bio");
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                Session.CrearModalError("Ha surgido un error. ¡Intente más tarde!", "Users", HttpContext);

                return RedirectToAction("Bio");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerBio(int id)
        {
            var usuarioID = _usersService.ObtenerUsuario();
            var bio = await _repositoryBio.ObtenerPorId(id, usuarioID);

            if (bio == null)
            {
                return Json(new { error = true, mensaje = "La bio no se pudo obtener.\n¡Se ha producido un error!" });
            }

            return Json(new { error = false, bio });
        }

        /*
        ========================================

        = USER

        Edición del perfil del usuario

        ========================================
        */


        [HttpPost]
        public async Task<IActionResult> EditarUser(UserViewModel ViewModel)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();
                var usuario = await _repositoryUsers.BuscarPorId(usuarioID);

                User usuarioEdit = _mapper.Map<User>(ViewModel);

                if (ViewModel.ImageFile != null)
                {
                    usuarioEdit.img = await _imageService.UploadImageAsync(ViewModel.ImageFile, usuario, "profile-images");
                }
                else
                {
                    usuarioEdit.img = usuario.img;
                }

                var result = await _userManager.UpdateAsync(usuarioEdit);

                if (result.Succeeded)
                {
                    //Crear mensaje de error para modal
                    var successModal = new ModalViewModel { message = "El perfil ha sido guardado exitosamente", type = true, path = "Users" };
                    Session.SuccessSession(HttpContext, successModal);

                    return RedirectToAction("Perfil");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                }
                return View(ViewModel);
            }
            catch (ArgumentException argument)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = argument.Message, type = true, path = "Users" };
                Session.ErrorSession(HttpContext, errorModal);

                return RedirectToAction("Perfil");
            }
        }

        /*
         ========================================

        = FUNCIONES

         ========================================
         */


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

    }
}
