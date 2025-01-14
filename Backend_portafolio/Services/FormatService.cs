using Backend_portafolio.Models;
using Backend_portafolio.Entities;
using Backend_portafolio.Datos;

namespace Backend_portafolio.Sevices
{
    public interface IFormatService
    {
        Task CreateFormat(Format format);
        Task DeleteFormat(int id);
        Task EditFormat(Format format);
        Task<IEnumerable<Format>> GetAllFormat(int userId);
        Task<Format> GetFormatById(int id);
    }

    public class FormatService : IFormatService
    {
        private readonly IRepositoryFormat _repositoryFormat;

        public FormatService(IRepositoryFormat repositoryFormat)
        {
            _repositoryFormat = repositoryFormat;
        }

        public async Task<IEnumerable<Format>> GetAllFormat(int userId)
        {
            return await _repositoryFormat.Obtener(userId);
        }

        public async Task CreateFormat(Format format)
        {
            await _repositoryFormat.Crear(format);
        }

        public async Task EditFormat(Format format)
        {
            await _repositoryFormat.Editar(format);
        }

        public async Task DeleteFormat(int id)
        {
            await _repositoryFormat.Borrar(id);
        }

        public async Task<Format> GetFormatById(int id)
        {
            return await _repositoryFormat.ObtenerPorId(id);
        }
    }
}
