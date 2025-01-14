using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;

namespace Backend_portafolio.Sevices
{
    public interface ISourceService
    {
        Task<IEnumerable<Source>> GetAllSource(int userId);
    }
    public class SourceService : ISourceService
    {
        private readonly IRepositorySource _repositorySource;

        public SourceService(IRepositorySource repositorySource)
        {
            _repositorySource = repositorySource;
        }

        public async Task<IEnumerable<Source>> GetAllSource(int userId)
        {
            return await _repositorySource.Obtener(userId);
        }

        public async Task CreateSource(Source source)
        {
            await _repositorySource.Crear(source);
        }
    }
}
