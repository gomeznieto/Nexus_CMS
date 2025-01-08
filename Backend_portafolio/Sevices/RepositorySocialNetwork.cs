using Microsoft.Data.SqlClient;
using Backend_portafolio.Models;
using Dapper;

namespace Backend_portafolio.Sevices
{
    public interface IRepositorySocialNetwork
    {
        Task<int> Agregar(SocialNetwork socialNetwork);
        Task<bool> Borrar(int id, int user_id);
        Task<bool> Editar(SocialNetwork socialNetwork);
        Task<SocialNetwork> ObtenerPorId(int id, int user_id);
        Task<IEnumerable<SocialNetwork>> ObtenerPorUsuario(int user_id);
    }
    public class RepositorySocialNetwork : IRepositorySocialNetwork
    {
        public struct SocialNetwork
        {
            public const string TABLE = "social_network";
            public const string ID = "id";
            public const string NAME = "name";
            public const string URL = "url";
            public const string USERNAME = "username";
            public const string ICON = "icon";
            public const string USERID = "user_id";
        }

        private readonly string _connectionString;

        public RepositorySocialNetwork(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // OBTENER REDES SOCIALES POR USUARIO
        public async Task<IEnumerable<Models.SocialNetwork>> ObtenerPorUsuario(int user_id)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<Models.SocialNetwork>($@"SELECT * FROM {SocialNetwork.TABLE} WHERE {SocialNetwork.USERID} = @{SocialNetwork.USERID}", new { user_id });
        }

        // OBTENER POR ID
        public async Task<Models.SocialNetwork> ObtenerPorId(int id, int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Models.SocialNetwork>($@"SELECT * FROM {SocialNetwork.TABLE} WHERE {SocialNetwork.ID} = @{SocialNetwork.ID} AND {SocialNetwork.USERID} = @{SocialNetwork.USERID}", new { id, user_id });
        }

        // CREAR
        public async Task<int> Agregar(Models.SocialNetwork socialNetwork)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync($@"INSERT INTO {SocialNetwork.TABLE} ({SocialNetwork.NAME}, {SocialNetwork.URL}, {SocialNetwork.USERNAME}, {SocialNetwork.ICON}, {SocialNetwork.USERID}) VALUES (@{SocialNetwork.NAME}, @{SocialNetwork.URL}, @{SocialNetwork.USERNAME}, @{SocialNetwork.ICON}, @{SocialNetwork.USERID}) SELECT SCOPE_IDENTITY()", socialNetwork);
        }

        // EDITAR
        public async Task<bool> Editar(Models.SocialNetwork socialNetwork)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync($@"UPDATE {SocialNetwork.TABLE} SET {SocialNetwork.NAME} = @{SocialNetwork.NAME}, {SocialNetwork.URL} = @{SocialNetwork.URL}, {SocialNetwork.USERNAME} = @{SocialNetwork.USERNAME}, {SocialNetwork.ICON} = @{SocialNetwork.ICON} WHERE {SocialNetwork.ID} = @{SocialNetwork.ID} AND {SocialNetwork.USERID} = @{SocialNetwork.USERID}", socialNetwork) > 0;

        }
        // BORRAR
        public async Task<bool> Borrar(int id, int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync($@"DELETE FROM {SocialNetwork.TABLE} WHERE {SocialNetwork.ID} = @{SocialNetwork.ID} AND {SocialNetwork.USERID} = @{SocialNetwork.USERID}", new { id, user_id }) > 0;
        }
    }
}
