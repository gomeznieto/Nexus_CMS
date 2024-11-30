using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryRole _repositoryRole;

        public UsersController(UserManager<User> userManager, IRepositoryRole repositoryRole)
        {
            _userManager = userManager;
            _repositoryRole = repositoryRole;
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
    }
}
