using System.Data;

namespace FuelPriceFunction.DataAccess.Abstraction;

public interface IDataConnection
{
	IDbConnection Connection { get; }

	IDbTransaction? Transaction { get; }

	void Begin();

	void Commit();

	void Rollback();
}