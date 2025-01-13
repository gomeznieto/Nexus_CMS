namespace Backend_portafolio.Helper
{
    public static class ApiKey
    {
        public static string GenerateApiKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
