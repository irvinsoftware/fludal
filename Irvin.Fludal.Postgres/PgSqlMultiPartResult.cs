using System.Data.Common;
using Npgsql;

namespace Irvin.Fludal.Postgres;

public class PgSqlMultiPartResult : DbMultiPartResult
{
    public PgSqlMultiPartResult(string connectionAddress, NpgsqlCommand command, CancellationToken cancellationToken) 
        : base(connectionAddress, command, cancellationToken)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new NpgsqlConnection(connectionAddress);
    }

    protected override DbCursor<T> CreateCursor<T>()
    {
        return new PgSqlCursor<T>(_pipeline);
    }
}