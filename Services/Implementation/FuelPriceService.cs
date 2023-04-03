using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FuelPriceFunction.Configuration.Models;
using FuelPriceFunction.DataAccess.Abstraction;
using FuelPriceFunction.Domain.DTO;
using FuelPriceFunction.Domain.Entities;
using FuelPriceFunction.Services.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuelPriceFunction.Services.Implementation;

public class FuelPriceService : IFuelPriceService
{
	private readonly IFuelRepository _fuelRepository;
	private readonly ILogger<FuelPriceService> _logger;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly SystemSettings _systemSettings;
 
	public FuelPriceService(
		IFuelRepository fuelRepository,
		ILogger<FuelPriceService> logger,
		IHttpClientFactory httpClientFactory,
		IOptions<SystemSettings> systemSettings)
	{
		_fuelRepository = fuelRepository;
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_systemSettings = systemSettings.Value;
	}
	
	public async Task SavePrices()
	{
		var prices = await FetchFuelPrices();
		if (prices?.Response is null || !prices.Response.Data.Any())
		{
			return;
		}

		var actualPrices = prices.Response.Data
			.Where(item => item.Value.HasValue 
				&& item.Period.Date >= DateTime.UtcNow.Date.AddDays(_systemSettings.RetentionPeriodDays * -1).Date)
			.ToList();

		if (!actualPrices.Any())
		{
			return;
		}

		await _fuelRepository.SaveFuelPrice(actualPrices
				.Select(item => new FuelPrice(
					item.Period,
					item.Value!.Value)));
		
		_logger.LogInformation("Prices updated");
	}

	private async Task<FuelPriceApiResponseWrapper> FetchFuelPrices()
	{
		var httpClient = _httpClientFactory.CreateClient("FuelPriceClient");
		try
		{
			var response = await httpClient.GetAsync(
				_systemSettings.FetchDataUrl);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<FuelPriceApiResponseWrapper>();
			}
			
			_logger.LogCritical("Count not fetch data. Reasons: {Reasons}", response.ReasonPhrase);
			throw new Exception("Failed attempt to load data");
		}
		catch (Exception e)
		{
			_logger.LogCritical("Count not fetch data. Reasons: {Reasons}", e.Message);
			throw;
		}
	}
}