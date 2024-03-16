using Backend_portafolio.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.SqlServer.Server;

namespace Backend_portafolio.Sevices
{
	struct POST
	{
		public const string TABLA = "post";
		public const string ID = "id";
		public const string TITULO = "title";
		public const string DESCRIPCION = "description";
		public const string COVER = "cover";
		public const string CATEGORIA_ID = "category_id";
		public const string USER_ID = "user_id";
		public const string FORMAT_ID = "format_id";
		public const string CREADO = "created_at";
		public const string MODIFICADO = "modify_at";
		public const string BORRADOR = "draft";
		public const string FORMAT = "format";
		public const string CATEGORIA = "category";
		public const string USER = "userName";
		public const string NOMBRE = "name";
		public const string CANTIDAD = "cantidad";
    }

	public interface IRepositoryPosts
	{
		Task Borrar(int id);
		Task Crear(Post post);
		Task Editar(Post post);
        Task EditarBorrador(int id, bool editar);
        Task<IEnumerable<Post>> Obtener();
        Task<int> ObtenerCantidadPorFormato(string formato);
        Task<IEnumerable<Post>> ObtenerPorFormato(string name, int page);
        Task<Post> ObtenerPorId(int id);
	}

	public class RepositoryPosts : IRepositoryPosts
	{
		private readonly string _connectionString;

		public RepositoryPosts(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DevConnection");
		}

		public async Task Crear(Post post)
		{
			using var connection = new SqlConnection(_connectionString);
			var id = await connection.QuerySingleAsync<int>($@"
				INSERT INTO {POST.TABLA} ({POST.TITULO}, {POST.DESCRIPCION}, {POST.COVER}, {POST.CATEGORIA_ID}, P.{POST.USER_ID}, P.{POST.FORMAT_ID}, P.{POST.CREADO}, P.{POST.BORRADOR})
				VALUES (@{POST.TITULO}, @{POST.DESCRIPCION}, @{POST.COVER}, @{POST.CATEGORIA_ID}, @{POST.USER_ID}, @{POST.FORMAT_ID}, @{POST.CREADO}, @{POST.BORRADOR});
				SELECT SCOPE_IDENTITY();",
				post);

			post.id = id;
		}

		public async Task<IEnumerable<Post>> Obtener()
		{
			using var connection = new SqlConnection(_connectionString);

			var query = $@"SELECT P.{POST.ID}, P.{POST.TITULO}, P.{POST.DESCRIPCION}, P.{POST.COVER}, P.{POST.CREADO}, P.{POST.CATEGORIA_ID}, P.{POST.FORMAT_ID}, P.{POST.USER_ID}, P.{POST.BORRADOR}, P.{POST.MODIFICADO},
						U.{POST.NOMBRE} as {POST.USER}, F.{FORMAT.NOMBRE} as {POST.FORMAT}, C.{CATEGORIA.NOMBRE} as {POST.CATEGORIA}
						FROM {POST.TABLA} P
						INNER JOIN {CATEGORIA.TABLA} C
						ON P.{POST.CATEGORIA_ID} = C.id
						INNER JOIN users U ON P.{POST.USER_ID} = U.id
						INNER JOIN {FORMAT.TABLA}
						F ON P.{POST.FORMAT_ID} = F.{FORMAT.ID}
						ORDER BY F.{FORMAT.NOMBRE}, P.{POST.CREADO} DESC;";

			return await connection.QueryAsync<Post>(query);
		}

        public async Task<IEnumerable<Post>> ObtenerPorFormato(string name, int page)
        {
            using var connection = new SqlConnection(_connectionString);

            var query = $@"SELECT P.{POST.ID}, P.{POST.TITULO}, P.{POST.DESCRIPCION}, P.{POST.COVER}, P.{POST.CREADO}, P.{POST.BORRADOR}, P.{POST.MODIFICADO},
						U.{POST.NOMBRE} as {POST.USER}, F.{FORMAT.NOMBRE} as {POST.FORMAT}, C.{CATEGORIA.NOMBRE} as {POST.CATEGORIA}
						FROM {POST.TABLA} P
						INNER JOIN {CATEGORIA.TABLA} C
						ON P.{POST.CATEGORIA_ID} = C.id
						INNER JOIN users U 
						ON P.{POST.USER_ID} = U.id
						INNER JOIN {FORMAT.TABLA}
						F ON P.{POST.FORMAT_ID} = F.{FORMAT.ID}
						WHERE F.{FORMAT.NOMBRE} = @{FORMAT.NOMBRE}
						ORDER BY P.{POST.CREADO} DESC
						OFFSET {(page-1) * 4} ROWS
						FETCH NEXT {4} ROWS ONLY;";
			 
            var postList = await connection.QueryAsync<Post>(query, new { name });
			return postList;
        }

        public async Task<Post> ObtenerPorId(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			return await connection.QueryFirstOrDefaultAsync<Post>($@"
								SELECT P.{POST.ID}, P.{POST.TITULO}, P.{POST.DESCRIPCION}, P.{POST.COVER}, P.{POST.CREADO}, P.{POST.BORRADOR}, P.{POST.MODIFICADO},
								U.{POST.NOMBRE} as {POST.USER}, F.{FORMAT.NOMBRE} as {POST.FORMAT}, C.{CATEGORIA.NOMBRE} as {POST.CATEGORIA}
								FROM {POST.TABLA} P
								INNER JOIN {CATEGORIA.TABLA} C
								ON P.{POST.CATEGORIA_ID} = C.id
								INNER JOIN users U ON P.{POST.USER_ID} = U.id
								INNER JOIN {FORMAT.TABLA}
								F ON P.{POST.FORMAT_ID} = F.{FORMAT.ID}
								WHERE P.{POST.ID} = @{POST.ID}", new { id });
		}

		public async Task<int> ObtenerCantidadPorFormato(string name)
		{
            using var connection = new SqlConnection(_connectionString);
			return await connection.QueryFirstAsync<int>($@"SELECT COUNT(P.{POST.ID}) 
															FROM {POST.TABLA} P
															INNER JOIN {FORMAT.TABLA} F
															ON P.{POST.FORMAT_ID} = F.{FORMAT.ID}
															WHERE F.{FORMAT.NOMBRE} = @{FORMAT.NOMBRE}", new { name });
        }

        public async Task Editar(Post post)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"UPDATE {POST.TABLA} 
											SET {POST.TITULO} = @{POST.TITULO}, 
											{POST.DESCRIPCION} = @{POST.DESCRIPCION}, 
											{POST.COVER} = @{POST.COVER}, 
											{POST.CATEGORIA_ID} = @{POST.CATEGORIA_ID}, 
											{POST.USER_ID} = @{POST.USER_ID}, 
											{POST.BORRADOR} = @{POST.BORRADOR},											
											{POST.FORMAT_ID} = @{POST.FORMAT_ID},
											{POST.MODIFICADO} = @{POST.MODIFICADO}
											WHERE {POST.ID} = @{POST.ID}", post);

		}

		public async Task EditarBorrador(int id, bool draft)
		{
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync($@"UPDATE {POST.TABLA} SET {POST.BORRADOR} = @{POST.BORRADOR}										
											WHERE {POST.ID} = @{POST.ID}", new {id, draft});
        }


        public async Task Borrar(int id)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.ExecuteAsync($@"DELETE {POST.TABLA} WHERE {POST.ID} = @{POST.ID}", new { id });
		}
	}
}
