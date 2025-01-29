using Backend_portafolio.Entities;
using Backend_portafolio.Helper;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Sevices;
using Backend_portafolio.Models;

namespace Backend_portafolio.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(
            IUsersService usersService,
            ICategoriaService categoriaService
            )
        {
            _categoriaService = categoriaService;
        }

        //****************************************************
        //*********************** INDEX **********************
        //****************************************************

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1) //TODO: PAGINACION
        {
            try
            {
                var categorias = await _categoriaService.GetAllCategorias();

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
        public async Task<IActionResult> Crear(CategoryViewModel categoria)
        {

            if (!ModelState.IsValid)
                return View(categoria);

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
        public async Task<IActionResult> Editar(CategoryViewModel categoria)
        {
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
        public async Task<IActionResult> VerificarExisteCategoria(string name, int user_id)
        {
            try
            {
               await _categoriaService.Existe(name);
                return Json(true);
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }

        }
    }
}
