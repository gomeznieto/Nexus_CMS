using Backend_portafolio.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    struct HOMESECTIONPOST
    {
        public const string TABLA = "HomeSectionPost";
        public const string ID = "Id";
        public const string POST_ID = "PostId";
        public const string HOMESECTION_ID = "HomeSectionId";
        public const string ORDER = "Order";
    }
    public interface IRepositoryHomeSectionPost
    {
        Task Borrar(int id);
        Task Crear(HomeSectionPost homeSectionPost);
        Task Editar(HomeSectionPost homeSectionPost);
        Task<IEnumerable<HomeSectionPost>> Obtener(int userId);
    }

    public class RepositoryHomeSectionPost : IRepositoryHomeSectionPost
    {
        private readonly string _connectionString;

        public RepositoryHomeSectionPost(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // --- OBTENER LISTADO DE HOME SECTION ---
        public async Task<IEnumerable<HomeSectionPost>> Obtener(int HomeSectionId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<HomeSectionPost>($@"SELECT {HOMESECTIONPOST.ID}, {HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.POST_ID} FROM {HOMESECTIONPOST.TABLA} WHERE {HOMESECTIONPOST.HOMESECTION_ID} = @{HOMESECTIONPOST.HOMESECTION_ID} ORDER BY {HOMESECTIONPOST.ORDER}", new { HomeSectionId });
        }

        // --- AGREGAR UN HOME SECTION ---
        public async Task Crear(HomeSectionPost homeSectionPost)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.ExecuteScalarAsync<int>($@"INSERT INTO {HOMESECTIONPOST.TABLA} ({HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.POST_ID}, {HOMESECTIONPOST.ORDER}) VALUES (@{HOMESECTIONPOST.HOMESECTION_ID}, @{HOMESECTIONPOST.POST_ID}, @{HOMESECTIONPOST.ORDER}); SELECT SCOPE_IDENTITY();", homeSectionPost);
            homeSectionPost.Id = id;
        }

        // --- EDITAR UN HOME SECTION ---
        public async Task Editar(HomeSectionPost homeSectionPost)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync($@"UPDATE {HOMESECTIONPOST.TABLA} SET {HOMESECTIONPOST.HOMESECTION_ID} = @{HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.POST_ID} = @{HOMESECTIONPOST.POST_ID}, {HOMESECTIONPOST.ORDER} = @{HOMESECTIONPOST.ORDER} WHERE {HOMESECTIONPOST.ID} = @{HOMESECTIONPOST.ID}", homeSectionPost);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        // --- ELIMINAR HOME SECTION ---
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"DELETE FROM {HOMESECTIONPOST.TABLA} WHERE {HOMESECTIONPOST.ID} = @{HOMESECTIONPOST.ID}", new { id });
        }
    }
}
