using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    struct BIO
    {
       public const string TABLA = "bio";
       public const string ID = "id";
       public const string USER_ID = "user_id";
       public const string WORK = "work";
       public const string YEAR = "year";
    }
    public interface IRepositoryBio
    {
        Task<IEnumerable<Bio>> Obtener(int user_id);
    }

    public class RepositoryBio : IRepositoryBio
    {
        private readonly string _connectionString;

        public RepositoryBio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // OBTENER
        public async Task<IEnumerable<Bio>> Obtener(int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Bio>($@"SELECT {BIO.ID}, {BIO.WORK}, {BIO.YEAR}, {BIO.USER_ID} 
                                                        FROM {BIO.TABLA} WHERE {BIO.USER_ID} = @{BIO.USER_ID}", 
                                                        new { user_id });

        }

        // AGREGAR


        // EDITAR


        // BORRAR
    }
}
