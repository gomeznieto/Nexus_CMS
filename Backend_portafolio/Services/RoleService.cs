using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;

namespace Backend_portafolio.Services
{
    public interface IRoleService
    {
        Task VerifyRole(string role);
        Task<RoleViewModel> GetRolesViewModel();
        Task CreateRole(RoleViewModel viewModel);
        Task DeleteRole(int id);
        Task EditRole(RoleViewModel viewModel);
        Task<Role> GetRoleById(int id);
        Task<RoleViewModel> GetRoleByName(string roleName);
        Task CreateAdminRole();
    }
    public class RoleService : IRoleService
    {
        private readonly IRepositoryRole _repositoryRole;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public RoleService(
            IRepositoryRole repositoryRole,
            IUsersService usersService,
            IMapper mapper
        )
        {
            _repositoryRole = repositoryRole;
            _usersService = usersService;
            _mapper = mapper;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        public async Task<RoleViewModel> GetRolesViewModel()
        {
            try
            {
                var roles = (await _repositoryRole.Obtener()).ToList();

                PasarMayusculas(ref roles);

                return new RoleViewModel()
                {
                    Roles = roles.ToList(),
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Role> GetRoleById(int id)
        {
            try
            {
                var user = await _usersService.GetDataUser();
                if (user.role != 1)
                    throw new ApplicationException("No tienes permisos para obtener un rol");

                var role = await _repositoryRole.BuscarPorId(id);

                if (role == null)
                    throw new ApplicationException("Rol no encontrado");

                return role;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleViewModel>GetRoleByName(string roleName)
        {
            try
            {
                var role = await _repositoryRole.BuscarPorNombre(roleName);

                if (role == null)
                    return null;


                var roleViewModel = _mapper.Map<RoleViewModel>(role);
                return roleViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreateRole(RoleViewModel viewModel)
        {
            try
            {
                var user = await _usersService.GetDataUser();

                if (user.role != 1)
                    throw new ApplicationException("No tienes permisos para crear un nuevo rol");

                var role = _mapper.Map<Role>(viewModel);

                role.name = role.name.ToLower();

                await _repositoryRole.Crear(role);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateAdminRole()
        {

            try
            {
                var isThereAdminRole = await _repositoryRole.BuscarPorNombre("admin");

                if (isThereAdminRole != null)
                    return;

                var role = new Role()
                {
                    name = "admin"
                };

                await _repositoryRole.Crear(role);

            }
            catch (Exception)
            {

                throw;
            }
        }

        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        public async Task DeleteRole(int id)
        {
            try
            {
                var user = await _usersService.GetDataUser();
                if (user.role != 1)
                    throw new ApplicationException("No tienes permisos para eliminar un rol");

                var sePuedeBorrar = await _repositoryRole.SePuedeBorrar(id);

                if (!sePuedeBorrar)
                    throw new ApplicationException("No se puede eliminar el rol");

                await _repositoryRole.Eliminar(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** EDIT ************************
        //****************************************************

        public async Task EditRole(RoleViewModel viewModel)
        {
            try
            {
                var user = await _usersService.GetDataUser();
                if (user.role != 1)

                    throw new ApplicationException("No tienes permisos para editar un rol");

                var role = _mapper.Map<Role>(viewModel);

                await _repositoryRole.Editar(role);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************

        /**
         * Función que pasa a mayúsculas los nombres de los roles
         * @param roles
         */
        private void PasarMayusculas(ref List<Role> roles)
        {
            foreach (var rol in roles)
            {
                rol.name = rol.name.ToUpper();
            }
        }

        public async Task VerifyRole(string role)
        {
            try
            {
                var result = await _repositoryRole.Existe(role);

                if (result)
                    throw new ApplicationException("El rol ya existe. Intente con otro nombre.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
