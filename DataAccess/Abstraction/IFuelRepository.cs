using System.Collections.Generic;
using System.Threading.Tasks;
using FuelPriceFunction.Domain.Entities;

namespace FuelPriceFunction.DataAccess.Abstraction;

public interface IFuelRepository
{
	Task SaveFuelPrice(IEnumerable<FuelPrice> prices);
}