using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_portafolio.Models;
using Backend_portafolio.Services;
using Microsoft.AspNetCore.Mvc;


namespace Backend_portafolio.Controllers
{
    public class LayoutsController : Controller
    {
        private readonly ILayoutService _layoutService;
        public LayoutsController(ILayoutService layoutService)
        {
            _layoutService = layoutService;

        }

        // -- GET ---
        [HttpGet]
        public async Task<IActionResult> HomeLayout()
        {
            try
            {
                UserHomeLayoutFormModel model = await _layoutService.GetLayoutForm();
                return View(model);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}