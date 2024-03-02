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
		Task<IEnumerable<Format>> Obtener();
		Task<Format> ObtenerPorId(int id);
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
	}
}
