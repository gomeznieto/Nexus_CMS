using System.Security.Claims;

namespace Backend_portafolio.Sevices
{
	public interface IUsersService
	{
		int ObtenerUsuario();
	}

	public class UsersService : IUsersService
	{
        private readonly HttpContext _httpContextAccessor;

        public UsersService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor.HttpContext;
        }

        public int ObtenerUsuario()
        {
            if(_httpContextAccessor.User.Identity.IsAuthenticated)
            {
                var idClaim = _httpContextAccessor.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }
        }
    }
}
