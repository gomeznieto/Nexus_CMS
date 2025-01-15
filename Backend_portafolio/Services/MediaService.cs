using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using System.Text.Json;

namespace Backend_portafolio.Sevices
{
    public interface IMediaService
    {
        Task Create(Media media);
        Task Create(List<Media> medias);
        Task<IEnumerable<Media>> GetMediaByPost(int post_id);
        IEnumerable<Media> SerealizarJsonMedia(string jsonMedia);
        List<MediaForm> SerealizarJsonMediaForm(string jsonMedia);
    }
    public class MediaService : IMediaService
    {
        private readonly IRepositoryMedia _repositoryMedia;

        public MediaService(IRepositoryMedia repositoryMedia)
        {
            _repositoryMedia = repositoryMedia;
        }

        public async Task<IEnumerable<Media>> GetAllMedia(int userId)
        {
            return await _repositoryMedia.Obtener();
        }
        
        public async Task<IEnumerable<Media>> GetMediaByPost(int post_id)
        {
            return await _repositoryMedia.ObtenerPorPost(post_id);
        }

        public async Task Create (Media media)
        {
            await _repositoryMedia.Crear(media);
        }

        public async Task Create(List<Media> medias)
        {
            foreach (var media in medias)
            {
                await _repositoryMedia.Crear(media);
            }
        }
        public List<MediaForm> SerealizarJsonMediaForm(string jsonMedia)
        {
            return JsonSerializer.Deserialize<List<MediaForm>>(jsonMedia, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public IEnumerable<Media> SerealizarJsonMedia(string jsonMedia)
        {
            return JsonSerializer.Deserialize<List<Media>>(jsonMedia, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
