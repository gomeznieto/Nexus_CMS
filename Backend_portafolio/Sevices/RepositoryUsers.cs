using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend_portafolio.Sevices
{
    public interface IRepositoryUsers
    {
        Task<User> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(User user);
    }

    public class RepositoryUsers : IRepositoryUsers
    {
        private readonly string _connectionString;
        public RepositoryUsers(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // CREAR USUARIO
        public async Task<int> CrearUsuario(User user)
        {
            //user.emailNormalizado = user.email.ToUpper();
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO users (name, email, emailNormalizado, password, role) 
            VALUES (@name, @email, @emailNormalizado, @password, @role);
            ", user);

            return id;
        }

        // BUSCAR USUARIO POR EMAIL
        public async Task<User> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE emailNormalizado = @emailNormalizado", 
                new { emailNormalizado });

        }

    }
}
