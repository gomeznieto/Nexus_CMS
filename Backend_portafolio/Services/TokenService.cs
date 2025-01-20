using Backend_portafolio.Datos;
using Microsoft.IdentityModel.Tokens;

namespace Backend_portafolio.Services
{
    public interface ITokenService
    {
        string GenerateApiKey();
        Task ValidateApiKey(string token);
    }
    public class TokenService : ITokenService
    {
        private readonly IRepositoryUsers _repositoryUsers;

        public TokenService(
            IRepositoryUsers repositoryUsers
        )
        {
            _repositoryUsers = repositoryUsers;
        }

        //****************************************************
        //***************** VALIDAR TOKEN ********************
        //****************************************************

        /**
         * Valida un token de acceso
         */
        public async Task ValidateApiKey(string apiKey)
        {
            try
            {
                // Validar que el token no esté vacío
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new Exception("El token no es válido");

                //Validar cadena
                if (apiKey.Length < 10)
                    throw new Exception("El token no es válido");

                //Validar UUID
                if (!IsValidUUID(apiKey))
                    throw new Exception("El token no es válido");

                //Normalizar token
                apiKey = NormalizeToken(apiKey);

                //Validar token
                var isValid = await _repositoryUsers.ValidarApiKey(apiKey);

                if (!isValid)
                    throw new ApplicationException("Token inválido");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Genera un token de acceso
         */
        public string GenerateApiKey()
        {
            return Guid.NewGuid().ToString();
        }

        //****************************************************
        //***************** FUNCIONES TOKEN ******************
        //****************************************************

        /**
         * Valida si una cadena es un UUID
         */
        private bool IsValidUUID(string cadena)
        {
            return Guid.TryParse(cadena, out _);
        }

        /**
         * Normaliza un token
         */
        private string NormalizeToken(string token)
        {
            if(IsValidUUID(token))
                return token.ToLowerInvariant();

            throw new Exception("El token no es válido");
        }

    }
}
