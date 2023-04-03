using System;
using System.Threading.Tasks;
using FuelPriceFunction.Services.Abstraction;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FuelPriceFunction;

public class SaveFuelPrice
{
	private readonly ILogger<SaveFuelPrice> _logger;
	private readonly IFuelPriceService _fuelPriceService;

	public SaveFuelPrice(
		IFuelPriceService fuelPriceService,
		ILogger<SaveFuelPrice> logger)
	{
		_fuelPriceService = fuelPriceService;
		_logger = logger;
	}
	
	[Function("SaveFuelPrice")]
	public async Task Run(
		[TimerTrigger("%TriggerTime%", RunOnStartup = true)]
		object timer,
		FunctionContext context)
	{
		try
		{
			await _fuelPriceService.SavePrices();
		}
		catch (Exception e)
		{
			_logger.LogCritical("Error while loading prices. Reason: {Reason}", e.Message);
		}
	}
}