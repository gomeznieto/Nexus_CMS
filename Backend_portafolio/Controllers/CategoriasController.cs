using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Backend_portafolio.Sevices;

namespace Backend_portafolio.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositoryCategorias _repositoryCategorias;
        private readonly IUsersService _usersService;
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(
            IRepositoryCategorias repositoryCategorias,
            IUsersService usersService,
            ICategoriaService categoriaService
            )
        {
            _repositoryCategorias = repositoryCategorias;
            _usersService = usersService;
            _categoriaService = categoriaService;
        }

        //****************************************************
        //********************** INDEX **********************
        //****************************************************
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1) //TODO: PAGINACION
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                var categorias = await _categoriaService.GetAllCategorias(userID);

                ViewBag.Cantidad = categorias.Count();
                ViewBag.Message = $"No hay categorias para mostrar.";

                return View(categorias.OrderBy(x => x.name).ToList());
            }
            catch (Exception)
            {
                Session.CrearModalError("Ha surgido un error. ¡Intente más tarde!", "home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(string buscar)
        {
            try
            {
                var categorias = await _categoriaService.GetCategoryByName(buscar);

                ViewBag.Cantidad = categorias.Count();
                ViewBag.Message = $"Sin resultados para \"{buscar}\".";

                return View(categorias);
            }
            catch (Exception)
            {
                Session.CrearModalError("Ha surgido un error. ¡Intente más tarde!", "home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        [HttpGet]
        public IActionResult Crear()
        {
            var viewModel = _categoriaService.GetViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            try
            {
                await _categoriaService.CreateCategories(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }


        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var categoriaCambiar = await _categoriaService.GetCategoriaById(id);
                return View(categoriaCambiar);
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "home", HttpContext);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            var userID = _usersService.ObtenerUsuario();

            //Validar errores del Model
            if (!ModelState.IsValid)
                return View(categoria);

            try
            {
                await _categoriaService.EditCategory(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session.CrearModalError(ex.Message, "home", HttpContext);
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
                await _categoriaService.DeleteCategory(id);
                return Json(new { error = false, mensaje = "Borrado con Éxito" });
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
        public async Task<IActionResult> VerificarExisteCategoria(string name)
        {
            try
            {
               await _categoriaService.Existe(name);
                return Json(true);
            }
            catch(Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message });
            }

        }
    }
}
