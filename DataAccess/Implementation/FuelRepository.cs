using System.Collections.Generic;
using System.Threading.Tasks;
using FuelPriceFunction.Configuration.Models;
using FuelPriceFunction.DataAccess.Abstraction;
using FuelPriceFunction.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuelPriceFunction.DataAccess.Implementation;

public class FuelRepository : BaseRepository, IFuelRepository
{
	private readonly SystemSettings _systemSettings;

	public FuelRepository(
		IDataConnection dataConnection,
		ILogger<FuelRepository> logger,
		IOptions<SystemSettings> systemSettings) : base(dataConnection, logger)
	{
		_systemSettings = systemSettings.Value;
	}

	public Task SaveFuelPrice(
		IEnumerable<FuelPrice> prices) =>
		ExecuteAsync(
			StoredProcedureNames.InsertFuelPrice, 
			new
			{
				Prices = TableTypeHelper.CreateFuelPriceTable(prices),
				_systemSettings.RetentionPeriodDays
			});
}