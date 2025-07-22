using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;

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
        Task<bool> UpdateAsync(HomeSectionModel section);

        // Eliminar una sección
        Task<bool> DeleteAsync(int id, int userId);
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

        public Task<bool> DeleteAsync(int id, int userId)
        {
            throw new NotImplementedException();
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

        public Task<bool> UpdateAsync(HomeSectionModel section)
        {
            throw new NotImplementedException();
        }
    }
}
