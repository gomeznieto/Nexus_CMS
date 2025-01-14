using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;

namespace Backend_portafolio.Sevices
{
    public interface IMediaTypeService
    {
        Task<IEnumerable<MediaType>> GetAllMediaType(int userId);
    }
    public class MediaTypeService : IMediaTypeService
    {
        private readonly IRepositoryMediatype _repositoryMediatype;

        public MediaTypeService(
            IRepositoryMediatype repositoryMediatype
            )
        {
            _repositoryMediatype = repositoryMediatype;
        }

        public async Task<IEnumerable<MediaType>> GetAllMediaType(int userId)
        {
            return await _repositoryMediatype.Obtener(userId);
        }
    }
}
