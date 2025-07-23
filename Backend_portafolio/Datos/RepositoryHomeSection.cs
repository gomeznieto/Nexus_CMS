using Backend_portafolio.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    struct HOMESECTION
    {
        public const string TABLA = "HomeSection";
        public const string ID = "Id";
        public const string NOMBRE = "Name";
        public const string USER_ID = "UserId";
        public const string MAX_ITEMS = "MaxItems";
        public const string ORDER = "Order";
    }
    public interface IRepositoryHomeSection
    {
        Task Borrar(int id);
        Task Crear(HomeSection homeSection);
        Task Editar(HomeSection homeSection);
        Task<IEnumerable<HomeSection>> Obtener(int userId);
        Task<HomeSection> Obtener(int id, int userId);
        Task<bool> GetOrderAsync(int order, int userId);
    }

    public class RepositoryHomeSection : IRepositoryHomeSection
    {
        private readonly string _connectionString;

        public RepositoryHomeSection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // --- OBTENER LISTADO DE HOME SECTION ---
        public async Task<IEnumerable<HomeSection>> Obtener(int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<HomeSection>($@"SELECT {HOMESECTION.ID}, {HOMESECTION.NOMBRE}, {HOMESECTION.USER_ID}, [{HOMESECTION.ORDER}], {HOMESECTION.MAX_ITEMS} FROM {HOMESECTION.TABLA} WHERE {HOMESECTION.USER_ID} = @{HOMESECTION.USER_ID} ORDER BY {HOMESECTION.NOMBRE}", new { userId });
        }

        // --- AGREGAR UN HOME SECTION ---
        public async Task Crear(HomeSection homeSection)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.ExecuteScalarAsync<int>($@"INSERT INTO {HOMESECTION.TABLA} ({HOMESECTION.NOMBRE}, {HOMESECTION.USER_ID}, {HOMESECTION.MAX_ITEMS}, [{HOMESECTION.ORDER}]) VALUES (@{HOMESECTION.NOMBRE}, @{HOMESECTION.USER_ID}, @{HOMESECTION.MAX_ITEMS}, @{HOMESECTION.ORDER}); SELECT SCOPE_IDENTITY();", homeSection);
            homeSection.Id = id;
        }

        // --- EDITAR UN HOME SECTION ---
        public async Task Editar(HomeSection homeSection)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync($@"UPDATE {HOMESECTION.TABLA} SET {HOMESECTION.NOMBRE} = @{HOMESECTION.NOMBRE}, {HOMESECTION.MAX_ITEMS} = @{HOMESECTION.MAX_ITEMS}, [{HOMESECTION.ORDER}] = @{HOMESECTION.ORDER} WHERE {HOMESECTION.ID} = @{HOMESECTION.ID} AND {HOMESECTION.USER_ID} = @{HOMESECTION.USER_ID}", homeSection);
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
            await connection.ExecuteAsync($@"DELETE FROM {HOMESECTION.TABLA} WHERE {HOMESECTION.ID} = @{HOMESECTION.ID}", new { id });
        }

        public async Task<bool> GetOrderAsync(int Order, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var section = await connection.QueryFirstAsync<int>($@"SELECT 1 FROM {HOMESECTION.TABLA} WHERE {HOMESECTION.USER_ID} = @{HOMESECTION.USER_ID} AND [{HOMESECTION.ORDER}] = @{HOMESECTION.ORDER}", new { userId, Order });

            return section == 1;
        }

        public async Task<HomeSection> Obtener(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstAsync<HomeSection>($@"SELECT {HOMESECTION.ID}, {HOMESECTION.NOMBRE}, {HOMESECTION.USER_ID}, [{HOMESECTION.ORDER}], {HOMESECTION.MAX_ITEMS} FROM {HOMESECTION.TABLA} WHERE {HOMESECTION.USER_ID} = @{HOMESECTION.USER_ID} AND {HOMESECTION.ID} = @{HOMESECTION.ID}", new { id, userId });
        }
    }
}
