using Backend_portafolio.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    struct SOURCE
	{

		public const string TABLA = "source";
		public const string ID = "id";
		public const string NAME = "name";
		public const string ICON = "icon";
		public const string USER_ID = "user_id";

	}
	public interface IRepositorySource
	{
		Task Borrar(int id, int user_id);
		Task Crear(Source source);
		Task Editar(Source source);
		Task<bool> Existe(string name, int user_id);
		Task<IEnumerable<Source>> Obtener(int user_id);
		Task<Source> ObtenerPorId(int id);
		Task<bool> sePuedeBorrar(int source_id);
	}

	public class RepositorySource : IRepositorySource
	{
		private readonly string _connectionString;

		public RepositorySource(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DevConnection");

		}

		// OBTENER LISTADO DE MEDIA TYPES
		public async Task<IEnumerable<Source>> Obtener(int user_id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QueryAsync<Source>($@"SELECT {SOURCE.ID}, {SOURCE.NAME}, {SOURCE.ICON} FROM {SOURCE.TABLA} WHERE {SOURCE.USER_ID} = @{SOURCE.USER_ID}", new { user_id });
		}

		//OBTENER POR ID
		// OBTENER LISTADO DE MEDIA TYPES
		public async Task<Source> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QuerySingleAsync<Source>($@"SELECT {SOURCE.ID}, {SOURCE.NAME}, {SOURCE.ICON}, {SOURCE.USER_ID} FROM {SOURCE.TABLA} WHERE {SOURCE.ID} = @{SOURCE.ID}", new { id });
		}

		//TODO CREAR
		public async Task Crear(Source source)
		{
			using var connection = new SqlConnection(_connectionString);
			var id = await connection.QuerySingleAsync<int>($@"INSERT INTO {SOURCE.TABLA} ({SOURCE.NAME}, {SOURCE.ICON}, {SOURCE.USER_ID}) VALUES (@{SOURCE.NAME}, @{SOURCE.ICON}, @{SOURCE.USER_ID}) 
															SELECT SCOPE_IDENTITY()", source);
			source.id = id;
		}

		//TODO EDITAR
		public async Task Editar(Source source)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"UPDATE {SOURCE.TABLA} SET {SOURCE.NAME} = @{SOURCE.NAME}, {SOURCE.ICON} = @{SOURCE.ICON} WHERE {SOURCE.ID} = @{SOURCE.ID} AND {SOURCE.USER_ID} = @{SOURCE.USER_ID}", source);
		}

		//TODO BORRA
		public async Task Borrar(int id, int user_id)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"DELETE {SOURCE.TABLA} WHERE {SOURCE.ID} = @{SOURCE.ID} AND {SOURCE.USER_ID} = @{SOURCE.USER_ID}", new { id, user_id });
		}

		//TODO EXISTE
		public async Task<bool> Existe(string name, int user_id)
		{
			using var connection = new SqlConnection(_connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<bool>(
					$@"IF EXISTS (
                    SELECT 1 FROM {SOURCE.TABLA}
                    WHERE UPPER({SOURCE.NAME}) = UPPER(@{SOURCE.NAME}) AND {SOURCE.USER_ID} = @{SOURCE.USER_ID}
						)
						SELECT 1
					ELSE
						SELECT 0"
							, new { name, user_id }
						);

			return existe;
		}
		public async Task<bool> sePuedeBorrar(int source_id)
		{
			using var connection = new SqlConnection(_connectionString);

			var cantidadDeLinksExistentes = await connection.QuerySingleAsync<int>($@"SELECT COUNT({SOURCE.ID}) FROM {LINK.TABLA} 
														WHERE {LINK.SOURCE_ID} = @{LINK.SOURCE_ID}",
														new { source_id });
			return cantidadDeLinksExistentes == 0;
		}
	}
}
