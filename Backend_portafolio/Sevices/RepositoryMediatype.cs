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
        Task<IEnumerable<MediaType>> Obtener();
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

        //TODO CREAR
        //TODO EDITAR
        //TODO BORRA
        //TODO EXISTE
    }
}
