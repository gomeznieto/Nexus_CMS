using Backend_portafolio.Entities;
using Backend_portafolio.Models;
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
        Task<bool> DoesOrderExistAsync(int Order, int HomeSectionId);
        Task<IEnumerable<HomeSectionPost>> Obtener(int userId);
        Task<HomeSectionPostModel> ObtenerPorPostId(int postId);
        Task<HomeSectionPostModel> ObtenerPorId(int postId);
        Task<IEnumerable<HomeSectionPostModel>> ObtenerPorHomeSectionId(int homeSectionId);
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

            return await connection.QueryAsync<HomeSectionPost>($@"SELECT {HOMESECTIONPOST.ID}, {HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.POST_ID} FROM {HOMESECTIONPOST.TABLA} WHERE {HOMESECTIONPOST.HOMESECTION_ID} = @{HOMESECTIONPOST.HOMESECTION_ID} ORDER BY [{HOMESECTIONPOST.ORDER}]", new { HomeSectionId });
        }

        // --- AGREGAR UN HOME SECTION ---
        public async Task Crear(HomeSectionPost homeSectionPost)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.ExecuteScalarAsync<int>($@"INSERT INTO {HOMESECTIONPOST.TABLA} ({HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.POST_ID}, [{HOMESECTIONPOST.ORDER}]) VALUES (@{HOMESECTIONPOST.HOMESECTION_ID}, @{HOMESECTIONPOST.POST_ID}, @{HOMESECTIONPOST.ORDER}); SELECT SCOPE_IDENTITY();", homeSectionPost);
            homeSectionPost.Id = id;
        }

        // --- EDITAR UN HOME SECTION ---
        public async Task Editar(HomeSectionPost homeSectionPost)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync($@"UPDATE {HOMESECTIONPOST.TABLA} SET {HOMESECTIONPOST.HOMESECTION_ID} = @{HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.POST_ID} = @{HOMESECTIONPOST.POST_ID}, [{HOMESECTIONPOST.ORDER}] = @{HOMESECTIONPOST.ORDER} WHERE {HOMESECTIONPOST.ID} = @{HOMESECTIONPOST.ID}", homeSectionPost);
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

        // --- BUSCAR SI EL NUMERO DE ORDEN ESTA SIENDO UTILIZADO ---
        public async Task<bool> DoesOrderExistAsync(int order, int homeSectionId)
        {
            const string query = $@"SELECT 1 FROM {HOMESECTIONPOST.TABLA} WHERE {HOMESECTIONPOST.HOMESECTION_ID} = @{HOMESECTIONPOST.HOMESECTION_ID} AND [{HOMESECTIONPOST.ORDER}] = @{HOMESECTIONPOST.ORDER}";

            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, new
            {
                HomeSectionId = homeSectionId,
                Order = order
            });

            return result.HasValue;
        }

        public async Task<HomeSectionPostModel> ObtenerPorPostId(int postId)
        {
            var query = $@"SELECT {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.ID}, {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.TABLA}.[{HOMESECTIONPOST.ORDER}], {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.POST_ID}, {HOMESECTION.TABLA}.{HOMESECTION.NOMBRE} 
                           FROM {HOMESECTIONPOST.TABLA} 
                           JOIN {HOMESECTION.TABLA} ON {HOMESECTION.TABLA}.{HOMESECTION.ID} = {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID}
                           WHERE {HOMESECTIONPOST.POST_ID} = @{HOMESECTIONPOST.POST_ID}";
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<HomeSectionPostModel>(query, new { PostId = postId });

            return result;

        }

        public async Task<HomeSectionPostModel> ObtenerPorId(int id)
        {
            var query = $@"SELECT {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.ID}, {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.TABLA}.[{HOMESECTIONPOST.ORDER}], {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.POST_ID}, {HOMESECTION.TABLA}.{HOMESECTION.NOMBRE} 
                           FROM {HOMESECTIONPOST.TABLA} 
                           JOIN {HOMESECTION.TABLA} ON {HOMESECTION.TABLA}.{HOMESECTION.ID} = {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID}
                           WHERE {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.ID} = @{HOMESECTIONPOST.ID}";
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<HomeSectionPostModel>(query, new { Id = id });

            return result;
        }

        public async Task<IEnumerable<HomeSectionPostModel>> ObtenerPorHomeSectionId(int homeSectionId)
        {
            var query = $@"SELECT {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.ID}, {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID}, {HOMESECTIONPOST.TABLA}.[{HOMESECTIONPOST.ORDER}], {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.POST_ID}, {HOMESECTION.TABLA}.{HOMESECTION.NOMBRE} 
                           FROM {HOMESECTIONPOST.TABLA} 
                           JOIN {HOMESECTION.TABLA} ON {HOMESECTION.TABLA}.{HOMESECTION.ID} = {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID}
                           WHERE {HOMESECTIONPOST.TABLA}.{HOMESECTIONPOST.HOMESECTION_ID} = @{HOMESECTIONPOST.HOMESECTION_ID}";
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<HomeSectionPostModel>(query, new { HomeSectionId = homeSectionId });

            return result;
        }
    }
}
