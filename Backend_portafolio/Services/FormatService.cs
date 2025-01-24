using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Backend_portafolio.Services;

namespace Backend_portafolio.Sevices
{
    public interface IFormatService
    {
        Task CreateFormat(Format format);
        Task DeleteFormat(int id);
        Task EditFormat(Format format);
        Task<bool> Existe(string name);
        Task<List<Format>> GetAllFormat(int userID = 0);
        Task<Format> GetFormatById(int id);
        Format GetFormatViewModel();
    }

    public class FormatService : IFormatService
    {
        private readonly IRepositoryFormat _repositoryFormat;
        private readonly IUsersService _usersService;
        private readonly HttpContext _httpContext;
        public FormatService(
            IRepositoryFormat repositoryFormat,
            IHttpContextAccessor httpContextAccessor,
           IUsersService usersService
        )
        {
            _repositoryFormat = repositoryFormat;
            _usersService = usersService;
            _httpContext = httpContextAccessor.HttpContext;

        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************

        // Obtener todos los formatos
        public async Task<List<Format>> GetAllFormat(int userID = 0)
        {
            if (userID == 0)
                userID = _usersService.ObtenerUsuario();

            return (await _repositoryFormat.Obtener(userID)).ToList();
        }

        // Obtener un formato por id
        public async Task<Format> GetFormatById(int id)
        {
            try
            {
                var format = await _repositoryFormat.ObtenerPorId(id);

                if (format == null)
                    throw new Exception("Formato no encontrado");

                return format;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Obtener un formato para la vista con el usuario actual
        public Format GetFormatViewModel()
        {
            var viewModel = new Format();
            var userId = _usersService.ObtenerUsuario();
            viewModel.user_id = userId;

            return viewModel;
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        // Crear un formato
        public async Task CreateFormat(Format format)
        {
            try
            {
                var userId = _usersService.ObtenerUsuario();

                if (userId != format.user_id)
                    throw new Exception("No puedes crear un formato para otro usuario");

                await _repositoryFormat.Crear(format);

                //Actualizar Session de Formatos para barra de navegacion
                await Helper.Session.UpdateSession(_httpContext, this, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************


        // Editar un formato
        public async Task EditFormat(Format format)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                if (userID != format.user_id)
                    throw new Exception("No puedes editar un formato de otro usuario");

                var existeFormato = GetFormatById(format.id);
                if (existeFormato == null)
                    throw new Exception("Formato no encontrado");

                await _repositoryFormat.Editar(format);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        // Borrar un formato
        public async Task DeleteFormat(int id)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                var format = await GetFormatById(id);

                // Verificar si existe el formato
                if (format == null)
                    throw new Exception("Formato no encontrado");

                // Verificar si el formato pertenece al usuario
                if (userID != format.user_id)
                    throw new Exception("No puedes borrar un formato de otro usuario");

                // Verificar si se puede borrar
                var borrar = await _repositoryFormat.sePuedeBorrar(id);
                if (!borrar)
                    throw new Exception("No se puede borrar porque el formato se encuentra en uso");

                await _repositoryFormat.Borrar(id);

                // Actualizar Session de Formatos para barra de navegacion
                await Helper.Session.UpdateSession(_httpContext, this, userID);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //****************************************************
        //********************* FUNCIONES ********************
        //****************************************************

        public async Task<bool> Existe(string name)
        {
            try
            {
                var userID = _usersService.ObtenerUsuario();
                var existsFormat = await _repositoryFormat.Existe(name, userID);
                return existsFormat;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
