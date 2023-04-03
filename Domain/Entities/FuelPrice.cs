using System;

namespace FuelPriceFunction.Domain.Entities;

public class FuelPrice
{
	public FuelPrice()
	{
	}

	public FuelPrice(DateTime period, decimal price)
	{
		Period = period;
		Price = price;
	}
	
	public DateTime Period { get; set; }
	
	public decimal Price { get; set; }
}