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
	}
	public interface IRepositoryCategorias
	{
		Task Editar(Categoria categoria);
		Task Crear(Categoria categoria);
		Task<bool> Existe(string name);
		Task<IEnumerable<Categoria>> Obtener();
		Task<Categoria> ObtenerPorId(int id);
		Task Borrar(int id);
		Task<bool> sePuedeBorrar(int category_id);
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
			var id = await connection.QuerySingleAsync<int>($@"INSERT INTO {CATEGORIA.TABLA} ({CATEGORIA.NOMBRE}) VALUES (@{CATEGORIA.NOMBRE}) 
												SELECT SCOPE_IDENTITY()", categoria);
			categoria.id = id;
		}

		// OBTENER CATEGORIA POR LA ID
		public async Task<Categoria> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			Categoria categoria = await connection.QueryFirstOrDefaultAsync<Categoria>(
					$@"SELECT {CATEGORIA.NOMBRE} FROM {CATEGORIA.TABLA} WHERE {CATEGORIA.ID} = @{CATEGORIA.ID} ",
					new { id }
				);

			return categoria;
		}

		// OBTENER TODAS LAS CATEGORIAS
		public async Task<IEnumerable<Categoria>> Obtener()
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QueryAsync<Categoria>($@"SELECT {CATEGORIA.ID}, {CATEGORIA.NOMBRE} FROM {CATEGORIA.TABLA}");

		}

		// VERIFICAR SI EXISTE EL NOMBRE DE LA CATEGORIA
		public async Task<bool> Existe(string name)
		{
			using var connection = new SqlConnection(_connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<bool>(
					$@"IF EXISTS (
                    SELECT 1 FROM {CATEGORIA.TABLA}
                    WHERE UPPER({CATEGORIA.NOMBRE}) = UPPER(@{CATEGORIA.NOMBRE})
						)
						SELECT 1
					ELSE
						SELECT 0"
							, new { name }
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
