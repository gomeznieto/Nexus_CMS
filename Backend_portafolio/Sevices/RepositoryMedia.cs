using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
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
        Task Crear(IEnumerable<Media> media);
        Task<IEnumerable<Media>> Obtener();
    }

    public class RepositoryMedia : IRepositoryMedia
    {
        private readonly string _connectionString;
        public RepositoryMedia(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task Crear (IEnumerable<Media> mediaList)
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

        public async Task<IEnumerable<Media>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Media>($@"SELECT {MEDIA.URL}, {MEDIA.POST_ID}, {MEDIA.MEDIA_ID} FROM {MEDIA.TABLA};"); //TODO MODIFICAR TIPO DE MEDIA

        }

        public async Task<IEnumerable<Media>> ObtenerPorId(int post_id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Media>($@"SELECT {MEDIA.URL}, {MEDIA.POST_ID}, {MEDIA.MEDIA_ID} FROM {MEDIA.TABLA} WHERE {MEDIA.POST_ID} = @{MEDIA.POST_ID};", new {
                post_id
            }); //TODO MODIFICAR TIPO DE MEDIA

        }
    }
}
