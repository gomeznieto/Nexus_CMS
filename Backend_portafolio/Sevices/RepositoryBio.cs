using Backend_portafolio.Entities;
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
        Task Agregar(Bio bio);
        Task Borrar(int id, int user_id);
        Task Editar(Bio bio);
        Task<IEnumerable<Bio>> Obtener(int user_id);
        Task<Bio> ObtenerPorId(int id, int user_id);
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

        // OBTENER POR ID
        public async Task<Bio> ObtenerPorId(int id, int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Bio>($@"SELECT {BIO.ID}, {BIO.WORK}, {BIO.YEAR}, {BIO.USER_ID} 
                                                                    FROM {BIO.TABLA} WHERE {BIO.ID} = @{BIO.ID} AND {BIO.USER_ID} = @{BIO.USER_ID}",
                                                                    new { id, user_id });
        }

        // AGREGAR
        public async Task Agregar(Bio bio)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"INSERT INTO {BIO.TABLA} ({BIO.WORK}, {BIO.YEAR}, {BIO.USER_ID}) 
                                            VALUES (@{BIO.WORK}, @{BIO.YEAR}, @{BIO.USER_ID})", bio);
        }

        // EDITAR
        public async Task Editar(Bio bio)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {BIO.TABLA} SET {BIO.WORK} = @{BIO.WORK}, {BIO.YEAR} = @{BIO.YEAR} 
                                            WHERE {BIO.ID} = @{BIO.ID}", bio);
        }


        // BORRAR
        public async Task Borrar(int id, int user_id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"DELETE FROM {BIO.TABLA} WHERE {BIO.ID} = @{BIO.ID} AND {BIO.USER_ID} = @{BIO.USER_ID}", new { id, user_id });
        }
    }
}
