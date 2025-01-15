using Backend_portafolio.Models;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using System.Text.Json;


namespace Backend_portafolio.Sevices
{
    public interface ILinkService
    {
        Task CreateLink(Link link);
        Task CreateLink(IEnumerable<Link> links);
        Task<IEnumerable<Link>> GetAllLink(int postId);
        Task<List<Link>> GetLinkByPost(int postId);
        IEnumerable<Link> SerealizarJsonLink(string jsonLinks);
        List<LinkForm> SerealizarJsonLinkForm(string jsonLinks);
    }

    public class LinkService : ILinkService
    {
        private readonly IRepositoryLink _repositoryLink;

        public LinkService(
            IRepositoryLink repositoryLink
            )
        {
            _repositoryLink = repositoryLink;
        }

        public async Task<IEnumerable<Link>> GetAllLink(int postId)
        {
            return await _repositoryLink.ObtenerPorPost(postId);
        }

        public async Task<List<Link>> GetLinkByPost(int postId)
        {
            return (List<Link>)await _repositoryLink.ObtenerPorPost(postId);
        }

        public async Task CreateLink(Link link)
        {
            await _repositoryLink.Crear(link);
        }

        public async Task CreateLink(IEnumerable<Link> links)
        {
            foreach (var link in links)
            {
                await _repositoryLink.Crear(link);
            }
        }

        public List<LinkForm> SerealizarJsonLinkForm(string jsonLinks)
        {
            return JsonSerializer.Deserialize<List<LinkForm>>(jsonLinks, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public IEnumerable<Link> SerealizarJsonLink(string jsonLinks)
        {
            return JsonSerializer.Deserialize<List<Link>>(jsonLinks, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
