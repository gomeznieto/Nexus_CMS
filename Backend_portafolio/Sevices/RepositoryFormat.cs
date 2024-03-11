using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
	struct FORMAT
	{
		public const string TABLA = "format";
		public const string ID = "id";
		public const string NOMBRE = "name";
	}

	public interface IRepositoryFormat
	{
		Task Borrar(int id);
		Task Crear(Format format);
		Task Editar(Format format);
		Task<bool> Existe(string name);
		Task<IEnumerable<Format>> Obtener();
		Task<Format> ObtenerPorId(int id);
		Task<bool> sePuedeBorrar(int format_id);
	}

	public class RepositoryFormat : IRepositoryFormat
	{
		private readonly string _connectionString;

		public RepositoryFormat(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DevConnection");
		}

		public async Task<IEnumerable<Format>> Obtener()
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QueryAsync<Format>($@"SELECT {FORMAT.ID}, {FORMAT.NOMBRE} FROM {FORMAT.TABLA} ORDER BY {FORMAT.NOMBRE}");
		}

		public async Task<Format> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QueryFirstOrDefaultAsync<Format>($@"SELECT {FORMAT.ID}, {FORMAT.NOMBRE} FROM {FORMAT.TABLA} WHERE {FORMAT.ID} = @{FORMAT.ID}", new { id });
		}

		public async Task Crear(Format format)
		{
			using var connection = new SqlConnection(_connectionString);
			var id = await connection.QuerySingleAsync<int>($@"INSERT INTO {FORMAT.TABLA} ({FORMAT.NOMBRE}) VALUES (@{FORMAT.NOMBRE}); SELECT SCOPE_IDENTITY();", format);
			format.id = id;
		}

		public async Task Editar(Format format)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"UPDATE {FORMAT.TABLA} SET ({FORMAT.NOMBRE} = @{FORMAT.NOMBRE}) WHERE {FORMAT.ID} = @{FORMAT.ID};", format);
		}

		public async Task Borrar(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"DELETE FROM {FORMAT.TABLA} WHERE {FORMAT.ID} = @{FORMAT.ID}", new { id });
		}

		public async Task<bool> Existe(string name)
		{
			using var connection = new SqlConnection(_connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<bool>(
					$@"IF EXISTS (
                    SELECT 1 FROM {FORMAT.TABLA}
                    WHERE UPPER({FORMAT.NOMBRE}) = UPPER(@{FORMAT.NOMBRE})
						)
						SELECT 1
					ELSE
						SELECT 0"
							, new { name }
						);

			return existe;
		}

		public async Task<bool> sePuedeBorrar(int format_id)
		{
			using var connection = new SqlConnection(_connectionString);

			var cantidadDePost = await connection.QuerySingleAsync<int>($@"SELECT COUNT({POST.ID}) FROM {POST.TABLA} 
														WHERE {POST.FORMAT_ID} = @{POST.FORMAT_ID}",
														new { format_id });
			return cantidadDePost == 0;
		}
	}
}
