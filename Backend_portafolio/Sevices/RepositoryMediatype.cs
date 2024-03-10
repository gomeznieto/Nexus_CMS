using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    struct MEDIATYPE
    {

        public const string TABLA = "mediatype";
        public const string ID = "id";
        public const string NAME = "name";

    }
    public interface IRepositoryMediatype
    {
		Task Borrar(int id);
		Task Editar(MediaType mediatype);
		Task<bool> Existe(string name);
		Task<IEnumerable<MediaType>> Obtener();
		Task<MediaType> ObtenerPorId(int id);
	}

    public class RepositoryMediatype : IRepositoryMediatype
    {
        private readonly string _connectionString;

        public RepositoryMediatype(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");

        }

        // OBTENER LISTADO DE MEDIA TYPES
        public async Task<IEnumerable<MediaType>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MediaType>($@"SELECT {MEDIATYPE.ID}, {MEDIATYPE.NAME} FROM {MEDIATYPE.TABLA}");
        }

		//OBTENER POR ID
		// OBTENER LISTADO DE MEDIA TYPES
		public async Task<MediaType> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QuerySingleAsync<MediaType>($@"SELECT {MEDIATYPE.ID}, {MEDIATYPE.NAME} FROM {MEDIATYPE.TABLA} WHERE {MEDIATYPE.ID} = @{MEDIATYPE.ID}", new { id });
		}

		//TODO CREAR


		//TODO EDITAR
		public async Task Editar(MediaType mediaType)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {MEDIATYPE.TABLA} SET {MEDIATYPE.NAME} = @{MEDIATYPE.NAME} WHERE {MEDIATYPE.ID} = @{MEDIATYPE.ID}", mediaType);
        }

		//TODO BORRA
		public async Task Borrar(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"DELETE {MEDIATYPE.TABLA} WHERE {MEDIATYPE.ID} = @{MEDIATYPE.ID}", new { id });
		}

		//TODO EXISTE
		public async Task<bool> Existe(string name)
		{
			using var connection = new SqlConnection(_connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<bool>(
					$@"IF EXISTS (
                    SELECT 1 FROM {MEDIATYPE.TABLA}
                    WHERE UPPER({MEDIATYPE.NAME}) = UPPER(@{MEDIATYPE.NAME})
						)
						SELECT 1
					ELSE
						SELECT 0"
							, new { name }
						);

			return existe;
		}
	}
}
