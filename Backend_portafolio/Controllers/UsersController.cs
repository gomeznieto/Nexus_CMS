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

        public UsersController(
            UserManager<User> userManager,
            IRepositoryRole repositoryRole,
            IRepositoryBio repositoryBio,
            SignInManager<User> signInManager,
            IUsersService usersService
            )
        {
            _userManager = userManager;
            _repositoryRole = repositoryRole;
            _repositoryBio = repositoryBio;
            _signInManager = signInManager;
            _usersService = usersService;
        }

        // REGISTER
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

            var usuario = new User() { email = viewModel.Email, name = viewModel.Name, role = viewModel.role };

            var result = await _userManager.CreateAsync(usuario, password: viewModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: true);
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

        // LOGIN
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

        // LOGOUT
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Configuración
        [HttpGet]
        public async Task<IActionResult> Configuracion()
        {
            User user = await _signInManager.UserManager.GetUserAsync(User);

            UserViewModel viewModel = new UserViewModel();
            viewModel.CV = user.cv;
            viewModel.Email = user.email;
            viewModel.Name = user.name;
            viewModel.Image = user.img;
            viewModel.RoleName = (await _repositoryRole.Obtener()).Where(x => x.id == user.role).Select(x => x.name).FirstOrDefault();

            return View(viewModel);
        }

        // PERFIL

        // Configuración
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            User user = await _signInManager.UserManager.GetUserAsync(User);

            UserViewModel viewModel = new UserViewModel();
            viewModel.CV = user.cv;
            viewModel.Email = user.email;
            viewModel.Name = user.name;
            viewModel.Image = user.img;
            viewModel.Bios = (await _repositoryBio.Obtener(user.Id)).ToList();
            viewModel.RoleName = (await _repositoryRole.Obtener()).Where(x => x.id == user.role).Select(x => x.name).FirstOrDefault();

            return View(viewModel);
        }

        // BIO

        [HttpGet]
        public async Task<IActionResult> Bio()
        {
            var usuarioID = _usersService.ObtenerUsuario();
            BioViewModel viewModel = new BioViewModel();
            viewModel.Bios = (await _repositoryBio.Obtener(usuarioID)).OrderByDescending(x => x.year).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Bio(Bio bio)
        {
            if (!ModelState.IsValid)
            {
                return View(bio);
            }

            var usuarioID = _usersService.ObtenerUsuario();

            bio.user_id = usuarioID;
            await _repositoryBio.Agregar(bio);
            return RedirectToAction("Bio");
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();

                var bio = await _repositoryBio.ObtenerPorId(id, usuarioID);

                if (bio == null)
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

                return RedirectToAction("Bio");
            }
            catch (Exception)
            {
                //Crear mensaje de error para modal
                var errorModal = new ModalViewModel { message = "Ha surgido un error. ¡Intente más tarde!", type = true, path = "Users" };
                Session.ErrorSession(HttpContext, errorModal);

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


    }
}
