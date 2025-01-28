using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(
            IRoleService roleService,
            IUsersService userService
            )
        {
            _roleService = roleService;
        }

        //****************************************************
        //********************** INDEX ***********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            try
            {
                var viewModel = await _roleService.GetRolesViewModel();
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
        public async Task<IActionResult> Roles(RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            try
            {
                await _roleService.CreateRole(viewModel);
                Session.CrearModalSuccess("Se ha creado el rol con éxito", "Users", HttpContext);
                return RedirectToAction("Roles");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Roles");
            }
        }

        //****************************************************
        //*********************** DELETE *********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> BorrarRole(int id)
        {
            try
            {
                await _roleService.DeleteRole(id);
                return Json(new { error = false, mensaje = "¡El rol ha sido borrado correctamente!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }
        }

        //****************************************************
        //*********************** EDITAR *********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> EditarRole(RoleViewModel viewModel)
        {
            try
            {
                await _roleService.EditRole(viewModel);
                Session.CrearModalSuccess("El rol ha sido modificado exitosamente", "Users", HttpContext);
                return RedirectToAction("Roles");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        //****************************************************
        //********************** FUNCIONES *******************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> ObtenerRol(int id)
        {
            try
            {
                var role = await _roleService.GetRoleById(id);
                return Json(new { error = false, role });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }

        }

        public async Task<IActionResult> VerificarExisteRole(string name)
        {
            try
            {
                await _roleService.VerifyRole(name);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

    }
}
