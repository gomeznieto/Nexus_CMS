using Backend_portafolio.Entities;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace Backend_portafolio.Sevices
{
    public interface IRepositoryUsers
    {
        Task<User> BuscarPorId(int id);
        Task<User> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(User user);
        Task<bool> EditarPass(User user, string nuevaPass);
        Task EditarUsuario(User user);
        Task<bool> Existe(string emailNormalizado);
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

        //EDITAR USUARIO
        public async Task EditarUsuario(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
            UPDATE users SET name = @name, email = @email, emailNormalizado = @emailNormalizado, cv = @cv, about = @about, hobbies = @hobbies, img = @img, role = @role
            WHERE id = @id
            ", user);
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

        // BUSCAR POR ID
        public async Task<User> BuscarPorId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var user = await connection.QuerySingleOrDefaultAsync<User>(
               "SELECT * FROM users WHERE id = @id",
               new { id });

            return user;
        }

        // CAMBIAR PASS
        public async Task<bool> EditarPass(User user, string nuevaPass)
        {
            var query = "UPDATE users SET passwordHash = @passwordHash, securityStamp = NEWID() WHERE Id = @id";

            using (var connection = new SqlConnection(_connectionString))
            {
                var hashedPassword = new PasswordHasher<User>().HashPassword(user, nuevaPass);

               var modificado = await connection.ExecuteAsync(query, new { id = user.id, passwordHash = hashedPassword });

                return modificado > 0;
            }
        }

        // EXISTE
        public async Task<bool> Existe(string emailNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);

            emailNormalizado = emailNormalizado.ToUpper();

            var user = await connection.QuerySingleOrDefaultAsync<User>(
               "SELECT * FROM users WHERE emailNormalizado = @emailNormalizado",
               new { emailNormalizado });

            return user != null;

        }
    }
}
