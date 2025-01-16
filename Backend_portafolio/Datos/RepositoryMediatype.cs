using Backend_portafolio.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    struct MEDIATYPE
    {

        public const string TABLA = "mediatype";
        public const string ID = "id";
        public const string NAME = "name";
        public const string USER_ID = "user_id";

    }
    public interface IRepositoryMediatype
    {
		Task Borrar(int id);
		Task Crear(MediaType mediatype);
		Task Editar(MediaType mediatype);
		Task<bool> Existe(string name, int user_id);
		Task<IEnumerable<MediaType>> Obtener(int user_id);
		Task<MediaType> ObtenerPorId(int id);
		Task<bool> sePuedeBorrar(int mediatype_id);
	}

    public class RepositoryMediatype : IRepositoryMediatype
    {
        private readonly string _connectionString;

        public RepositoryMediatype(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");

        }

        // OBTENER LISTADO DE MEDIA TYPES
        public async Task<IEnumerable<MediaType>> Obtener(int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MediaType>($@"SELECT {MEDIATYPE.ID}, {MEDIATYPE.NAME}, {MEDIATYPE.USER_ID} FROM {MEDIATYPE.TABLA} WHERE {MEDIATYPE.USER_ID} = @{MEDIATYPE.USER_ID}", new { user_id});
        }

		//OBTENER POR ID
		// OBTENER LISTADO DE MEDIA TYPES
		public async Task<MediaType> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QuerySingleAsync<MediaType>($@"SELECT {MEDIATYPE.ID}, {MEDIATYPE.NAME}, {MEDIATYPE.USER_ID} FROM {MEDIATYPE.TABLA} WHERE {MEDIATYPE.ID} = @{MEDIATYPE.ID}", new { id });
		}

		//TODO CREAR
		public async Task Crear(MediaType mediatype)
		{
			try
			{
                using var connection = new SqlConnection(_connectionString);
                var id = await connection.QuerySingleAsync<int>($@"INSERT INTO {MEDIATYPE.TABLA} ({MEDIATYPE.NAME}, {MEDIATYPE.USER_ID}) VALUES (@{MEDIATYPE.NAME}, @{MEDIATYPE.USER_ID}) 
															SELECT SCOPE_IDENTITY()", mediatype);
                mediatype.id = id;
            }
			catch(SqlException ex)
			{
                throw new ApplicationException(ex.Message);
            }
		}

		//TODO EDITAR
		public async Task Editar(MediaType mediaType)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {MEDIATYPE.TABLA} SET {MEDIATYPE.NAME} = @{MEDIATYPE.NAME} WHERE {MEDIATYPE.ID} = @{MEDIATYPE.ID} AND {MEDIATYPE.USER_ID} = @{MEDIATYPE.USER_ID}", mediaType);
        }

		//TODO BORRA
		public async Task Borrar(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"DELETE {MEDIATYPE.TABLA} WHERE {MEDIATYPE.ID} = @{MEDIATYPE.ID}", new { id });
		}

		//TODO EXISTE
		public async Task<bool> Existe(string name, int user_id)
		{
			using var connection = new SqlConnection(_connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<bool>(
					$@"IF EXISTS (
                    SELECT 1 FROM {MEDIATYPE.TABLA}
                    WHERE UPPER({MEDIATYPE.NAME}) = UPPER(@{MEDIATYPE.NAME}) AND {MEDIATYPE.USER_ID} = @{MEDIATYPE.USER_ID}
						)
						SELECT 1
					ELSE
						SELECT 0"
							, new { name, user_id }
						);

			return existe;
		}

		public async Task<bool>sePuedeBorrar(int mediatype_id)
		{
			using var connection = new SqlConnection(_connectionString);

			var cantidadDeLinksExistentes =  await connection.QuerySingleAsync<int>($@"SELECT COUNT({MEDIA.ID}) FROM {MEDIA.TABLA} 
														WHERE {MEDIA.MEDIA_ID} = @{MEDIA.MEDIA_ID}", 
														new { mediatype_id });
			return cantidadDeLinksExistentes == 0;
		}
	}
}
