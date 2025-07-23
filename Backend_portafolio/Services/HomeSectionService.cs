using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Backend_portafolio.Services
{
    public interface IHomeSectionService
    {
        // Obtener todas las secciones del home para un usuario específico
        Task<List<HomeSectionModel>> GetByUserAsync(int userId);

        // Obtener una sección específica por Id y usuario (para editarla, validando dueño)
        Task<HomeSectionModel?> GetByIdAsync(int id, int userId);

        // Crear una nueva sección
        Task<int> CreateAsync(HomeSectionModel section);

        // Editar una sección existente
        Task UpdateAsync(HomeSectionModel section);

        // Eliminar una sección
        Task DeleteAsync(HomeSectionModel viewModel);
    }

    public class HomeSectionService : IHomeSectionService
    {
        private readonly IRepositoryHomeSection _repositoryHomeSection;
        private readonly IMapper _mapper;
        private readonly IUsersService _usersService;

        public HomeSectionService(
            IRepositoryHomeSection repositoryHomeSection,
            IMapper mapper,
            IUsersService usersService
            )
        {
            _repositoryHomeSection = repositoryHomeSection;
            _mapper = mapper;
            _usersService = usersService;
        }

        public async Task<int> CreateAsync(HomeSectionModel sectionModel)
        {
            try
            {
                var currenUser = await _usersService.GetUserViewModel();
                if (currenUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                //Verificar que no exista con el mismo nombre otro
                var existOrder = await _repositoryHomeSection.GetOrderAsync((int)sectionModel.Order, currenUser.id);

                if (existOrder)
                    sectionModel.Order = 0;

                sectionModel.UserId = currenUser.id;
                var section = _mapper.Map<HomeSection>(sectionModel);
                await _repositoryHomeSection.Crear(section);

                return section.Id;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(HomeSectionModel sectionModel)
        {
            try
            {
                var currenUser = await _usersService.GetUserViewModel();
                if (currenUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                //Verificar que no exista con el mismo nombre otro

                if(sectionModel?.Id is null)
                    throw new Exception("No se ha encontrado la sección");

                var section = _mapper.Map<HomeSection>(sectionModel);

                await _repositoryHomeSection.Borrar((int)sectionModel.Id);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public Task<HomeSectionModel> GetByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HomeSectionModel>> GetByUserAsync(int userId)
        {
            try
            {
                var currenUser = await _usersService.GetUserViewModel();
                if (currenUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                var listHomeSection = await _repositoryHomeSection.Obtener(userId);

                if (listHomeSection is null)
                    throw new Exception("No se han encontrado las secciones del Home");

                var listaSectionModel = _mapper.Map<IEnumerable<HomeSectionModel>>(listHomeSection);

                return listaSectionModel.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(HomeSectionModel sectionModel)
        {
            try
            {
                var currentUser = await _usersService.GetUserViewModel();

                if (currentUser is null)
                    throw new Exception("No se ha encontrado el usuario");

                sectionModel.UserId = currentUser.id;

                //Verificar si existe la sección
                var existSection = await _repositoryHomeSection.Obtener((int)sectionModel.Id, currentUser.id);

                if(existSection is null)
                    throw new Exception("No se ha encontrado la sección");


                //Verificar si el orden existe
                var existOrder = await _repositoryHomeSection.GetOrderAsync((int)sectionModel.Order, currentUser.id);

                if (existOrder)
                    sectionModel.Order = existSection.Order;

                var section = _mapper.Map<HomeSection>(sectionModel);
                await _repositoryHomeSection.Editar(section);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
