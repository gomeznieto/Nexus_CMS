using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class NetworkController : Controller
    {
        private readonly INetworkService _networkService;

        public NetworkController(
            INetworkService networkService
            )
        {
            _networkService = networkService;
        }

        //****************************************************
        //********************* REDES SOCIALES ****************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Redes()
        {
            try
            {
                var viewModel = await _networkService.GetSocialNetworkViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        //****************************************************
        //*********************** CREATE *********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> Redes(SocialNetworkViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                await _networkService.CreteSocialNetwork(viewModel);
                Session.CrearModalSuccess("Se ha creado la Red Social con éxito", "Users", HttpContext);
                return RedirectToAction("Redes");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        //****************************************************
        //*********************** EDITAR *********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> EditarRedes(SocialNetworkViewModel viewModel)
        {
            try
            {
                await _networkService.EditSocialNetwork(viewModel);
                Session.CrearModalSuccess("La red social ha sido modificada exitosamente", "Users", HttpContext);
                return RedirectToAction("Redes");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");

            }
        }

        //****************************************************
        //*********************** BORRAR *********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> BorrarRedes(int id)
        {
            try
            {
                await _networkService.DeleteSocialNetwork(id);
                return Json(new { error = false, mensaje = "¡La bio ha sido borrada correctamente!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }
        }

        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************


        [HttpGet]
        public async Task<IActionResult> ObtenerRedes(int id)
        {
            try
            {
                var bio = await _networkService.GetSocialNetworkById(id);
                return Json(new { error = false, bio });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }

        }

    }
}
