using Backend_portafolio.Services;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_portafolio.Controllers
{

    public class ApiController : ControllerBase
    {
        private readonly IApiService _apiService;

        public ApiController(
            IApiService apiService
            )
        {
            _apiService = apiService;
        }

        //****************************************************
        //******************** CATEGOIAS *********************
        //****************************************************

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Categoria([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                var categories = await _apiService.GetCategories(apiKey);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ah ocurrido un error",
                    Data = ex.Message,
                });
            }
        }


        //****************************************************
        //******************** FORMATOS **********************
        //****************************************************
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Formato([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                var formats = await _apiService.GetFormats(apiKey);
                return Ok(formats);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ah ocurrido un error",
                    Data = ex.Message,
                });
            }

        }

        //****************************************************
        //********************* USUARIOS *********************
        //****************************************************

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Usuario([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                var user = await _apiService.GetUser(apiKey);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ah ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

        //****************************************************
        //******************** ENTRADAS **********************
        //****************************************************

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Entrada([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                var posts = await _apiService.GetAllPosts(apiKey);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ah ocurrido un error",
                    Data = ex.Message,
                });
            }

        }

        [AllowAnonymous]
        [HttpPost("{controller}/{action}/{id}")]
        public async Task<IActionResult> Entrada([FromHeader(Name = "X-Api-Key")] string apiKey, int id)
        {
            try
            {
                var post = await _apiService.GetPostById(apiKey, id);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ah ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("{controller}/{action}/{pageNumber}/{pageSize}")]
        public async Task<IActionResult> Entrada([FromHeader(Name = "X-Api-Key")] string apiKey, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var posts = await _apiService.GetPostsPagination(apiKey, pageNumber, pageSize);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ah ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

    }
}
