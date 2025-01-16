using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Services;

namespace Backend_portafolio.Sevices
{
    public interface IMediaTypeService
    {
        Task CreateMediaType(MediaType viewModel);
        Task DeleteMediaType(int id);
        Task EditMediaType(MediaType viewModel);
        Task<bool> ExisteMediaType(string mediaType);
        Task<IEnumerable<MediaType>> GetAllMediaType();
        Task<MediaType> GetMediaTypeById(int id);
        MediaType GetMediaTypeViewModel();
    }
    public class MediaTypeService : IMediaTypeService
    {
        private readonly IRepositoryMediatype _repositoryMediatype;
        private readonly IUsersService _usersService;

        public MediaTypeService(
            IRepositoryMediatype repositoryMediatype,
            IUsersService usersService
            )
        {
            _repositoryMediatype = repositoryMediatype;
            _usersService = usersService;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************
        public async Task<IEnumerable<MediaType>> GetAllMediaType()
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                return await _repositoryMediatype.Obtener(userID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<MediaType> GetMediaTypeById(int id)
        {
            try
            {
                return await _repositoryMediatype.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public MediaType GetMediaTypeViewModel()
        {
            var userID = _usersService.ObtenerUsuario();
            var viewModel = new MediaType();
            viewModel.user_id = userID;
            return viewModel;
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************
        public async Task CreateMediaType(MediaType viewModel)
        {
            try
            {
                // Validar usuario
                var userID = _usersService.ObtenerUsuario();
                if (userID != viewModel.user_id)
                {
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");
                }

                // Validar que exista
                var existe = await _repositoryMediatype.Existe(viewModel.name, userID);
                if (existe)
                {
                    throw new ApplicationException("El media type ya existe");
                }

                await _repositoryMediatype.Crear(viewModel);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task CreateMediaType(List<MediaType> mediaType)
        {
            foreach (var media in mediaType)
            {
                await _repositoryMediatype.Crear(media);
            }
        }

        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditMediaType(MediaType viewModel)
        {
            try
            {
                // Verificar usuario
                var userID = _usersService.ObtenerUsuario();

                if (userID != viewModel.user_id)
                {
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");
                }

                // Verificar que exista
                var existe = await _repositoryMediatype.Existe(viewModel.name, userID);

                if (existe)
                {
                    throw new ApplicationException("El media type ya existe");
                }

                await _repositoryMediatype.Editar(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        //****************************************************
        //*********************** DELETE *********************
        //****************************************************

        public async Task DeleteMediaType(int id)
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();
                var mediaType = await _repositoryMediatype.ObtenerPorId(id);

                if (userId != mediaType.user_id)
                {
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");
                }

                if (mediaType == null)
                {
                    throw new ApplicationException("El media type no existe");
                }

                //Verificar si se encuentra en uso
                var borrarTipo = await _repositoryMediatype.sePuedeBorrar(id);

                if (!borrarTipo)
                {
                    throw new ApplicationException("El media type se encuentra en uso");
                }

                await _repositoryMediatype.Borrar(id);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }


            //****************************************************
            //********************* FUNCIONES ********************
            //****************************************************

        public async Task<bool> ExisteMediaType(string mediaType)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                
                var existeMediaType = await _repositoryMediatype.Existe(mediaType, userID);

                if(existeMediaType)
                    throw new ApplicationException($"El tipo de media {mediaType} ya se encuentra creado");

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}
