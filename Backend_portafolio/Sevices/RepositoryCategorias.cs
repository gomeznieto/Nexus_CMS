using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;


namespace Backend_portafolio.Sevices
{
    struct CATEGORIA
    {
        public const string TABLA = "category";
        public const string ID = "id";
        public const string NOMBRE = "name";
        public const string USER_ID = "user_id";
    }

    struct CATEGORIA_POST
    {
        public const string TABLA = "category_post";
        public const string ID = "id";
        public const string CATEGORY_ID = "category_id";
        public const string POST_ID = "post_id";
    }
    public interface IRepositoryCategorias
    {
        Task Editar(Categoria categoria);
        Task Crear(Categoria categoria);
        Task CrearCategoriaPorPost(IEnumerable<CategoryForm> categorias);
        Task CrearCategoriaPorPost(CategoryForm categorias);
        Task<bool> Existe(string name, int user_id);
        Task<IEnumerable<Categoria>> Obtener(int user_id);
        Task<Categoria> ObtenerPorId(int id);
        Task<IEnumerable<Categoria>> ObtenerPorPost(int post_id);
        Task BorrarCatergoriaPorPost(int idPost, int idCategoria);
        Task Borrar(int id);
        Task<bool> sePuedeBorrar(int category_id);
        Task<IEnumerable<Category_Post>> ObtenerCategoriaPostPorId(int id);
    }

    public class RepositoryCategorias : IRepositoryCategorias
    {
        private readonly string _connectionString;

        public RepositoryCategorias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // CREAR CATEGORIAS
        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.ExecuteScalarAsync<int>($@"INSERT INTO {CATEGORIA.TABLA} ({CATEGORIA.NOMBRE}, {CATEGORIA.USER_ID}) VALUES (@{CATEGORIA.NOMBRE}, @{CATEGORIA.USER_ID}) 
												SELECT SCOPE_IDENTITY()", categoria);
            categoria.id = id;
        }

        public async Task CrearCategoriaPorPost(IEnumerable<CategoryForm> categorias)
        {
            using var connection = new SqlConnection(_connectionString);

            foreach (var category in categorias)
            {
                await connection.ExecuteAsync(
                    $@"INSERT INTO {CATEGORIA_POST.TABLA} ({CATEGORIA_POST.POST_ID}, {CATEGORIA_POST.CATEGORY_ID}) VALUES (@{CATEGORIA_POST.POST_ID}, @{CATEGORIA_POST.CATEGORY_ID});",
                    category
                );
            }
        }

        public async Task CrearCategoriaPorPost(CategoryForm category)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                        $@"INSERT INTO {CATEGORIA_POST.TABLA} ({CATEGORIA_POST.POST_ID}, {CATEGORIA_POST.CATEGORY_ID}) VALUES (@{CATEGORIA_POST.POST_ID}, @{CATEGORIA_POST.CATEGORY_ID});",
                    category

                );
        }


        // OBTENER CATEGORIA POR LA ID
        public async Task<Categoria> ObtenerPorId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            Categoria categoria = await connection.QueryFirstOrDefaultAsync<Categoria>(
                    $@"SELECT {CATEGORIA.ID}, {CATEGORIA.NOMBRE}, {CATEGORIA.USER_ID} FROM {CATEGORIA.TABLA} WHERE {CATEGORIA.ID} = @{CATEGORIA.ID} ",
                    new { id }
                );

            return categoria;
        }

        // OBTENER TODAS LAS CATEGORIAS
        public async Task<IEnumerable<Categoria>> Obtener(int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Categoria>($@"SELECT {CATEGORIA.ID}, {CATEGORIA.NOMBRE}, {CATEGORIA.USER_ID} FROM {CATEGORIA.TABLA} WHERE {CATEGORIA.USER_ID} = @{CATEGORIA.USER_ID}", new { user_id});

        }

        public async Task<IEnumerable<Category_Post>> ObtenerCategoriaPostPorId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = $@"SELECT CP.{CATEGORIA_POST.ID}, CP.{CATEGORIA_POST.POST_ID}, C.{CATEGORIA.ID}, C.{CATEGORIA.NOMBRE}
						FROM {CATEGORIA.TABLA} C
						INNER JOIN {CATEGORIA_POST.TABLA} CP
						ON CP.{CATEGORIA_POST.CATEGORY_ID} = C.{CATEGORIA.ID}
						WHERE CP.{CATEGORIA_POST.POST_ID} = @{POST.ID}";

            return await connection.QueryAsync<Category_Post, Categoria, Category_Post>(query, (category_Post, categoria) =>
            {
                category_Post.Categoria = categoria;
                return category_Post;
            },
            splitOn: $"id",
            param: new { id }
            );

        }

        public async Task<IEnumerable<Categoria>> ObtenerPorPost(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<Categoria>(
                        $@"SELECT C.{CATEGORIA.ID}, C.{CATEGORIA.NOMBRE}, C{CATEGORIA.USER_ID}
                        FROM {POST.TABLA} P
                        INNER JOIN {CATEGORIA_POST.TABLA} CP
                        ON P.{POST.ID} = CP.{CATEGORIA_POST.POST_ID}
						INNER JOIN {CATEGORIA.TABLA} C
						ON C.{CATEGORIA.ID} = CP.{CATEGORIA_POST.CATEGORY_ID}
                        WHERE CP.{CATEGORIA_POST.POST_ID} = @{POST.ID};",
                        new
                        {
                            id
                        });
        }

        // VERIFICAR SI EXISTE EL NOMBRE DE LA CATEGORIA
        public async Task<bool> Existe(string name, int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<bool>(
                    $@"IF EXISTS (
                    SELECT 1 FROM {CATEGORIA.TABLA}
                    WHERE UPPER({CATEGORIA.NOMBRE}) = UPPER(@{CATEGORIA.NOMBRE}) AND {CATEGORIA.USER_ID} = @{CATEGORIA.USER_ID}
						)
						SELECT 1
					ELSE
						SELECT 0"
                            , new { name, user_id }
                        );

            return existe;
        }

        // EDITAR CATEGORIA
        public async Task Editar(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {CATEGORIA.TABLA} SET {CATEGORIA.NOMBRE} = @{CATEGORIA.NOMBRE} WHERE {CATEGORIA.ID} = @{CATEGORIA.ID}", categoria);
        }

        //BORRAR CATEGORIA
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"DELETE FROM {CATEGORIA.TABLA} WHERE {CATEGORIA.ID} = @{CATEGORIA.ID}", new { id });
        }

        public async Task BorrarCatergoriaPorPost(int post_id, int category_id)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync($@"DELETE FROM {CATEGORIA_POST.TABLA} WHERE {CATEGORIA_POST.POST_ID} = @{CATEGORIA_POST.POST_ID} AND {CATEGORIA_POST.CATEGORY_ID} = @{CATEGORIA_POST.CATEGORY_ID}", new { post_id, category_id });
        }

        //VERIFICAR SI SE PUEDE BORRAR
        public async Task<bool> sePuedeBorrar(int category_id)
        {
            using var connection = new SqlConnection(_connectionString);

            var cantidadDeLinksExistentes = await connection.QuerySingleAsync<int>($@"SELECT COUNT({POST.ID}) FROM {POST.TABLA} 
														WHERE {POST.CATEGORIA_ID} = @{POST.CATEGORIA_ID}",
                                                        new { category_id });
            return cantidadDeLinksExistentes == 0;
        }
    }
}
