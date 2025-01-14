using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using System.Text.Json;

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

        public async Task CreateMediaType(MediaType mediaType)
        {
            await _repositoryMediatype.Crear(mediaType);
        }

        public async Task CreateMediaType(List<MediaType> mediaType)
        {
            foreach (var media in mediaType)
            {
                await _repositoryMediatype.Crear(media);
            }
        }
    }
}
