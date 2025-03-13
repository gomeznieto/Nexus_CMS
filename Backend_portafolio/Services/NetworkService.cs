using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;

namespace Backend_portafolio.Services
{
    public interface INetworkService
    {
        Task CreteSocialNetwork(SocialNetworkViewModel socialNetwork);
        Task DeleteSocialNetwork(int id);
        Task EditSocialNetwork(SocialNetworkViewModel viewModel);
        Task<SocialNetworkViewModel> GetSocialNetworkById(int id);
        Task<List<SocialNetworkViewModel>> GetSocialNetworksByUserId(int userId);
        Task<SocialNetworkViewModel> GetSocialNetworkViewModel(SocialNetworkViewModel viewModel = null);
    }

    public class NetworkService : INetworkService
    {
        private readonly IRepositorySocialNetwork _repositorySocialNetwork;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IUsersService _usersService;

        public NetworkService(
            IRepositorySocialNetwork repositorySocialNetwork,
            IImageService imageService,
            IMapper mapper,
            IUsersService usersService
        )
        {
            _repositorySocialNetwork = repositorySocialNetwork;
            _imageService = imageService;
            _mapper = mapper;
            _usersService = usersService;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        /**
         * Retorna un modelo de vista de redes sociales
         * @param viewModel Modelo de vista de redes sociales
         * @return Modelo de vista de redes sociales
         */
        public async Task<SocialNetworkViewModel> GetSocialNetworkViewModel(SocialNetworkViewModel viewModel = null)
        {
            try
            {
                if (viewModel is null)
                    viewModel = new SocialNetworkViewModel();

                var userId = _usersService.ObtenerUsuario();
                viewModel.Networks = _mapper.Map<List<SocialNetwork>>(await GetSocialNetworksByUserId(userId));
                viewModel.user_id = userId;

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Retorna una lista de redes sociales por usuario
         * @param userId Id del usuario
         * @return Lista de redes sociales
         */
        public async Task<List<SocialNetworkViewModel>> GetSocialNetworksByUserId(int userId)
        {
            try
            {
                var socialNetworkList = await _repositorySocialNetwork.ObtenerPorUsuario(userId);
                return _mapper.Map<List<SocialNetworkViewModel>>(socialNetworkList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Retorna una red social por id
         * @param id Id de la red social
         * @return Red social
         */
        public async Task<SocialNetworkViewModel> GetSocialNetworkById(int id)
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();
                var socialNetwork = await _repositorySocialNetwork.ObtenerPorId(id, userId);

                if(socialNetwork is null)
                    throw new Exception("La red social no existe");

                return _mapper.Map<SocialNetworkViewModel>(socialNetwork);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreteSocialNetwork(SocialNetworkViewModel viewmodel)
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();
                var user = await _usersService.GetDataUser();

                if (viewmodel.user_id != userId)
                    throw new Exception("No puedes crear una red social para otro usuario");

                //Guardar imagen del íconos y ponerla en icon
                if (viewmodel.ImageFile != null)
                {
                    viewmodel.icon = await _imageService.UploadImageAsync(viewmodel.ImageFile, user, "social_networks", viewmodel.name);
                }

                var result = await _repositorySocialNetwork.Agregar(_mapper.Map<SocialNetwork>(viewmodel));

                if (result == 0)
                    throw new Exception("No se pudo crear la red social");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************** EDIT ************************
        //****************************************************

        public async Task EditSocialNetwork(SocialNetworkViewModel viewmodel)
        {
            try
            {
                // Verificar si el usuario autenticado es el dueño de la red social
                var userId = _usersService.ObtenerUsuario();
                var user = await _usersService.GetDataUser();

                if (viewmodel.user_id != userId)
                    throw new Exception("No puedes editar una red social de otro usuario");

                // Verificar si la red social existe
                var socialNetworkToEdit = await GetSocialNetworkById(viewmodel.id);

                if (socialNetworkToEdit is null)
                    throw new Exception("La red social no existe");

                //Modificar la imagen
                if(viewmodel.ImageFile != null)
                {
                    viewmodel.icon = await _imageService.UploadImageAsync(viewmodel.ImageFile, user, "social_networks", viewmodel.name);
                }

                //Mapear el modelo de vista a la entidad
                SocialNetwork socialNetwork = _mapper.Map(viewmodel, new SocialNetwork());

                var editado = await _repositorySocialNetwork.Editar(socialNetwork);

                // Verificar si se editó la red social
                if (!editado)
                    throw new Exception("No se pudo editar la red social");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        public async Task DeleteSocialNetwork(int id)
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();
                var socialNetworkToDelete = await GetSocialNetworkById(id);

                // Verificar si la red social existe
                if (socialNetworkToDelete is null)
                    throw new Exception("La red social no existe");

                // Verificar si el usuario autenticado es el dueño de la red social
                if (socialNetworkToDelete.user_id != userId)
                    throw new Exception("No puedes eliminar una red social de otro usuario");

                var isDelete = await _repositorySocialNetwork.Borrar(id, userId);

                // Verificar si se eliminó la red social
                if (!isDelete)
                    throw new Exception("No se pudo eliminar la red social");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //****************************************************
        //********************* FUNCTIONS ********************
        //****************************************************
    }
}
