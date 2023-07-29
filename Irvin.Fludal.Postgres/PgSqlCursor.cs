using System.Data.Common;
using Npgsql;

namespace Irvin.Fludal.Postgres;

internal class PgSqlCursor : DbCursor
{
    protected PgSqlCursor(string connectionAddress, NpgsqlCommand command)
        : base(connectionAddress, command)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new NpgsqlConnection(connectionAddress);
    }
}

internal class PgSqlCursor<TModel> : DbCursor<TModel>
{
    public PgSqlCursor(string connectionAddress, NpgsqlCommand command) 
        : base(connectionAddress, command)
    {
    }

    public PgSqlCursor(ResourceStack pipeline) 
        : base(pipeline)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new NpgsqlConnection(connectionAddress);
    }
}