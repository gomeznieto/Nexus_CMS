namespace Backend_portafolio.Sevices
{
	public interface IUsersService
	{
		int ObtenerUsuario();
	}

	public class UsersService : IUsersService
	{
        public UsersService()
        {
            
        }

        public int ObtenerUsuario()
        {
            return 1;
        }
    }
}
