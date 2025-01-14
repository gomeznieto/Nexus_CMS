using Backend_portafolio.Models;
using Backend_portafolio.Entities;
using Backend_portafolio.Datos;

namespace Backend_portafolio.Sevices
{
    public interface IFormatService
    {
        Task<IEnumerable<Format>> GetAllFormat(int userId);
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
    }
}
