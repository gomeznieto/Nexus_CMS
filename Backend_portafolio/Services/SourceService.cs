using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Services;

namespace Backend_portafolio.Sevices
{
    public interface ISourceService
    {
        Task CreateSource(Source viewModel);
        Task DeleteSource(int id);
        Task EditSource(Source viewModel);
        Task<bool> Existe(string source);
        Task<IEnumerable<Source>> GetAllSource();
        Task<Source> GetSourceById(int id);
        Source GetSourceViewModel();
    }

    public class SourceService : ISourceService
    {
        private readonly IRepositorySource _repositorySource;
        private readonly IUsersService _usersService;

        public SourceService(
            IRepositorySource repositorySource,
            IUsersService usersService
            )
        {
            _repositorySource = repositorySource;
            _usersService = usersService;
        }


        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        public async Task<IEnumerable<Source>> GetAllSource()
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();
                return await _repositorySource.Obtener(userId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<Source> GetSourceById(int id)
        {
            try
            {
                var viewModel = await _repositorySource.ObtenerPorId(id);

                if (viewModel is null)
                    throw new ApplicationException("El recurso solicitado no existe");

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public Source GetSourceViewModel()
        {
            var userId = _usersService.ObtenerUsuario();
            var viewModel = new Source();
            viewModel.user_id = userId;
            return viewModel;
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreateSource(Source viewModel)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                if(userID != viewModel.user_id)
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");

                var existe = await _repositorySource.Existe(viewModel.name, userID);

                if(!existe)
                    throw new ApplicationException("El recurso ya existe");

                await _repositorySource.Crear(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditSource(Source viewModel)
        {
           try
            {
                var existe = await _repositorySource.ObtenerPorId(viewModel.id);

                if (existe != null)
                    throw new ApplicationException("El recurso no existe");

                var userID = _usersService.ObtenerUsuario();

                if (userID != viewModel.user_id)
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");

                await _repositorySource.Editar(viewModel);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }


        //****************************************************
        //*********************** DELETE *********************
        //****************************************************

        public async Task DeleteSource(int id)
        {
            try
            {
                // Validar si existe
                var existe = await _repositorySource.ObtenerPorId(id);
                if (existe is null)
                    throw new ApplicationException("El recurso no existe");

                //validar si el usuario es el mismo
                var userID = _usersService.ObtenerUsuario();
                if (userID != existe.user_id)
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");

                //Validar si se puede borrar
                var borrarTipo = await _repositorySource.sePuedeBorrar(id);
                if (!borrarTipo)
                    throw new ApplicationException("El recurso se encuentra en uso");

                await _repositorySource.Borrar(id, userID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************

        public async Task<bool> Existe(string source)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();

                var existe = await _repositorySource.Existe(source, userID);
                if (existe)
                    throw new ApplicationException($"El nombre {source} ya existe!");

                return existe;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}
