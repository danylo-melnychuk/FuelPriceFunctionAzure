namespace FuelPriceFunction.Configuration.Models;

public class SystemSettings
{
	public int RetentionPeriodDays { get; set; }

	public string FetchDataUrl { get; set; } = default!;
}