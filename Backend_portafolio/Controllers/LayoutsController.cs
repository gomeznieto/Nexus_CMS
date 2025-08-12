using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Backend_portafolio.Controllers
{
    public class LayoutsController : Controller
    {
        private readonly ILayoutService _layoutService;
        private readonly IUsersService _usersService;

        public LayoutsController(
            ILayoutService layoutService,
            IUsersService usersService
            )
        {
            _layoutService = layoutService;
            _usersService = usersService;
        }

        // -- GET ---
        [HttpGet]
        public async Task<IActionResult> HomeLayout()
        {
            try
            {
                var currentUser = await _usersService.GetDataUser();
                UserHomeLayoutFormModel model = await _layoutService.GetLayoutForm(currentUser.id);
                return View(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> HomeLayout(UserHomeLayoutFormModel model)
        {
            try
            {
                var currentUser = await _usersService.GetDataUser();
                await _layoutService.SaveLayoutForm(model, currentUser.id);
                return RedirectToAction("HomeLayout");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> BorrarHomeLayout (int id)
        {
            try
            {
                await _layoutService.DeleteHomeLayoutSection(id);
                return RedirectToAction("HomeLayout");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}