
using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Services;
using System.Text.Json;

namespace Backend_portafolio.Sevices
{
    public interface ICategoriaService
    {
        Task CreateCategoriesForm(int id, List<CategoryForm> categoriesForms);
        Task<IEnumerable<Categoria>> GetAllCategorias(int userId);
        Task<Categoria> GetCategoriaById(int id);
        Task<IEnumerable<Categoria>> GetCategoriasByPost(int post_id);
        Task<List<Category_Post>> SerealizarJsonCategoryPost(string jsonCategoria);
        public List<CategoryForm> SerealizarJsonCategoryForm(string jsonCategoria);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly IUsersService _usersService;
        private readonly IRepositoryCategorias _repositoryCategorias;

        public CategoriaService(
            IUsersService usersService,
            IRepositoryCategorias repositoryCategorias
        )
        {
            _usersService = usersService;
            _repositoryCategorias = repositoryCategorias;
        }

        public async Task<IEnumerable<Categoria>> GetAllCategorias(int userId)
        {
            var userID = _usersService.ObtenerUsuario();

            var categorias = await _repositoryCategorias.Obtener(userID);

            return categorias;
        }

        public async Task<Categoria> GetCategoriaById(int id)
        {
            var categoria = await _repositoryCategorias.ObtenerPorId(id);
            return categoria;
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasByPost(int post_id)
        {
            var categorias = await _repositoryCategorias.ObtenerPorPost(post_id);
            return categorias;
        }

        public async Task CreateCategoriesForm(int id, List<CategoryForm> categoriesForms)
        {
            try
            {
                foreach (var category in categoriesForms)
                {
                    // Validar que cada categoría exista
                    var categorySearched = _repositoryCategorias.ObtenerPorId(category.category_id);

                    if (categorySearched == null)
                    {
                        throw new Exception("La categoría no existe");
                    }

                    // Agregarle el número del post a CategoriaForm
                    category.post_id = id;
                }

                //Subimos Las categorias del Post
                await _repositoryCategorias.CrearCategoriaPorPost(categoriesForms);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Category_Post>> SerealizarJsonCategoryPost(string jsonCategoria)
        {
            try
            {
                List<CategoryForm> categoriesForms = JsonSerializer.Deserialize<List<CategoryForm>>(jsonCategoria, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                List<Category_Post> categoryPostList = new List<Category_Post>();

                foreach (var category in categoriesForms)
                {
                    // Validar que cada categoría exista
                    var categorySearched = await GetCategoriaById(category.category_id);
                    categorySearched.id = category.category_id;

                    if (categorySearched == null)
                    {
                        throw new Exception("La categoría no existe");
                    }
                    else
                    {
                        Category_Post aux = new Category_Post();
                        aux.Categoria = categorySearched;
                        categoryPostList.Add(aux);
                    }
                }

                return categoryPostList;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<CategoryForm> SerealizarJsonCategoryForm(string jsonCategoria)
        {
            return JsonSerializer.Deserialize<List<CategoryForm>>(jsonCategoria, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
