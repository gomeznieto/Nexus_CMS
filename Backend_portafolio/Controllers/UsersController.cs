
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(
            IUsersService usersService
        )
        {
            _usersService = usersService;
        }


        //****************************************************
        //********************* REGISTER *********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Register()
        {
                var viewModel = await _usersService.GetRegisterViewModel();
                return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _usersService.GetRegisterViewModel(viewModel);
                Session.CrearModalError(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault(), "Users", HttpContext);
                return View(viewModel);
            }

            try
            {
                await _usersService.CreateUser(viewModel);
                Session.CrearModalSuccess("El Usuario ha sido Creado exitosamente", "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        //****************************************************
        //********************** LOGIN ***********************
        //****************************************************

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
                return View(viewModel);

            try
            {
                await _usersService.LoginUser(viewModel);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return View(viewModel);
            }
        }


        //****************************************************
        //********************** LOGOUT **********************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _usersService.LogoutUser();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //********************* CONFIGURE ********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Configuracion()
        {
            try
            {
                //var viewModel = await _usersService.GetUserViewModel();
                var viewModel = await _usersService.GetPasswordViewModel();

                return View(viewModel);

            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }

        }

        //****************************************************
        //********************* LISTADO **********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Users(string buscar, int role, int page = 1)
        {
            try
            {
                var viewModel = new UserDataListViewModel();

                if (buscar != null || role != 0)
                    viewModel = await _usersService.SearchUser(buscar, role, page);
                else
                    viewModel = await _usersService.GetUserDataList(page);

                ViewBag.Message = "No hay entradas para mostrar";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //********************* PROFILE **********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                var viewModel = await _usersService.GetUserViewModel();
                return View(viewModel);

            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Perfil(UserViewModel ViewModel)
        {

            if (!ModelState.IsValid)
            {
                ViewModel = await _usersService.GetUserViewModel();
                return View(ViewModel);
            }

            try
            {
                await _usersService.EditUser(ViewModel);
                Session.CrearModalSuccess("El perfil ha sido guardado exitosamente", "Users", HttpContext);
                return RedirectToAction("Perfil");
            }
            catch (Exception argument)
            {
                Session.CrearModalError(argument.Message, "Users", HttpContext);
                return RedirectToAction("Perfil");
            }
        }


        //****************************************************
        //********************* APIKEY ***********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult>Apikey()
        {
            try
            {
                var viewModel = await _usersService.GetUserViewModel();
                return View(viewModel);

            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //************* EDIT REGISTER BY ADMIN  **************
        //****************************************************

        [HttpPost]
        public async Task<IActionResult>EditRole(UserDataListViewModel viewModel)
        {
            try
            {
                await _usersService.EditUserByAdmin(viewModel);
                Session.CrearModalSuccess("El Usuario ha sido editado exitosamente", "Users", HttpContext);
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {

                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Users");
            }
        }


        //****************************************************
        //******************** EDIT PASS *********************
        //****************************************************
        [HttpPost]
        public async Task<IActionResult> EditarPass(PasswordViewModel viewModel)
        {

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                await _usersService.ChangePassword(viewModel);
                Session.CrearModalSuccess("La contraseña ha sido cambiada exitosamente", "Users", HttpContext);
                return RedirectToAction("Configuracion");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "Users", HttpContext);
                return RedirectToAction("Configuracion");
            }
        }


        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************


        // Verificar si el Email existe
        [HttpGet]
        public async Task<IActionResult> VerificarExisteEmail(string email)
        {
            try
            {
                await _usersService.VerifyEmail(email);

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        // Verificar si la contraseña actual es correcta
        [HttpGet]
        public async Task<IActionResult> VerficarExistePass(string password)
        {
            try
            {
                var result = await _usersService.verifyPassword(password);

                if (!result)
                    throw new Exception("La contrasela es incorrecta");

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // Verificar si la nueva contraseña es diferente a la actual
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerificarPassNuevo(string passwordNuevo, string repetirPasswordNuevo)
        {
            try
            {
                await _usersService.verifyNewPassword(passwordNuevo, repetirPasswordNuevo);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
