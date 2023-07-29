using System.Data.Common;
using Npgsql;

namespace Irvin.Fludal.Postgres;

internal class PgSqlExecutor : DbExecutor
{
    internal PgSqlExecutor(string connectionAddress, NpgsqlCommand command)
        : base(connectionAddress, command)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new NpgsqlConnection(connectionAddress);
    }
}
