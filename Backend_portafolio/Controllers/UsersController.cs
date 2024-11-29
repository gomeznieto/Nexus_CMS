using Backend_portafolio.Models;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Backend_portafolio.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Register() { 

            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult>Register(RegisterViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("index", "home");
        }
    }
}
