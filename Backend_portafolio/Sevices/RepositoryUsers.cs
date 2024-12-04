using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    public interface IRepositoryUsers
    {
        Task<User> BuscarPorId(int id);
        Task<User> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(User user);
    }

    public class RepositoryUsers : IRepositoryUsers
    {
        private readonly string _connectionString;
        public RepositoryUsers(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // CREAR USUARIO
        public async Task<int> CrearUsuario(User user)
        {
            //user.emailNormalizado = user.email.ToUpper();
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO users (name, email, emailNormalizado, passwordHash, role) 
            VALUES (@name, @email, @emailNormalizado, @passwordHash, @role);
            SELECT SCOPE_IDENTITY();
            ", user);

            return id;
        }

        // BUSCAR USUARIO POR EMAIL
        public async Task<User> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);
             var user = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE emailNormalizado = @emailNormalizado", 
                new { emailNormalizado });

            return user;
        }

        public async Task<User> BuscarPorId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var user = await connection.QuerySingleOrDefaultAsync<User>(
               "SELECT * FROM users WHERE id = @id",
               new { id });

            return user;
        }


    }
}
