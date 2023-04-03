using System.Threading.Tasks;
using FuelPriceFunction.Configuration;
using FuelPriceFunction.Configuration.Models;
using FuelPriceFunction.DataAccess;
using FuelPriceFunction.DataAccess.Abstraction;
using FuelPriceFunction.DataAccess.Implementation;
using FuelPriceFunction.Services.Abstraction;
using FuelPriceFunction.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FuelPriceFunction
{
	public class Program
	{
		public static async Task Main()
		{
			LogConfiguration.ConfigureLogging();
			
			var host = new HostBuilder()
				.ConfigureFunctionsWorkerDefaults()
				.ConfigureAppConfiguration((hostContext, configuration) =>
				{
					configuration.SetBasePath(hostContext.HostingEnvironment.ContentRootPath);
					configuration.AddJsonFile("local.settings.json", true, true);
					configuration.AddEnvironmentVariables();
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.AddTransient<IDataConnection, DataConnection>();
					services.AddTransient<IFuelRepository, FuelRepository>();
					services.AddTransient<IFuelPriceService, FuelPriceService>();
					services.Configure<SystemSettings>(hostContext.Configuration.GetSection("SystemSettings"));
					services.AddHttpClient("FuelPriceClient")
						.AddPolicyHandler(HttpClientConfiguration.RetryPolicy);
				})
				.UseSerilog()
				.Build();
			
			DbUpConfiguration.MigrateDatabase();

			await host.RunAsync();
		}
	}
}