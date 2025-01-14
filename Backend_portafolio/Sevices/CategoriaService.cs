
using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Services;
using Backend_portafolio.Helper;

namespace Backend_portafolio.Sevices
{
    public interface ICategociaService
    {
        Task<bool> CreateCategoriesForm(int id, List<CategoryForm> categoriesForms);
        Task<IEnumerable<Categoria>> GetAllCategorias(int userId);
        Task<Categoria> GetCategoriaById(int id);
        Task<IEnumerable<Categoria>> GetCategoriasByPost(int post_id);
    }


    public class CategoriaService : ICategociaService
    {
        private readonly UsersService _usersService;
        private readonly IRepositoryCategorias _repositoryCategorias;

        public CategoriaService(
            UsersService usersService,
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
}
