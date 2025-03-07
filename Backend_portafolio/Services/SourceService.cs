using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Services;
using Backend_portafolio.Models;
using AutoMapper;

namespace Backend_portafolio.Sevices
{
    public interface ISourceService
    {
        Task CreateSource(SourceViewModel viewModel);
        Task DeleteSource(int id);
        Task EditSource(SourceViewModel viewModel);
        Task<bool> Existe(string source);
        Task<IEnumerable<SourceViewModel>> GetAllSource();
        Task<SourceViewModel> GetSourceById(int id);
        SourceViewModel GetSourceViewModel();
    }

    public class SourceService : ISourceService
    {
        private readonly IRepositorySource _repositorySource;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUsersService _usersService;

        public SourceService(
            IRepositorySource repositorySource,
            IMapper mapper,
            IImageService imageService,
            IUsersService usersService
            )
        {
            _repositorySource = repositorySource;
            _mapper = mapper;
            this._imageService = imageService;
            _usersService = usersService;
        }


        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        public async Task<IEnumerable<SourceViewModel>> GetAllSource()
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();
                return _mapper.Map<IEnumerable<SourceViewModel>>(await _repositorySource.Obtener(userId));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<SourceViewModel> GetSourceById(int id)
        {
            try
            {
                var viewModel = _mapper.Map<SourceViewModel>(await _repositorySource.ObtenerPorId(id));

                if (viewModel is null)
                    throw new ApplicationException("El recurso solicitado no existe");

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public SourceViewModel GetSourceViewModel()
        {
            var userId = _usersService.ObtenerUsuario();
            var viewModel = new Source();
            viewModel.user_id = userId;
            return _mapper.Map<SourceViewModel>(viewModel);
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreateSource(SourceViewModel viewModel)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                UserViewModel currentUser = await _usersService.GetDataUser();

                if (userID != viewModel.user_id)
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");

                var existe = await _repositorySource.Existe(viewModel.name, userID);

                if(existe)
                    throw new ApplicationException("El recurso ya existe");

                //Subir imagen
                viewModel.icon = await _imageService.UploadImageAsync(viewModel.ImageFile, currentUser, $"source-images", viewModel.name);

                await _repositorySource.Crear(_mapper.Map<Source>(viewModel));

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditSource(SourceViewModel viewModel)
        {
           try
            {
                UserViewModel currentUser = await _usersService.GetDataUser();

                var existe = await _repositorySource.ObtenerPorId(viewModel.id);

                if (existe is null)
                    throw new ApplicationException("El recurso no existe");

                var userID = _usersService.ObtenerUsuario();

                if (userID != viewModel.user_id)
                    throw new ApplicationException("El usuario no tiene permisos para realizar esta acción");

                //Subir imagen
                viewModel.icon = await _imageService.UploadImageAsync(viewModel.ImageFile, currentUser, $"source-images", viewModel.name);

                await _repositorySource.Editar(_mapper.Map<Source>(viewModel));
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
