using Backend_portafolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend_portafolio.Models;

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
        [HttpGet]
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
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }
        }


        //****************************************************
        //******************** FORMATOS **********************
        //****************************************************
        [AllowAnonymous]
        [HttpGet]
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
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }

        }

        //****************************************************
        //********************* USUARIOS *********************
        //****************************************************

        [AllowAnonymous]
        [HttpGet]
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
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

        //****************************************************
        //********************** HOME** **********************
        //****************************************************

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Home([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            try
            {
                var home = await _apiService.GetHomeSection(apiKey);
                return Ok(home);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>()
                {
                    Success = false,
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

        //****************************************************
        //******************** ENTRADAS **********************
        //****************************************************

        [AllowAnonymous]
        [HttpGet]
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
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }

        }

        [AllowAnonymous]
        [HttpGet("{controller}/{action}/{id}")]
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
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("{controller}/{action}/{pageNumber}/{pageSize}")]
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
                    Message = "Ha ocurrido un error",
                    Data = ex.Message,
                });
            }
        }

    }
}
