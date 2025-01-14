using Backend_portafolio.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    struct MEDIA
    {
        public const string TABLA = "media";
        public const string ID = "id";
        public const string URL = "url";
        public const string POST_ID = "post_id";
        public const string MEDIA_ID = "mediatype_id";
        public const string MEDIATYPE = "mediatype";
        public const string NAME = "name";

    }
    public interface IRepositoryMedia
    {
        Task Borrar(int id);
        Task Crear(IEnumerable<Media> media);
        Task Crear(Media media);
        Task Editar(Media nuevoMedia);
        Task<IEnumerable<Media>> Obtener();
        Task<IEnumerable<Media>> ObtenerPorPost(int post_id);
    }

    public class RepositoryMedia : IRepositoryMedia
    {
        private readonly string _connectionString;
        public RepositoryMedia(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task Crear(IEnumerable<Media> mediaList)
        {
            using var connection = new SqlConnection(_connectionString);

            foreach (var media in mediaList)
            {
                await connection.ExecuteAsync(
                    $@"INSERT INTO {MEDIA.TABLA} ({MEDIA.URL}, {MEDIA.POST_ID}, {MEDIA.MEDIA_ID}) VALUES (@{MEDIA.URL}, @{MEDIA.POST_ID}, @{MEDIA.MEDIA_ID});",
                    media
                );
            }
        }

        public async Task Crear(Media mediaList)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                $@"INSERT INTO {MEDIA.TABLA} ({MEDIA.URL}, {MEDIA.POST_ID}, {MEDIA.MEDIA_ID}) VALUES (@{MEDIA.URL}, @{MEDIA.POST_ID}, @{MEDIA.MEDIA_ID});",
                mediaList
            );
        }

        public async Task<IEnumerable<Media>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Media>($@"SELECT {MEDIA.URL}, {MEDIA.POST_ID}, {MEDIA.MEDIA_ID} FROM {MEDIA.TABLA};");

        }

        public async Task<IEnumerable<Media>> ObtenerPorPost(int post_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Media>(
                            $@"SELECT M.{MEDIA.ID}, M.{MEDIA.URL}, M.{MEDIA.POST_ID}, M.{MEDIA.MEDIA_ID}, MT.{MEDIATYPE.NAME}
                            FROM {MEDIA.TABLA} M
                            INNER JOIN {MEDIATYPE.TABLA} MT
                            ON MT.{MEDIATYPE.ID} = M.{MEDIA.MEDIA_ID}
                            WHERE {MEDIA.POST_ID} = @{MEDIA.POST_ID};",
                            new
                            {
                                post_id
                            });
        }

        public async Task Editar(Media nuevoMedia)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {MEDIA.TABLA} SET {MEDIA.URL} = @{MEDIA.URL}, {MEDIA.MEDIA_ID} = @{MEDIA.MEDIA_ID} WHERE {MEDIA.ID} = @{MEDIA.ID}", nuevoMedia);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"DELETE {MEDIA.TABLA} WHERE {MEDIA.ID} = @{MEDIA.ID} ", new { id });
        }
    }
}
