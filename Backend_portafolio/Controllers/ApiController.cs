using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
	public class ApiController : Controller
	{
        private readonly IRepositoryCategorias _repositoryCateogorias;
        public ApiController(IRepositoryCategorias repositoryCateogorias)
        {
            _repositoryCateogorias = repositoryCateogorias;

		}

        [HttpGet]
        public async Task<IActionResult> Categoria()
        {
            //TODO: Validar Token


            var categorias = await _repositoryCateogorias.Obtener();

            return Json(categorias);
        }

		[HttpGet]
		public async Task<IActionResult> Entrada()
		{
			//TODO: Validar Token

			//TODO: Armar salida con toda la información

			var categorias = await _repositoryCateogorias.Obtener();

			return Json(categorias);
		}

		[HttpGet]
		public async Task<IActionResult> Usuario()
		{
			//TODO: Validar Token


			var categorias = await _repositoryCateogorias.Obtener();

			return Json(categorias);
		}
	}
}
