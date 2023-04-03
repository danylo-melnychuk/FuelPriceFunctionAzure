using System.Threading.Tasks;

namespace FuelPriceFunction.Services.Abstraction;

public interface IFuelPriceService
{
	Task SavePrices();
}