using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    struct LINK
    {
        public const string TABLA = "link";
        public const string ID = "id";
        public const string URL = "url";
        public const string POST_ID = "post_id";
        public const string SOURCE_ID = "source_id";
        public const string SOURCE = "source";
        public const string NAME = "name";

    }
    public interface IRepositoryLink
    {
        Task Borrar(int id);
        Task Crear(IEnumerable<Link> mediaList);
        Task Editar(Link nuevoLink);
        Task<IEnumerable<Link>> Obtener();
        Task<IEnumerable<Link>> ObtenerPorPost(int post_id);
    }

    public class RepositoryLink : IRepositoryLink
    {
        private readonly string _connectionString;
        public RepositoryLink(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task Crear (IEnumerable<Link> mediaList)
        {
            using var connection = new SqlConnection(_connectionString);

            foreach (var media in mediaList)
            {
                await connection.ExecuteAsync(
                    $@"INSERT INTO {LINK.TABLA} ({LINK.URL}, {LINK.POST_ID}, {LINK.SOURCE_ID}) VALUES (@{LINK.URL}, @{LINK.POST_ID}, @{LINK.SOURCE_ID});",
                    media
                );
            }
        }

        public async Task<IEnumerable<Link>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Link>($@"SELECT {LINK.URL}, {LINK.POST_ID}, {LINK.SOURCE_ID} FROM {LINK.TABLA};");

        }

        public async Task<IEnumerable<Link>> ObtenerPorPost(int post_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Link>(
                        $@"SELECT L.{LINK.ID}, L.{LINK.URL}, L.{LINK.POST_ID}, L.{LINK.SOURCE_ID}, S.{SOURCE.NAME}, S.{SOURCE.ICON}
                        FROM {LINK.TABLA} L
                        INNER JOIN {SOURCE.TABLA} S
                        ON S.{SOURCE.ID} = L.{LINK.SOURCE_ID}
                        WHERE L.{LINK.POST_ID} = @{LINK.POST_ID};", 
                        new {
                            post_id
                        });
        }

        public async Task Editar(Link nuevoLink)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {LINK.TABLA} SET {LINK.URL} = @{LINK.URL}, {LINK.SOURCE_ID} = @{LINK.SOURCE_ID} WHERE {LINK.ID} = @{LINK.ID}", nuevoLink );
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"DELETE {LINK.TABLA} WHERE {LINK.ID} = @{LINK.ID} ", new { id });
        }
    }
}
