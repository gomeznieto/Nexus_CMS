using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.Controllers
{
    public class FormatsController : Controller
    {
        private readonly IRepositoryFormat _repositoryFormat;

        public FormatsController(IRepositoryFormat repositoryFormat)
        {
            _repositoryFormat = repositoryFormat;
        }

        public async Task<IActionResult> Index()
        {
            var formats = await _repositoryFormat.Obtener();

            return View(formats);
        }
    }
}
