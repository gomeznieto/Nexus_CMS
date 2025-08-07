using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc;


namespace Backend_portafolio.Controllers
{
    public class LayoutsController : Controller
    {
        public LayoutsController()
        {

        }

        // -- GET ---
        [HttpGet]
        public async Task<IActionResult> HomeLayout()
        {
            try
            {
                //UserHomeLayoutFormModel model = 
                return View();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}