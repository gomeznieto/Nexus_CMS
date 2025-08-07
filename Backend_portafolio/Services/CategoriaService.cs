
using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Services;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;

namespace Backend_portafolio.Sevices
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategorias(int userID = 0);
        Task<List<CategoryViewModel>> GetCategoryByName(string buscar);
        CategoryViewModel GetViewModel();
        Task<IEnumerable<Category_Post>> GetCategoriasByPost(int post_id);
        Task<CategoryViewModel> GetCategoriaById(int id);
        Task CreateCategoriesForm(int id, List<CategoryForm> categoriesForms);
        Task CreateCategories(CategoryViewModel category);
        Task EditCategory(CategoryViewModel category);
        Task DeleteCategory(int id);
        Task<bool> Existe(string name);
        public List<CategoryForm> SerealizarJsonCategoryForm(string jsonCategoria);
        Task<List<Category_Post>> SerealizarJsonCategoryPost(string jsonCategoria);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly IUsersService _usersService;
        private readonly IRepositoryCategorias _repositoryCategorias;
        private readonly IMapper _mapper;

        public CategoriaService(
            IUsersService usersService,
            IRepositoryCategorias repositoryCategorias,
            IMapper mapper
        )
        {
            _usersService = usersService;
            _repositoryCategorias = repositoryCategorias;
            _mapper = mapper;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        // Obtener todas las categorías
        public async Task<IEnumerable<CategoryViewModel>> GetAllCategorias(int userID = 0)
        {
            if(userID == 0)
                userID = _usersService.ObtenerUsuario();

            var categorias = _mapper.Map<IEnumerable<CategoryViewModel>>(await _repositoryCategorias.Obtener(userID));

            return categorias;
        }

        // Obtener una categoría por id
        public async Task<CategoryViewModel> GetCategoriaById(int id)
        {
            try
            {
                var categoria = await _repositoryCategorias.ObtenerPorId(id);

                if (categoria == null)
                    throw new Exception("Ha surgido un error. ¡Intente más tarde!");

                return _mapper.Map<CategoryViewModel>(categoria);
            }
            catch (Exception e)
            {
                throw new Exception($"Ha surgido un error. ¡Intente más tarde! {e}");
            }
        }

        // Obtener las categorías de un post
        public async Task<IEnumerable<Category_Post>> GetCategoriasByPost(int post_id)
        {
            var categorias = await _repositoryCategorias.ObtenerCategoriaPostPorId(post_id);
            return categorias;
        }


        // Obtener categorías por nombre
        public async Task<List<CategoryViewModel>> GetCategoryByName(string buscar)
        {
            var userID = _usersService.ObtenerUsuario();
            var categorias = await GetAllCategorias();

            if (!buscar.IsNullOrEmpty())
            {
                categorias = categorias.Where(p => p.name.ToUpper().Contains(buscar.ToUpper()));
            }

            return categorias.OrderBy(x => x.name).ToList();
        }

        // Obtener el modelo de vista
        public CategoryViewModel GetViewModel()
        {
            var userID = _usersService.ObtenerUsuario();
            var viewModel = new Categoria();
            viewModel.user_id = userID;

            return _mapper.Map<CategoryViewModel>(viewModel) ;
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreateCategories(CategoryViewModel category)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                if (userID != category.user_id)
                {
                    throw new SecurityTokenException("No puedes crear una categoría para otro usuario");
                }

                var existe = await _repositoryCategorias.Existe(category.name, userID);

                if (existe)
                {
                    throw new Exception("La categoría ya existe");
                }

                category.name = category.name.Trim();

                await _repositoryCategorias.Crear(_mapper.Map<Categoria>(category));

            }
            catch (Exception)
            {
                throw new Exception("Ha surgido un error. ¡Intente más tarde!");
            }
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
            catch (Exception)
            {
                throw new Exception("Ha surgido un error. ¡Intente más tarde!");
            }
        }

        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditCategory(CategoryViewModel category)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                if (userID != category.user_id)
                {
                    throw new SecurityTokenException("No puedes editar una categoría de otro usuario");
                }
                var existe = await _repositoryCategorias.Existe(category.name, userID);
                if (existe)
                {
                    throw new Exception("La categoría ya existe");
                }
                category.name = category.name.Trim();
                await _repositoryCategorias.Editar(_mapper.Map<Categoria>(category));
            }
            catch (Exception)
            {
                throw new Exception("Ha surgido un error. ¡Intente más tarde!");
            }
        }

        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        public async Task DeleteCategory(int id)
        {
            try
            {
                var category = await GetCategoriaById(id);

                if (category == null)
                    throw new Exception("La categoría no existe");

                //Verificar si no está en uso
                var borrar = await _repositoryCategorias.sePuedeBorrar(id);

                if (!borrar)
                    throw new Exception("La categoría está en uso");

                //Borrar
                await _repositoryCategorias.Borrar(id);

            }
            catch (Exception)
            {
                throw new Exception("Ha surgido un error. ¡Intente más tarde!");
            }
        }

        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************

        // Serializar las categorías de un post
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

        // Serializar las categorías de un post
        public List<CategoryForm> SerealizarJsonCategoryForm(string jsonCategoria)
        {
            return JsonSerializer.Deserialize<List<CategoryForm>>(jsonCategoria, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Verificar si existe una categoría
        public async Task<bool> Existe(string name)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                var existeCategoria = await _repositoryCategorias.Existe(name, userID);

                if (existeCategoria)
                    throw new Exception($"La categoría {name} ya existe! Intente con otro.");

                return existeCategoria;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
