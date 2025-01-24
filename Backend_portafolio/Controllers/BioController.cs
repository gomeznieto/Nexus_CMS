using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class BioController : Controller
    {
        private readonly IBioService _bioService;

        public BioController(
            IBioService bioService
        )
        {
            _bioService = bioService;
        }

        //****************************************************
        //********************** INDEX ***********************
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


        //****************************************************
        //*********************** CREATE *********************
        //****************************************************

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

        //****************************************************
        //********************** DELETE **********************
        //****************************************************

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

        //****************************************************
        //********************** EDIT ************************
        //****************************************************

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
        //********************* FUNCIONES ********************
        //****************************************************
        [HttpGet]
        public async Task<IActionResult> ObtenerBio(int id)
        {
            try
            {
                var bio = await _bioService.GetBioById(id);
                return Json(new { error = false, bio });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }

        }

    }
}
