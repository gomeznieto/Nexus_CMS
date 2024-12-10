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

        public UsersController(
            UserManager<User> userManager, 
            IRepositoryRole repositoryRole,
            IRepositoryBio repositoryBio,
            SignInManager<User> signInManager
            )
        {
            _userManager = userManager;
            _repositoryRole = repositoryRole;
            _repositoryBio = repositoryBio;
            _signInManager = signInManager;
        }

        // REGISTER
        [HttpGet]
        public async Task<IActionResult> Register() {

            RegisterViewModel viewModel = new RegisterViewModel();

            // Obtener roles
            viewModel.roles = (await _repositoryRole.Obtener()).ToList();

            return View(viewModel); 
        }

        [HttpPost]
        public async Task<IActionResult>Register(RegisterViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                viewModel.roles = (await _repositoryRole.Obtener()).ToList();

                return View(viewModel);
            }

            var usuario = new User(){ email = viewModel.Email, name = viewModel.Name, role = viewModel.role  };

            var result = await _userManager.CreateAsync(usuario, password: viewModel.Password);

            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("index", "home");
            }
            else
            {
                foreach(var error in result.Errors)
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
        public async Task<IActionResult>Login(LoginViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);

            if(result.Succeeded)
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
    }
}
