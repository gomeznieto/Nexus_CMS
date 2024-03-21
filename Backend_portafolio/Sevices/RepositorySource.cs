using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    struct SOURCE
    {

        public const string TABLA = "source";
        public const string ID = "id";
        public const string NAME = "name";
        public const string ICON = "icon";

    }
    public interface IRepositorySource
    {
        Task Borrar(int id);
        Task Crear(Source source);
        Task Editar(Source source);
        Task<bool> Existe(string name);
        Task<IEnumerable<Source>> Obtener();
        Task<Source> ObtenerPorId(int id);
    }

    public class RepositorySource : IRepositorySource
    {
        private readonly string _connectionString;

        public RepositorySource(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");

        }

        // OBTENER LISTADO DE MEDIA TYPES
        public async Task<IEnumerable<Source>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Source>($@"SELECT {SOURCE.ID}, {SOURCE.NAME} FROM {SOURCE.TABLA}");
        }

        //OBTENER POR ID
        // OBTENER LISTADO DE MEDIA TYPES
        public async Task<Source> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QuerySingleAsync<Source>($@"SELECT {SOURCE.ID}, {SOURCE.NAME} FROM {SOURCE.TABLA} WHERE {SOURCE.ID} = @{SOURCE.ID}", new { id });
		}

		//TODO CREAR
		public async Task Crear(Source source)
		{
			using var connection = new SqlConnection(_connectionString);
			var id = await connection.QuerySingleAsync<int>($@"INSERT INTO {SOURCE.TABLA} ({SOURCE.NAME}) VALUES (@{SOURCE.NAME}) 
															SELECT SCOPE_IDENTITY()", source);
			source.id = id;
		}

        //TODO EDITAR
        public async Task Editar(Source source)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {SOURCE.TABLA} SET {SOURCE.NAME} = @{SOURCE.NAME} WHERE {SOURCE.ID} = @{SOURCE.ID}", source);
        }

        //TODO BORRA
        public async Task Borrar(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"DELETE {SOURCE.TABLA} WHERE {SOURCE.ID} = @{SOURCE.ID}", new { id });
		}

		//TODO EXISTE
		public async Task<bool> Existe(string name)
		{
			using var connection = new SqlConnection(_connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<bool>(
					$@"IF EXISTS (
                    SELECT 1 FROM {SOURCE.TABLA}
                    WHERE UPPER({SOURCE.NAME}) = UPPER(@{SOURCE.NAME})
						)
						SELECT 1
					ELSE
						SELECT 0"
							, new { name }
						);

			return existe;
		}

		//CUANDO ESTE LINK CREADO

		//public async Task<bool>sePuedeBorrar(int source_id)
		//{
		//	using var connection = new SqlConnection(_connectionString);

		//	var cantidadDeLinksExistentes =  await connection.QuerySingleAsync<int>($@"SELECT COUNT({SOURCE.ID}) FROM {SOURCE.TABLA} 
		//												WHERE {SOURCE.MEDIA_ID} = @{SOURCE.MEDIA_ID}", 
		//												new { mediatype_id });
		//	return cantidadDeLinksExistentes == 0;
		//}
	}
}
