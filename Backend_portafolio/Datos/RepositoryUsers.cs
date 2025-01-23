using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Backend_portafolio.Entities;


namespace Backend_portafolio.Datos
{
    public interface IRepositoryUsers
    {
        Task EditarUsuario(User user);
        Task<bool> EditarPass(User user, string nuevaPass);
        Task<bool> ExistEmail(string emailNormalizado);
        Task<bool> ExistUsername(string emailNormalizado);
        Task<bool> ValidarApiKey(string apiKey);
        Task<int> CrearUsuario(User user);
        Task<User> BuscarPorId(int id);
        Task<User> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<User> BuscarUsuarioPorUsername(string usernameNormalizado);
        Task<User> ObtenerUsuarioPorApiKey(string apiKey);
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
            INSERT INTO users (name, username, usernameNormalizado, email, emailNormalizado, passwordHash, role, apiKey) 
            VALUES (@name, @username, @usernameNormalizado, @email, @emailNormalizado, @passwordHash, @role, @apiKey);
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

        // BUSCAR USUARIO POR USERNAME
        public async Task<User> BuscarUsuarioPorUsername(string usernameNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);
            var user = await connection.QuerySingleOrDefaultAsync<User>(
               "SELECT * FROM users WHERE usernameNormalizado = @usernameNormalizado",
               new { usernameNormalizado });

            return user;
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
        public async Task<bool> ExistEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);

            emailNormalizado = emailNormalizado.ToUpper();

            var user = await connection.QuerySingleOrDefaultAsync<User>(
               "SELECT * FROM users WHERE emailNormalizado = @emailNormalizado",
               new { emailNormalizado });

            return user != null;

        }

        public async Task<bool> ExistUsername(string usernameNormalizado)
        {
            using var connection = new SqlConnection(_connectionString);

            usernameNormalizado = usernameNormalizado.ToUpper();

            var user = await connection.QuerySingleOrDefaultAsync<User>(
               "SELECT * FROM users WHERE usernameNormalizado = @usernameNormalizado",
               new { usernameNormalizado });

            return user != null;

        }

        public async Task<bool> ValidarApiKey(string apiKey)
        {
            using var connection = new SqlConnection(_connectionString);

            var user = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE apiKey = @apiKey",
                new { apiKey });

            return user != null;
        }

        public async Task<User> ObtenerUsuarioPorApiKey(string apiKey)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var user = await connection.QuerySingleOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE apiKey = @apiKey",
                    new { apiKey });

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
