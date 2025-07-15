using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Backend_portafolio.Services;

namespace Backend_portafolio.Sevices
{
    public interface IMediaTypeService
    {
        Task CreateMediaType(MediaTypeViewModel viewModel);
        Task DeleteMediaType(int id);
        Task EditMediaType(MediaTypeViewModel viewModel);
        Task<bool> ExisteMediaType(string mediaType);
        Task<IEnumerable<MediaTypeViewModel>> GetAllMediaType(int userID = 0);
        IEnumerable<MediaTypeDefaults> GetMediaTypeDefault(int userID = 0);
        Task<MediaTypeViewModel> GetMediaTypeById(int id);
        MediaTypeViewModel GetMediaTypeViewModel();
    }
    public class MediaTypeService : IMediaTypeService
    {
        private readonly IRepositoryMediatype _repositoryMediatype;
        private readonly IMapper _mapper;
        private readonly IUsersService _usersService;

        public MediaTypeService(
            IRepositoryMediatype repositoryMediatype,
            IMapper mapper,
            IUsersService usersService
            )
        {
            _repositoryMediatype = repositoryMediatype;
            _mapper = mapper;
            _usersService = usersService;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************
        public async Task<IEnumerable<MediaTypeViewModel>> GetAllMediaType(int userID = 0)
        {
            try
            {
                if(userID == 0)
                    userID = _usersService.ObtenerUsuario();

                return _mapper.Map<IEnumerable<MediaTypeViewModel>>(await _repositoryMediatype.Obtener(userID));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<MediaTypeViewModel> GetMediaTypeById(int id)
        {
            try
            {
                return _mapper.Map<MediaTypeViewModel>(await _repositoryMediatype.ObtenerPorId(id));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public MediaTypeViewModel GetMediaTypeViewModel()
        {
            var userID = _usersService.ObtenerUsuario();
            var viewModel = new MediaType();
            viewModel.user_id = userID;
            return _mapper.Map<MediaTypeViewModel>(viewModel);
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************
        public async Task CreateMediaType(MediaTypeViewModel viewModel)
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

                await _repositoryMediatype.Crear(_mapper.Map<MediaType>(viewModel));
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

        public async Task EditMediaType(MediaTypeViewModel viewModel)
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

                await _repositoryMediatype.Editar(_mapper.Map<MediaType>(viewModel));

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

        public IEnumerable<MediaTypeDefaults> GetMediaTypeDefault(int userID = 0)
        {
            try
            {
                if (userID == 0)
                    userID = _usersService.ObtenerUsuario();

                return new List<MediaTypeDefaults>()
                {
                    new MediaTypeDefaults() {
                        Name = "Image"
                    },
                    new MediaTypeDefaults() {
                        Name = "Video"
                    },
                    new MediaTypeDefaults() {
                        Name = "Sound"
                    },
                    new MediaTypeDefaults() {
                        Name = "Archive"
                    },
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
