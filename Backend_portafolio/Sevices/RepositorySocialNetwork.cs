using Microsoft.Data.SqlClient;
using Backend_portafolio.Models;
using Dapper;

namespace Backend_portafolio.Sevices
{
    public interface IRepositorySocialNetwork
    {
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

        // OBTENER REDES SOCIALES POR ID
        public async Task<IEnumerable<Models.SocialNetwork>> ObtenerPorUsuario(int user_id)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<Models.SocialNetwork>($@"SELECT * FROM {SocialNetwork.TABLE} WHERE {SocialNetwork.USERID} = @{SocialNetwork.USERID}", new { user_id });
        }
    }
}
