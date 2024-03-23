using Microsoft.Extensions.Configuration;

namespace ASPUnitTesting
{
	public static class ConfigurationHelper
	{
		public static IConfiguration GetConfiguration()
		{
			return new ConfigurationBuilder()
				.AddJsonFile("appsettings.Development.json")
				.Build();
		}
	}
}
