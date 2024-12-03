using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryRole _repositoryRole;
        private readonly SignInManager<User> _signInManager;

        public UsersController(
            UserManager<User> userManager, 
            IRepositoryRole repositoryRole,
            SignInManager<User> signInManager
            )
        {
            _userManager = userManager;
            _repositoryRole = repositoryRole;
            _signInManager = signInManager;
        }

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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
