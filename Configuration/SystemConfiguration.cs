using Microsoft.Extensions.Configuration;

namespace FuelPriceFunction.Configuration;

public static class SystemConfiguration
{
	public static IConfigurationRoot BuildConfiguration => new ConfigurationBuilder()
		.AddJsonFile("local.settings.json", true, true)
#if DEBUG
		.AddJsonFile("local.settings.json", true, true)
#else
		.AddJsonFile(
			$"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
			true,
			true)
#endif
		.AddEnvironmentVariables()
		.Build();
}