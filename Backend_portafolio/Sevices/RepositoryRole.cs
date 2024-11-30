using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    public interface IRepositoryRole
    {
        Task<IEnumerable<Role>> Obtener();
    }

    public class RepositoryRole : IRepositoryRole
    {
        private readonly string _connectionString;

        public RepositoryRole(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }


        // Obtener
        public async Task<IEnumerable<Role>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<Role>(@"SELECT * FROM role");
        }
    }
}
