using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Backend_portafolio.Services
{
    public interface IHomeSectionPostService
    {
        // Obtener todas las secciones del home para un usuario específico
        Task<List<HomeSectionPostModel>> GetByUserAsync(UserViewModel user);

        // Obtener todas las secciones del home para un usuario específico y una sección específica
        Task<List<HomeSectionPostModel>> GetByHomeSectionIdAsync( int homeSectionId);

        // Obtener una sección específica por Id y usuario (para editarla, validando dueño)
        Task<HomeSectionPostModel> GetByPostIdAsync(int postId);

        // Crear una nueva sección
        Task<int> CreateAsync(HomeSectionPostModel section);

        // Editar una sección existente
        Task UpdateAsync(HomeSectionPostModel section);

        // Eliminar una sección
        Task DeleteAsync(HomeSectionPostModel viewModel);
    }

    public class HomeSectionPostService : IHomeSectionPostService
    {
        private readonly IRepositoryHomeSectionPost _repositoryHomeSectionPost;
        private readonly IMapper _mapper;
        private readonly IUsersService _usersService;

        public HomeSectionPostService(
            IRepositoryHomeSectionPost repositoryHomeSectionPost,
            IMapper mapper,
            IUsersService usersService
            )
        {
            _repositoryHomeSectionPost = repositoryHomeSectionPost;
            _mapper = mapper;
            _usersService = usersService;
        }

        // --- CREATE HOME SECTION POST ---
        public async Task<int> CreateAsync(HomeSectionPostModel sectionModel)
        {
            try
            {
                var currenUser = await _usersService.GetUserViewModel();
                if (currenUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                //Verificar que no exista con el mismo nombre otro
                var existOrder = await _repositoryHomeSectionPost.DoesOrderExistAsync((int)sectionModel.Order, sectionModel.HomeSectionId);

                if (existOrder)
                    sectionModel.Order = 0;

                var section = _mapper.Map<HomeSectionPost>(sectionModel);

                await _repositoryHomeSectionPost.Crear(section);

                return section.Id;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(HomeSectionPostModel sectionModel)
        {
            try
            {
                var currenUser = await _usersService.GetUserViewModel();
                if (currenUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                //Verificar que no exista con el mismo nombre otro

                if (sectionModel?.Id is null)
                    throw new Exception("No se ha encontrado la sección");

                await _repositoryHomeSectionPost.Borrar((int)sectionModel.Id);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<HomeSectionPostModel>> GetByHomeSectionIdAsync( int homeSectionId)
        {
            try
            {
                var listHomeSection = await _repositoryHomeSectionPost.ObtenerPorHomeSectionId(homeSectionId);

                if (listHomeSection is null)
                    throw new Exception("No se han encontrado las secciones del Home");

                var listaSectionModel = _mapper.Map<IEnumerable<HomeSectionPostModel>>(listHomeSection);

                return listaSectionModel.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<HomeSectionPostModel> GetByPostIdAsync(int postId)
        {
            try
            {
                var currenUser = await _usersService.GetUserViewModel();

                if (currenUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                var homeSectionPostBD = await _repositoryHomeSectionPost.ObtenerPorPostId(postId);

                var viewModel = _mapper.Map<HomeSectionPostModel>(homeSectionPostBD);

                return viewModel;
            }
            catch (Exception)
            {
                throw new Exception("No se ha encontrado la sección del post");
            }
        }

        public async Task<List<HomeSectionPostModel>> GetByUserAsync(UserViewModel user)
        {
            try
            {
                var listHomeSection = await _repositoryHomeSectionPost.Obtener(user.id);

                if (listHomeSection is null)
                    throw new Exception("No se han encontrado las secciones del Home");

                var listaSectionModel = _mapper.Map<IEnumerable<HomeSectionPostModel>>(listHomeSection);

                return listaSectionModel.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(HomeSectionPostModel sectionModel)
        {
            try
            {
                var currentUser = await _usersService.GetUserViewModel();

                if (currentUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                //Verificar si existe la sección
                var existSection = await _repositoryHomeSectionPost.ObtenerPorId((int)sectionModel.Id);

                if (existSection is null)
                    throw new Exception("No se ha encontrado la sección");


                //Verificar si el orden existe
                var existOrder = await _repositoryHomeSectionPost.DoesOrderExistAsync((int)sectionModel.Order, sectionModel.HomeSectionId);

                if (existOrder)
                    sectionModel.Order = existSection.Order;

                var section = _mapper.Map<HomeSectionPost>(sectionModel);
                await _repositoryHomeSectionPost.Editar(section);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
