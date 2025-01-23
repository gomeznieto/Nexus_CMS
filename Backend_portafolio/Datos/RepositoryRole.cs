using Backend_portafolio.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Datos
{
    public interface IRepositoryRole
    {
        Task<Role> BuscarPorId(int id);
        Task<Role> BuscarPorNombre(string name);
        Task<int> Crear(Role role);
        Task Editar(Role role);
        Task Eliminar(int id);
        Task<bool> Existe(string name);
        Task<bool> Existe(int id);
        Task<IEnumerable<Role>> Obtener();
        Task<bool> SePuedeBorrar(int id);
    }

    public class RepositoryRole : IRepositoryRole
    {
        private readonly string _connectionString;

        public RepositoryRole(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        //*****************************************************
        //********************** GET **************************
        //*****************************************************

        public async Task<IEnumerable<Role>> Obtener()
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<Role>(@"SELECT * FROM role");
        }

        //*****************************************************
        //******************** CREATE *************************
        //*****************************************************

        public async Task<int> Crear(Role role)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO role (name) 
            VALUES (@name);
            SELECT SCOPE_IDENTITY();
            ", role );
            return id;
        }

        //*****************************************************
        //********************** EDIT *************************
        //*****************************************************

        public async Task Editar(Role role)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
            UPDATE role SET name = @name
            WHERE id = @id
            ", role);
        }

        //*****************************************************
        //********************* DELETE ************************
        //*****************************************************

        public async Task Eliminar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
            DELETE FROM role WHERE id = @id
            ", new { id });
        }

        public async Task<bool> SePuedeBorrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var cantidad = await connection.QueryFirstOrDefaultAsync<int>(@"
            SELECT COUNT(*) FROM users WHERE role = @id
            ", new { id });

            return cantidad == 0;
        }

        //*****************************************************
        //********************** FIND *************************
        //*****************************************************

        public async Task<Role> BuscarPorId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Role>(@"
            SELECT * FROM role WHERE id = @id
            ", new { id });
        }

        public async Task<Role> BuscarPorNombre(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Role>(@"
            SELECT * FROM role WHERE name = @name
            ", new { name });
        }

        //*****************************************************
        //********************* EXISTS ************************
        //*****************************************************

        public async Task<bool> Existe(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Role>(@"
            SELECT * FROM role WHERE name = @name
            ", new { name }) != null;
        }

        public async Task<bool> Existe(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Role>(@"
            SELECT * FROM role WHERE id = @id
            ", new { id }) != null;
        }


    }
}
