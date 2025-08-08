using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    public interface IRepositoryLayout
    {
        Task CreateLayoutSectionAsync(UserHomeLayoutSectionModel section);
        Task DeleteLayoutSectionAsync(int sectionId);
        Task<IEnumerable<UserHomeLayoutSectionModel>> GetLayoutHomeSectionsAsync(int userId);
        Task<UserHomeLayoutSectionModel> GetLayoutSectionByIdAsync(int id);
        Task UpdateLayoutSectionAsync(UserHomeLayoutSectionModel section);
    }

    public class RepositoryLayout : IRepositoryLayout
    {
        private readonly string _connectionString;

        public RepositoryLayout(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // Obtener las secciones del layout del usuario
        public async Task<IEnumerable<UserHomeLayoutSectionModel>> GetLayoutHomeSectionsAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT * FROM UserLayoutSections WHERE UserId = @UserId ORDER BY DisplayOrder";
            return await connection.QueryAsync<UserHomeLayoutSectionModel>(query, new { UserId = userId });
        }

        // Obtener una seccion del layout del usuario por ID
        public async Task<UserHomeLayoutSectionModel> GetLayoutSectionByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = "SELECT * FROM UserLayoutSections WHERE id = @Id";
            return await connection.QuerySingleOrDefaultAsync<UserHomeLayoutSectionModel>(query, new { Id = id });
        }

        // Crear una nueva seccion en el layout del usuario
        public async Task CreateLayoutSectionAsync(UserHomeLayoutSectionModel section)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                INSERT INTO UserLayoutSections (DisplayOrder, SectionType, SectionId, Title, UserId)
                VALUES (@DisplayOrder, @SectionType, @SectionId, @Title, @UserId);
				SELECT SCOPE_IDENTITY();";
            var id = await connection.QuerySingleAsync<int>(query, section);

            section.Id = id;
        }

        // Actualizar una seccion en el layout del usuario
        public async Task UpdateLayoutSectionAsync(UserHomeLayoutSectionModel section)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                UPDATE UserLayoutSections
                SET DisplayOrder = @DisplayOrder, SectionType = @SectionType, SectionId = @SectionId, Title = @Title, UserID = @UserId
                WHERE Id = @Id;";
            await connection.ExecuteAsync(query, section);
        }

        // Eliminar una seccion del layout del usuario
        public async Task DeleteLayoutSectionAsync(int sectionId)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = "DELETE FROM UserLayoutSections WHERE Id = @Id;";
            await connection.ExecuteAsync(query, new { Id = sectionId });
        }
    }
}
