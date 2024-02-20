using Backend_portafolio.Controllers;
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
		Task Crear(Categoria categoria);
		Task<bool> Existe(Categoria categoria);
		Task<Categoria> Obtener(int id);
	}

	public class RepositoryCategorias : IRepositoryCategorias
	{
		private readonly string _connectionString;
        public RepositoryCategorias(IConfiguration configuration)
        {
			_connectionString = configuration.GetConnectionString("DevConnection");
        }

		public async Task Crear(Categoria categoria)
		{
			using var connection = new SqlConnection(_connectionString);
			var id = await connection.QuerySingleAsync<int>($@"INSERT INTO {CATEGORIA.TABLA} ({CATEGORIA.NOMBRE}) VALUES (@{CATEGORIA.NOMBRE}) 
												SELECT SCOPE_IDENTITY()", categoria);
			categoria.id = id;
		}

		public async Task<Categoria> Obtener(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			Categoria categoria = await connection.QueryFirstOrDefaultAsync<Categoria>(
					$@"SELECT {CATEGORIA.NOMBRE} FROM {CATEGORIA.TABLA} WHERE {CATEGORIA.ID} = @{CATEGORIA.ID} ",
					new { id }
				);

			return categoria;
		}

		public async Task<bool> Existe(Categoria categoria)
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
							, categoria
						);

			return existe;
		}
	}
}
